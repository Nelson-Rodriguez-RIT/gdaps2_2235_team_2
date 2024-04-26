using Microsoft.Xna.Framework;
using Moonwalk.Classes.Entities;
using System;
using Moonwalk.Classes.Helpful_Stuff;
using static Moonwalk.Classes.Entities.Player;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace Moonwalk.Classes.Boss {
    internal class BloodKing : BossFight {
        private Player player;

        private double activeActionTimer = 0;
        private double attackCooldownTimer = 0;

        private double hitboxDelayTimer = 0;

        private bool enraged = false;
        private bool charged = false;

        private bool activated = true;

        private int bufferFaceDirectionWhenAttacking = -1;

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
            TeleportOut,

            DoubleSlashEnd = -8,
            ChargeSlashEnd = -10
        }

        enum AttackHitboxs {
            Execution,
            DoubleSlash,
        }


        public BloodKing(Vector2 initialPosition, Player player) : base("../../../Content/Entities/BloodKing") {
            SwitchBehavior(Animations.Idle);

            abilities = new AbilityCooldowns<Abilities>(properties);

            this.player = player;
            center = initialPosition;

            foreach (KeyValuePair<string, List<Rectangle>> hitbox in hitboxData)
                hitboxes.Add(hitbox.Value[0]);
        }

        public override void Update(GameTime gameTime) {
            player = (Player)GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList()[0];
            if (!activated) return;

            // When activeActionTimer ends
            if (activeActionTimer != 0 && (activeActionTimer = activeActionTimer <= 0 ? 0 : activeActionTimer - gameTime.ElapsedGameTime.TotalSeconds) == 0) {
                bufferFaceDirectionWhenAttacking = -1;

                switch ((Animations)activeAnimation.AnimationValue) {
                    case Animations.ChargeSlash:
                        break;
                }
            }

            // When hitboxDelayTimer ends
            if (hitboxDelayTimer > 0 && (hitboxDelayTimer -= gameTime.ElapsedGameTime.TotalSeconds) <= 0) {
                switch ((Animations)activeAnimation.TrueAnimationValue) {
                    case Animations.Execution:
                        Hitbox attackExecution =
                            new Hitbox(
                                .2, // 12 frames
                                hitboxes[(int)AttackHitboxs.Execution],
                                center,
                                new List<IDamageable>() { player },
                                activeAnimation.FaceDirection);
                        attackExecution.targetEntered += DealDamage;
                        break;

                    case Animations.DoubleSlash:
                        Hitbox attackDoubleSlash =
                            new Hitbox(
                                .1, // 12 frames
                                hitboxes[(int)AttackHitboxs.DoubleSlash],
                                center,
                                new List<IDamageable>() { player },
                                activeAnimation.FaceDirection);
                        attackDoubleSlash.targetEntered += DealDamage;

                        SwitchBehavior(Animations.DoubleSlashEnd, false);
                        hitboxDelayTimer = 0.25;

                        break;
                    case Animations.DoubleSlashEnd:
                        Hitbox attackDoubleSlashEnd =
                            new Hitbox(
                                .1, // 12 frames
                                hitboxes[(int)AttackHitboxs.DoubleSlash],
                                center,
                                new List<IDamageable>() { player },
                                activeAnimation.FaceDirection);
                        attackDoubleSlashEnd.targetEntered += DealDamage;
                        break;
                }
            }

           

            abilities.Update(gameTime);
            activeAnimation.UpdateAnimation(gameTime);

            if (activeActionTimer != 0 && bufferFaceDirectionWhenAttacking == -1)
                bufferFaceDirectionWhenAttacking = center.X - player.Position.X < 0 ? 0 : 1;

            if (activeActionTimer == 0)
                attackCooldownTimer = attackCooldownTimer <= 0 ? 0 : attackCooldownTimer - gameTime.ElapsedGameTime.TotalSeconds;

            AI(gameTime);

            if (bufferFaceDirectionWhenAttacking == -1)
                activeAnimation.FaceDirection = center.X - player.Position.X < 0 ? 0 : 1;
            else
                activeAnimation.FaceDirection = bufferFaceDirectionWhenAttacking;
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

                    hitboxDelayTimer = 0.6;
                    return;
                }

                if (playerDistance <= 25 && abilities.UseAbility(Abilities.DoubleSlash)) {
                    SwitchBehavior(Animations.DoubleSlash);

                    activeActionTimer = abilities[Abilities.DoubleSlash, true];
                    attackCooldownTimer = activeActionTimer;

                    hitboxDelayTimer = 0.25;
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

            //if (enraged && playerDistance >= 60) {
            //    SwitchBehavior(Animations.TeleportIn_Death);
            //    return;
            //}

            if (playerDistance >= 7) {
                center.X += int.Parse(properties["MoveSpeed"]) * activeAnimation.FaceDirection == 0 ? 1 : -1;
                SwitchBehavior(Animations.Run, false);
                return;
            }

            

            SwitchBehavior(Animations.Idle, false);
        }


        protected override void DealDamage(List<IDamageable> damageables) {
            Animations attack = (Animations)activeAnimation.AnimationValue;

            foreach (IDamageable damageable in damageables)
                damageable.TakeDamage(int.Parse(properties[$"{attack}Damage"]));
        }
    }
}
