﻿#region Revision info
/*
 * $Author$
 * $Date$
 * $ID$
 * $Revision$
 * $URL$
 * $LastChangedBy$
 * $ChangesMade$
 */
#endregion

using CLU.Helpers;
using Styx.TreeSharp;
using System.Linq;
using CommonBehaviors.Actions;
using CLU.Lists;
using Styx.WoWInternals.WoWObjects;
using CLU.Settings;
using CLU.Base;
using Rest = CLU.Base.Rest;
using Styx.WoWInternals;

namespace CLU.Classes.Shaman
{

    class Elemental : RotationBase
    {
        public override string Name
        {
            get {
                return "Elemental Shaman";
            }
        }

        public override string Revision
        {
            get
            {
                return "$Rev$";
            }
        }

        public override string KeySpell
        {
            get {
                return "Thunderstorm";
            }
        }

        public override int KeySpellId
        {
            get { return 51490; }
        }

        public override float CombatMaxDistance
        {
            get {
                return 34f;
            }
        }

        public override float CombatMinDistance
        {
            get {
                return 28f;
            }
        }

        public override string Help
        {
            get {
                return "\n" +
                       "----------------------------------------------------------------------\n" +
                       "This Rotation will:\n" +
                       "1. Enchant Weapon: Flametongue Weapon(MainHand)\n" +
                       "2. Totem Bar: Stoneskin Totem, Wrath of Air, Healing Stream Totem, Searing (with range check) \n" +
                       "3. AutomaticCooldowns has: \n" +
                       "==> UseTrinkets \n" +
                       "==> UseRacials \n" +
                       "==> UseEngineerGloves \n" +
                       "==> Elemental Mastery \n" +
                       "==> Fire Elemental Totem \n" +
                       "==> Earth Elemental Totem \n" +
                       "4. AoE with Magma Totem, Chain Lightning, Earthquake\n" +
                       "4. Heal using: Lightning Shield, Shamanistic Rage, Healing Surge\n" +
                       "6. Best Suited for end game raiding\n" +
                       "NOTE: PvP rotations have been implemented in the most basic form, once MoP is released I will go back & revise the rotations for optimal functionality 'Dagradt'. \n" +
                       "Credits to Digitalmocking for his Initial Version and Stormchasing\n" +
                       "----------------------------------------------------------------------\n";
            }
        }

        public override Composite SingleRotation
        {
            get {
                return new PrioritySelector(
                           // Pause Rotation
                           new Decorator(ret => CLUSettings.Instance.PauseRotation, new ActionAlwaysSucceed()),

                           // For DS Encounters.
                           EncounterSpecific.ExtraActionButton(),

                           new Decorator(
                               ret => Me.CurrentTarget != null && Unit.IsTargetWorthy(Me.CurrentTarget),
                               new PrioritySelector(
                                   Item.UseTrinkets(),
                                   Racials.UseRacials(),
                                   Buff.CastBuff("Lifeblood", ret => true, "Lifeblood"), // Thanks Kink
                                   Item.UseEngineerGloves())),
                           // PvP TRICK.
                           // Spell.CastSpell("Purge",              ret => Spell.TargetHasPurgableBuff(), "Purge"), // this is working but it needs a hashset to prioritise what to purge as we dont want to purge blessings and shit, its on my to-do
                           // Interupts
                           Spell.CastInterupt("Wind Shear",                    ret => true, "Wind Shear"),
                           // Threat
                           Buff.CastBuff("Wind Shear",                         ret => Me.CurrentTarget != null && Me.CurrentTarget.ThreatInfo.RawPercent > 90, "Wind Shear (Threat)"),
                           // Totem management
                           Totems.CreateTotemsBehavior(),
                           // AoE
                           new Decorator(
                               ret => !Me.IsMoving && Me.CurrentTarget != null && Unit.CountEnnemiesInRange(Me.CurrentTarget.Location, 10) > 1 && !BossList.IgnoreAoE.Contains(Unit.CurrentTargetEntry),
                               new PrioritySelector(
                                   Spell.CastOnUnitLocation("Earthquake", u => Me.CurrentTarget, ret => Me.CurrentTarget != null && !Me.IsMoving && !Me.CurrentTarget.IsMoving && Me.ManaPercent > 60 && Unit.CountEnnemiesInRange(Me.CurrentTarget.Location, 10) > 4, "Earthquake"),
                                   Spell.CastSelfSpell("Thunderstorm",    ret => Me.ManaPercent < 16 || Unit.CountEnnemiesInRange(Me.Location, 10) > 1, "Thunderstorm"),
                                   Spell.CastSpell("Earth Shock",         ret => Buff.PlayerCountBuff("Lightning Shield") == 9, "Earth Shock"),
                                   Spell.CastSpell("Chain Lightning",     ret => true, "Chain Lightning")
                               )),
                           // Default Rotaion
                           Spell.CastSelfSpell("Elemental Mastery",           ret => Me.CurrentTarget != null && Unit.IsTargetWorthy(Me.CurrentTarget), "Elemental Mastery"),
                           Buff.CastDebuff("Flame Shock",                     ret => true, "Flame Shock"),
                           Spell.CastSpell("Lava Burst",                      ret => true, "Lava Burst"),
                           Spell.CastSpell("Earth Shock",                     ret => Buff.PlayerCountBuff("Lightning Shield") > 7, "Earth Shock"),
                           Buff.CastBuff("Spiritwalker's Grace",              ret => Me.IsMoving, "Spiritwalker's Grace"),
                           Spell.CastSpell("Lightning Bolt",                  ret => true, "Lightning Bolt"),
                           Spell.CastSpell("Unleash Elements",                ret => Me.IsMoving, "Unleash Elements - Moving"),
                           Spell.CastSelfSpell("Thunderstorm",                ret => Me.ManaPercent < 16 || Unit.CountEnnemiesInRange(Me.Location, 10) > 1, "Thunderstorm")
                       );
            }
        }

        public Composite burstRotation
        {
            get
            {
                return (
                    new PrioritySelector(
                ));
            }
        }

        public Composite baseRotation
        {
            get
            {
                return (
                    new PrioritySelector(
                        //new Action(a => { CLU.Log("I am the start of public Composite baseRotation"); return RunStatus.Failure; }),
                        //PvP Utilities

                        //Rotation
                        //6	0.00	wind_shear
                        //7	0.01	bloodlust,if=target.health.pct<25|time>5
                        //8	1.00	jade_serpent_potion,if=time>60&(pet.primal_fire_elemental.active|pet.greater_fire_elemental.active|target.time_to_die<=60)
                        //L	7.86	use_item,name=firebirds_gloves,if=((cooldown.ascendance.remains>10|level<87)&cooldown.fire_elemental_totem.remains>10)|buff.ascendance.up|buff.bloodlust.up|totem.fire_elemental_totem.active
                        //M	4.25	blood_fury,if=buff.bloodlust.up|buff.ascendance.up|((cooldown.ascendance.remains>10|level<87)&cooldown.fire_elemental_totem.remains>10)
                        //N	0.00	elemental_mastery,if=talent.elemental_mastery.enabled&time>15&((!buff.bloodlust.up&time<120)|(!buff.berserking.up&!buff.bloodlust.up&buff.ascendance.up)|(time>=200&(cooldown.ascendance.remains>30|level<87)))
                        //O	2.00	fire_elemental_totem,if=!active
                        //P	2.96	ascendance,if=dot.flame_shock.remains>buff.ascendance.duration&(target.time_to_die<20|buff.bloodlust.up|time>=180)
                        //Q	0.00	ancestral_swiftness,if=talent.ancestral_swiftness.enabled&!buff.ascendance.up
                        //R	0.00	unleash_elements,if=talent.unleashed_fury.enabled&!buff.ascendance.up
                        //S	15.30	flame_shock,if=!buff.ascendance.up&(!ticking|ticks_remain<2|((buff.bloodlust.up|buff.elemental_mastery.up)&ticks_remain<3))
                        //T	87.75	lava_burst,if=dot.flame_shock.remains>cast_time&(buff.ascendance.up|cooldown_react)
                        //U	29.71	elemental_blast,if=talent.elemental_blast.enabled&!buff.ascendance.up
                        //V	29.87	earth_shock,if=buff.lightning_shield.react=buff.lightning_shield.max_stack
                        //W	3.06	earth_shock,if=buff.lightning_shield.react>3&dot.flame_shock.remains>cooldown&dot.flame_shock.remains<cooldown+action.flame_shock.tick_time
                        //X	1.94	earth_elemental_totem,if=!active&cooldown.fire_elemental_totem.remains>=50
                        //Y	5.85	searing_totem,if=!totem.fire.active
                        //Z	0.00	spiritwalkers_grace,moving=1
                        //a	0.00	unleash_elements,moving=1
                        //b	141.47	lightning_bolt
                ));
            }
        }

        public override Composite Medic
        {
            get
            {
                return new PrioritySelector(
                    Common.HandleCompulsoryShamanBuffs(),
                    Common.HandleTotemRecall(),
                    // Healing shit.
                    new Decorator(
                           ret => Me.HealthPercent < 100 && CLUSettings.Instance.EnableSelfHealing,
                           new PrioritySelector(
                               Item.UseBagItem("Healthstone", ret => Me.HealthPercent < 30, "Healthstone"),
                               Buff.CastBuff("Shamanistic Rage", ret => Me.CurrentTarget != null && (Me.HealthPercent < 60 || (Me.ManaPercent < 65 && Me.CurrentTarget.HealthPercent >= 75)), "Shamanistic Rage"),
                               Spell.CastSelfSpell("Healing Surge", ret => Me.HealthPercent < 35, "Healing Surge"))));
            }
        }

        public override Composite PreCombat
        {
            get
            {
                return (
                    new Decorator(ret => !Me.Mounted && !Me.IsDead && !Me.Combat && !Me.IsFlying && !Me.IsOnTransport && !Me.HasAura("Food") && !Me.HasAura("Drink"),
                        new PrioritySelector(
                            //flask,type=warm_sun
                            //food,type=mogu_fish_stew
                            Common.HandleCompulsoryShamanBuffs(),
                            //jade_serpent_potion
                            Common.HandleTotemRecall()
                )));
            }
        }

        public override Composite Resting
        {
            get {
                return Rest.CreateDefaultRestBehaviour();
            }
        }

        public override Composite PVPRotation
        {
            get
            {
                return (
                    new PrioritySelector(
                    //new Action(a => { CLU.Log("I am the start of public override Composite PVPRotation"); return RunStatus.Failure; }),
                        CrowdControl.freeMe(),
                        new Decorator(ret => Macro.Manual || BotChecker.BotBaseInUse("BGBuddy"),
                            new Decorator(ret => Me.CurrentTarget != null && Unit.IsTargetWorthy(Me.CurrentTarget),
                                new PrioritySelector(
                                    new Decorator(ret => Macro.rotationSwap && !BotChecker.BotBaseInUse("BGbuddy"), wepSwapDefensive),
                                    new Decorator(ret => !Macro.rotationSwap && !BotChecker.BotBaseInUse("BGbuddy"), wepSwapOffensive),
                                    Item.UseTrinkets(),
                                    Racials.UseRacials(),
                                    Buff.CastBuff("Lifeblood", ret => true, "Lifeblood"),
                                    Item.UseEngineerGloves(),
                                    new Action(delegate
                                    {
                                        Macro.isMultiCastMacroInUse();
                                        return RunStatus.Failure;
                                    }),
                                    new Decorator(ret => Macro.Burst, burstRotation),
                                    new Decorator(ret => !Macro.Burst || BotChecker.BotBaseInUse("BGBuddy"), baseRotation)))
                )));
            }
        }

        public override Composite PVERotation
        {
            get {
                return this.SingleRotation;
            }
        }

        #region Weapon swap stuff

        public Composite wepSwapDefensive
        {
            get
            {
                return (
                    new Decorator(ret => Me.Inventory.Equipped.OffHand == null && !string.IsNullOrEmpty(CLUSettings.Instance.Warrior.PvPMainHandItemName) && !string.IsNullOrEmpty(CLUSettings.Instance.Warrior.PvPOffHandItemName) && CLUSettings.Instance.Warrior.PvPMainHandItemName != "Input the name of your mainhand weapon here" && CLUSettings.Instance.Warrior.PvPOffHandItemName != "Input the name of your offhand weapon here",
                        new Action(delegate
                        {
                            CLU.Log("Switching to defensive mode. Using MainHand: [{0}] Using OffHand: [{1}]", CLUSettings.Instance.Warrior.PvPMainHandItemName, CLUSettings.Instance.Warrior.PvPOffHandItemName);
                            Lua.DoString("RunMacroText(\"/equipslot 16 " + CLUSettings.Instance.Warrior.PvPMainHandItemName + "\")");
                            Lua.DoString("RunMacroText(\"/equipslot 17 " + CLUSettings.Instance.Warrior.PvPOffHandItemName + "\")");
                            return RunStatus.Failure;
                        })
                ));
            }
        }

        public Composite wepSwapOffensive
        {
            get
            {
                return (
                    new Decorator(ret => Me.Inventory.Equipped.OffHand != null && !string.IsNullOrEmpty(CLUSettings.Instance.Warrior.PvPTwoHandItemName) && CLUSettings.Instance.Warrior.PvPTwoHandItemName != "Input the name of your TwoHand weapon here",
                        new Action(delegate
                        {
                            CLU.Log("Switching to offensive mode. Using TwoHand: [{0}] ", CLUSettings.Instance.Warrior.PvPTwoHandItemName);
                            Lua.DoString("RunMacroText(\"/equipslot 16 " + CLUSettings.Instance.Warrior.PvPTwoHandItemName + "\")");
                            return RunStatus.Failure;
                        })
                ));
            }
        }
        #endregion
    }
}
