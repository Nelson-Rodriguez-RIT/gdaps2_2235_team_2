using Microsoft.Xna.Framework;
using Moonwalk.Classes.Entities;
using System;
using Moonwalk.Classes.Helpful_Stuff;
using static Moonwalk.Classes.Entities.Player;
using Moonwalk.Classes.Managers;

namespace Moonwalk.Classes.Boss {
    internal class BloodKing : BossFight {
        private Player player;

        private double activeActionTimer = 0;
        private double attackCooldownTimer = 0;

        private bool enraged = true;
        private bool charged = false;

        AbilityCooldowns<Abilities> abilities;
        
        enum Abilities {
            DoubleSlash,
            Execution,
            ChargedSlash,
            ComboP1,
            ComboP2,
            Teleport,
            Charge
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
            SwitchBehavior(Animations.Idle);

            abilities = new AbilityCooldowns<Abilities>(properties);

            this.player = player;
            center = initialPosition;
        }

        public override void Update(GameTime gameTime) {
            activeActionTimer = activeActionTimer <= 0 ? 0 : activeActionTimer - gameTime.ElapsedGameTime.TotalSeconds;
            
            if (activeActionTimer == 0)
                attackCooldownTimer = attackCooldownTimer <= 0 ? 0 : attackCooldownTimer - gameTime.ElapsedGameTime.TotalSeconds;

            abilities.Update(gameTime);
            activeAnimation.UpdateAnimation(gameTime);

            AI(gameTime);

            activeAnimation.FaceDirection = center.X - player.Position.X < 0 ? 0 : 1;
        }


        private void AI(GameTime gameTime) {
            int playerDistance = (int)Math.Abs((center.X + int.Parse(properties["HitboxX"]) / 2) - player.Center.X);

            if (activeActionTimer != 0) // Ignore AI if in the middle of an action
                return;

            
            if (attackCooldownTimer == 0) {

                if (playerDistance <= 8 && abilities.UseAbility(Abilities.Execution)) {
                    SwitchBehavior(Animations.Execution);
                    activeActionTimer = abilities[Abilities.Execution, true];
                    attackCooldownTimer = activeActionTimer;
                    return;
                }

                if (playerDistance <= 25 && abilities.UseAbility(Abilities.DoubleSlash)) {
                    SwitchBehavior(Animations.DoubleSlash);
                    activeActionTimer = abilities[Abilities.DoubleSlash, true];
                    attackCooldownTimer = activeActionTimer;
                    return;
                }

                if (enraged && playerDistance % 2 == 0) {
                    if (playerDistance <= 60 && charged && abilities.UseAbility(Abilities.ChargedSlash)) {
                        SwitchBehavior(Animations.ChargeSlash);
                        activeActionTimer = abilities[Abilities.ChargedSlash, true];
                        attackCooldownTimer = activeActionTimer;
                        charged = false;
                        return;
                    }
                }
            }

            if (playerDistance % 30 > 20 && enraged && !charged && abilities.UseAbility(Abilities.Charge)) {
                SwitchBehavior(Animations.Charge);
                activeActionTimer = abilities[Abilities.Charge, true];
                charged = true;
                return;
            }

            if (playerDistance > 5) {
                center.X += int.Parse(properties["MoveSpeed"]) * activeAnimation.FaceDirection == 0 ? 1 : -1;
                SwitchBehavior(Animations.Run, false);
                return;
            }

            

            SwitchBehavior(Animations.Idle, false);
        }
    }
}
