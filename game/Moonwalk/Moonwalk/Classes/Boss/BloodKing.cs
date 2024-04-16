using Microsoft.Xna.Framework;
using Moonwalk.Classes.Entities;
using System;
using Moonwalk.Classes.Helpful_Stuff;

namespace Moonwalk.Classes.Boss {

    public delegate void BloodKingFlagHandler();
    
    internal class BloodKing : BossFight {
        public event BloodKingFlagHandler BloodKingStartFlag;

        // Flags
        private bool enabled = false;
        private bool initialStage = true;
        private bool charged = false;
        private bool action = false;

        // Timers
        private float apporachTimer = 0;
        private float attackTimer = 0;
        private float abilityTimer = 0;

        private Player playerTarget;

        private Behavior activeBehavior = Behavior.Distant;
        private Abilities activeAbility;
        private Attacks activeAttack;


        enum Abilities {
            Teleport,
            Charge
        }
        enum Behavior {
            Idle,
            Distant,
            Approaching,
            Attacking,
            Death,
        }
        enum Attacks {
            DoubleSlash,
            Execution,
            ChargeSlash,
            Combo,
        }
        enum Animations {
            Idle,
            ToCharge,
            Charge,
            FromCharge,
            Run,
            Jump,
            JumpToFall,
            Fall,
            DoubleSlash,
            Execution,
            ChargeSlash,
            ComboP1,
            ComboP2,
            Hit,
            TeleportIn_Death,
            HeartSlam,
            TeleportOut
        }


        public BloodKing(Vector2 initialPosition, Player player) : base("../../../Content/Entities/BloodKing") {
            BloodKingStartFlag += Begin;

            center = initialPosition;
            playerTarget = player;
            SwitchBehavior(Animations.Idle);
        }


        public void Begin() {
            enabled = true;
        }

        public override void Update(GameTime gameTime) {
            bool active = false;
            activeAnimation.UpdateAnimation(gameTime);

            if (!enabled)
                return;

            float temp = VectorMath.Difference(center, playerTarget.Position.ToVector2()).X;
            faceDirection = temp > 0 
                ? FaceDirection.Right : FaceDirection.Left;


            // Update timers
            apporachTimer = (apporachTimer <= 0 ? 0 : apporachTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
            attackTimer = (attackTimer <= 0 ? 0 : attackTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
            abilityTimer = (abilityTimer <= 0 ? 0 : abilityTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);

            
            // If in the middle of an attack or ability, continue that ability and ignore loop
            if (attackTimer > 0) {
                active = true;

                if (!action)
                    switch (activeAttack) {
                        // Complete the second part of the combo if relevant
                        case Attacks.Combo:
                            if (attackTimer <= 3.50f && !action) {
                                action = true;
                                SwitchBehavior(Animations.ComboP2);
                            }
                            break;
                }
            }

            if (abilityTimer > 0) {
                active = true;

                if (!action)
                    switch (activeAbility) {
                        case Abilities.Teleport:
                            if (abilityTimer <= 1.5f && activeAnimation.AnimationValue != (int)Animations.TeleportOut) {
                                center = playerTarget.Position.ToVector2();
                                center.Y += 5;
                                SwitchBehavior(Animations.TeleportOut);
                            }
                            break;

                        case Abilities.Charge:
                            if (abilityTimer < 0.1f) {
                                charged = true;
                                action = true;
                            }
                            break;
                    }


            }

            if (apporachTimer > 0) {
                active = true;

                center.X += 10 * (float)gameTime.ElapsedGameTime.TotalSeconds * (faceDirection == FaceDirection.Right ? 1 : -1);
            }

            if (!active)
                AI();

            activeAnimation.FaceDirection = (int)faceDirection;
        }


        private void AI() {
            float distanceFromPlayer = Math.Abs(center.X - playerTarget.Position.X);
            int actionValue = rng.Next(10);

            switch (activeBehavior) {
                // When initially approaching, decide whether to walk or teleport closer
                case Behavior.Distant:
                    if (actionValue < 5)
                        UseAbility(Abilities.Teleport);
                    else
                        activeBehavior = Behavior.Approaching;
                    break;

                // When normally approaching
                case Behavior.Approaching:
                    // If in second stage, give Blood King an oppertunity to use their strong attacks
                    if (distanceFromPlayer < 70 && !initialStage && actionValue < 2) {
                        if (actionValue % 2 == 0)
                            UseAttack(Attacks.Combo);
                        else
                            if (!charged)
                            UseAbility(Abilities.Charge);
                        else
                            UseAttack(Attacks.ChargeSlash);

                    }

                    // If close enough, began attacking the player normally
                    else if (distanceFromPlayer < 25)
                        activeBehavior = Behavior.Attacking;

                    // Continue approaching player most of the time
                    // First phase increases the odds of this
                    else if (actionValue > 2 + (initialStage ? 0 : 3)) {
                        apporachTimer = 1;
                        SwitchBehavior(Animations.Run);
                    }

                    // Occasionally teleport to player
                    else
                        UseAbility(Abilities.Teleport);

                    break;

                case Behavior.Attacking:
                    // If too far from player begin approaching again
                    if (distanceFromPlayer > 70)
                        activeBehavior = Behavior.Approaching;

                    // If player is very close, have a chance at an insta kill
                    else if (actionValue % 2 == 0 && distanceFromPlayer < 10)
                        UseAttack(Attacks.Execution);

                    // During the second stage, have a chance at user either strong attack
                    else if (!initialStage && actionValue > 7)
                        UseAttack(Attacks.Combo);
                    else if (!initialStage && actionValue < 2) {
                        if (charged)
                            UseAttack(Attacks.ChargeSlash);
                        else
                            UseAbility(Abilities.Charge);
                    }

                    // Chance in Phase 2 to get closer for a normal attack
                    else if (!initialStage && distanceFromPlayer > 30) {
                        apporachTimer = 0.5f;
                        SwitchBehavior(Animations.Run);
                    }
                        

                    else {
                        // If player is medium distance, have a chance to basic attack
                        if (actionValue > 4 && distanceFromPlayer < 30)
                            UseAttack(Attacks.DoubleSlash);
                    }
                        
                    break;
            }
        }

        private void UseAttack(Attacks attack) {
            activeAttack = attack;

            switch (attack) {
                case Attacks.DoubleSlash:
                    SwitchBehavior(Animations.DoubleSlash);
                    attackTimer = 3.5f;
                    break;

                case Attacks.Execution:
                    SwitchBehavior(Animations.Execution);
                    attackTimer = 3.25f;
                    break;

                case Attacks.ChargeSlash:
                    charged = false;
                    SwitchBehavior(Animations.ChargeSlash);
                    attackTimer = 4.75f;
                    break;

                case Attacks.Combo:
                    SwitchBehavior(Animations.ComboP1);
                    attackTimer = 6.50f;
                    action = false;
                    break;

            }
        }

        private void UseAbility(Abilities ability) {
            activeAbility = ability;

            switch (ability) {
                case Abilities.Charge:
                    action = false;
                    charged = true;
                    abilityTimer = 1f;
                    SwitchBehavior(Animations.Charge);
                    break;

                case Abilities.Teleport:
                    action = false;
                    abilityTimer = 3.5f;
                    SwitchBehavior(Animations.TeleportIn_Death);
                    break;
            }
        }

        private void ApproachPlayerCycle() {

        }
    }
}
