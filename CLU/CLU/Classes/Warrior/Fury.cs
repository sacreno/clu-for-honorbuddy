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

using Styx.TreeSharp;
using System.Linq;
using CommonBehaviors.Actions;
using CLU.Helpers;
using CLU.Settings;
using CLU.Base;
using Styx.CommonBot;
using Styx;
using Styx.WoWInternals;
using Rest = CLU.Base.Rest;

namespace CLU.Classes.Warrior
{
    using global::CLU.Managers;

    class Fury : RotationBase
    {

        private const int ItemSetId = 1145; // Tier set ID Plate of Resounding Rings

        public override string Name
        {
            get { return "Fury Warrior"; }
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
            get { return "Bloodthirst"; }
        }
        public override int KeySpellId
        {
            get { return 23881; }
        }
        public override float CombatMaxDistance
        {
            get { return 3.2f; }
        }

        public override string Help
        {
            get
            {
                var twopceinfo = Item.Has2PcTeirBonus(ItemSetId) ? "2Pc Teir Bonus Detected" : "User does not have 2Pc Teir Bonus";
                var fourpceinfo = Item.Has2PcTeirBonus(ItemSetId) ? "2Pc Teir Bonus Detected" : "User does not have 2Pc Teir Bonus";
                return
                    @"
----------------------------------------------------------------------
Fury MoP:
[*] AutomaticCooldowns now works with Boss's or Mob's (See: General Settings)
This Rotation will:
1. Heal using Victory Rush, Enraged Regeneration, Rallying Cry, Healthstone
	==> Healthstone.
2. AutomaticCooldowns has:
    ==> UseTrinkets 
    ==> UseRacials 
    ==> UseEngineerGloves
    ==> Avatar, Bloodbath, Death Wish
NOTE: PvP uses single target rotation - It's not designed for PvP use until Dagradt changes that.
Credits: kbrebel04
----------------------------------------------------------------------" + twopceinfo + "\n" + fourpceinfo + "\n";
            }
        }

        private static bool ColossusSmashBloodthirstCooldown { get { return Spell.SpellCooldown("Colossus Smash").TotalSeconds > 1 && Spell.SpellCooldown("Bloodthirst").TotalSeconds > 1; } }

        public override Composite SingleRotation
        {
            get
            {
                return new PrioritySelector(
                    // Pause Rotation
                                new Decorator(ret => CLUSettings.Instance.PauseRotation, new ActionAlwaysSucceed()),

                                // For DS Encounters.
                                EncounterSpecific.ExtraActionButton(),

                                // Kill flying units.
                                Common.HandleFlyingUnits,

                                new Decorator(
                                    ret => Me.CurrentTarget != null && Unit.UseCooldowns(),
                                        new PrioritySelector(
                                        Item.UseTrinkets(),
                                        Racials.UseRacials(),
                                        Buff.CastBuff("Lifeblood", ret => true, "Lifeblood"), // Thanks Kink
                                        Item.UseEngineerGloves())),
                    // Interupts
                                    Spell.CastInterupt("Pummel", ret => true, "Pummel"),
                                    Spell.CastSelfSpell("Recklessness", ret => CLUSettings.Instance.Warrior.UseRecklessness && Unit.UseCooldowns() && Me.CurrentTarget != null && ((Buff.TargetDebuffTimeLeft("Colossus Smash").TotalSeconds >= 5 || Spell.SpellCooldown("Colossus Smash").TotalSeconds <= 4) && ((!SpellManager.HasSpell("Avatar") || !Item.Has4PcTeirBonus(ItemSetId))) && ((Me.CurrentTarget.HealthPercent < 20 || Unit.TimeToDeath(Me.CurrentTarget) > 315 || (Unit.TimeToDeath(Me.CurrentTarget) > 165 && Item.Has4PcTeirBonus(ItemSetId)))) || (SpellManager.HasSpell("Avatar") && Item.Has4PcTeirBonus(ItemSetId) && Buff.PlayerHasBuff("Avatar"))), "Recklessness"),
                                    Spell.CastSelfSpell("Avatar", ret => Me.CurrentTarget != null && (Unit.UseCooldowns() && SpellManager.HasSpell("Avatar") && (((Spell.SpellCooldown("Recklessness").TotalSeconds >= 180 || Buff.PlayerHasBuff("Recklessness")) || (Me.CurrentTarget.HealthPercent >= 20 && Unit.TimeToDeath(Me.CurrentTarget) > 195) || (Me.CurrentTarget.HealthPercent < 20 && Item.Has4PcTeirBonus(ItemSetId))) || Unit.TimeToDeath(Me.CurrentTarget) <= 20)), "Avatar"),
                                    Spell.CastSelfSpell("Bloodbath", ret => Me.CurrentTarget != null && (SpellManager.HasSpell("Bloodbath") && (((Spell.SpellCooldown("Recklessness").TotalSeconds >= 10 || Buff.PlayerHasBuff("Recklessness")) || (Me.CurrentTarget.HealthPercent >= 20 && (Unit.TimeToDeath(Me.CurrentTarget) <= 165 || (Unit.TimeToDeath(Me.CurrentTarget) <= 315 & !Item.Has4PcTeirBonus(ItemSetId))) && Unit.TimeToDeath(Me.CurrentTarget) > 75)) || Unit.TimeToDeath(Me.CurrentTarget) <= 19)), "Bloodbath"),
                                    Spell.CastSelfSpell("Berserker Rage", ret => Me.CurrentTarget != null && !(Buff.PlayerHasActiveBuff("Enrage") || (Buff.PlayerCountBuff("Raging Blow!") == 2 && Me.CurrentTarget.HealthPercent >= 20)) && Me.CurrentTarget.IsWithinMeleeRange, "Berserker Rage"),
                                    Spell.CastSelfSpell("Deadly Calm", ret => CLUSettings.Instance.Warrior.UseDeadlyCalm && Unit.UseCooldowns() && Me.CurrentRage >= 40 && (CLUSettings.Instance.UseAoEAbilities && Unit.EnemyUnits.Count() < 4 || !CLUSettings.Instance.UseAoEAbilities), "Deadly Calm"),
                                    Spell.CastSelfSpell("Skull Banner", ret => Me.CurrentTarget != null && Unit.UseCooldowns(), "Skull Banner"),
                    //AoE
                                    Spell.CastSpell("Raging Blow", ret => Me.HasAura(131116) && Buff.PlayerCountBuff("Meat Cleaver") > 2, "Raging Blow AoE"),
                                    Spell.CastSpell("Whirlwind", ret => Unit.EnemyUnits.Count() > 3 && ColossusSmashBloodthirstCooldown && Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 80) && (Buff.PlayerCountBuff("Meat Cleaver") < 3 || Buff.PlayerCountBuff("Meat Cleaver") > 2 && !Buff.PlayerHasActiveBuff("Raging Blow!")), "Whirlwind"),
                                    Spell.CastSpell("Cleave", ret => Me.CurrentTarget != null && Unit.EnemyUnits.Count() > 1 && Unit.EnemyUnits.Count() < 4 && CLUSettings.Instance.UseAoEAbilities && ColossusSmashBloodthirstCooldown && !Buff.PlayerHasActiveBuff("Raging Blow!") && (Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 80) || Buff.PlayerHasBuff("Deadly Calm") && Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 40)), "Cleave"),
                    //Single Target
                                    Spell.CastSpell("Bloodthirst", ret => true, "Bloodthirst"),
                                    Spell.CastSpell("Colossus Smash", ret => true, "Colossus Smash"),
                                    Spell.CastSpell("Execute", ret => Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 20, "Execute"),
                                    Spell.CastSpell("Raging Blow", ret => CLUSettings.Instance.UseAoEAbilities && Unit.EnemyUnits.Count() < 4 && Buff.PlayerHasActiveBuff("Raging Blow!") || !CLUSettings.Instance.UseAoEAbilities && Buff.PlayerHasActiveBuff("Raging Blow!"), "Raging Blow"),
                                    Spell.CastSpell("Wild Strike", ret => Me.HasAura(46916) && Me.CurrentTarget.HealthPercent >= 20 && Spell.SpellOnCooldown("Bloodthirst") && (CLUSettings.Instance.UseAoEAbilities && Unit.EnemyUnits.Count() < 4 || !CLUSettings.Instance.UseAoEAbilities), "Wild Strike"),
                                    Spell.CastSpell("Heroic Strike", ret => Me.CurrentTarget != null && Unit.EnemyUnits.Count() < 2 && (Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 80) || Buff.PlayerHasBuff("Deadly Calm") && Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 40)), "Heroic Strike"),
                                    Spell.CastSpell("Heroic Strike", ret => Me.CurrentTarget != null && !CLUSettings.Instance.UseAoEAbilities && (Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 80) || Buff.PlayerHasBuff("Deadly Calm") && Me.CurrentRage >= (TalentManager.HasGlyph("Unending Rage") ? 60 : 40)), "Heroic Strike"),
                                    Spell.CastSpell("Storm Bolt", ret => SpellManager.HasSpell("Storm Bolt"), "Storm Bolt"),
                                    Spell.CastConicSpell("Shockwave", 11f, 33f, ret => CLUSettings.Instance.Warrior.UseShockwave, "Shockwave"),
                                    Spell.CastSpell("Dragon Roar", ret => CLUSettings.Instance.Warrior.UseDragonRoar && Me.CurrentTarget.IsWithinMeleeRange && TalentManager.HasTalent(12), "Dragon Roar"),
                                    Spell.CastSpell("Heroic Throw", ret => StyxWoW.Me.Inventory.Equipped.MainHand != null, "Heroic Throw"),
                                    Spell.CastSpell("Bladestorm", ret => Me.CurrentTarget != null && (SpellManager.HasSpell("Bladestorm") && Spell.SpellCooldown("Colossus Smash").TotalSeconds >= 5 && !Buff.TargetHasDebuff("Colossus Smash") && Spell.SpellCooldown("Bloodthirst").TotalSeconds >= 2 && Me.CurrentTarget.HealthPercent >= 20), "Bladestorm"),
                                    Spell.CastSpell("Commanding Shout", ret => Me.RagePercent < 70 && !WoWSpell.FromId(469).Cooldown && CLUSettings.Instance.Warrior.ShoutSelection == WarriorShout.Commanding, "Commanding Shout for Rage"),
                                    Spell.CastSpell("Battle Shout", ret => Me.RagePercent < 70 && !WoWSpell.FromId(6673).Cooldown && CLUSettings.Instance.Warrior.ShoutSelection == WarriorShout.Battle, "Battle Shout for Rage"));
            }
        }

        public override Composite Pull
        {
            get { return this.SingleRotation; }
        }

        public override Composite Medic
        {
            get
            {
                return new Decorator(
                    ret => Me.HealthPercent < 100 && CLUSettings.Instance.EnableSelfHealing,
                    new PrioritySelector(
                    //Spell.CastSpell("Impending Victory",            ret => Me.CurrentTarget != null && !WoWSpell.FromId(103840).Cooldown && TalentManager.HasTalent(6) && Me.CurrentTarget.HealthPercent >= 20, "Impending Victory"),
                        Spell.CastSelfSpell("Enraged Regeneration", ret => Me.HealthPercent < 45 && !Buff.PlayerHasBuff("Rallying Cry"), "Enraged Regeneration"),
                        Spell.CastSelfSpell("Rallying Cry", ret => Me.HealthPercent < 45 && !Buff.PlayerHasBuff("Enraged Regeneration"), "Rallying Cry"),
                        Item.UseBagItem("Healthstone", ret => Me.HealthPercent < 40 && !Buff.PlayerHasBuff("Rallying Cry") && !Buff.PlayerHasBuff("Enraged Regeneration"), "Healthstone")));
            }
        }

        public override Composite PreCombat
        {
            get
            {
                return new Decorator(
                        ret => !Me.Mounted && !Me.IsDead && !Me.Combat && !Me.IsFlying && !Me.IsOnTransport && !Me.HasAura("Food") && !Me.HasAura("Drink"),
                        new PrioritySelector(
                            Buff.CastRaidBuff("Commanding Shout", ret => !WoWSpell.FromId(469).Cooldown, "Commanding Shout"),
                            Buff.CastRaidBuff("Battle Shout", ret => !WoWSpell.FromId(6673).Cooldown, "Battle Shout"),
                    //Buff.CastBuff("Berserker Stance",       ret => StyxWoW.Me.Shapeshift != CLUSettings.Instance.Warrior.StanceSelection && CLUSettings.Instance.Warrior.StanceSelection == ShapeshiftForm.BerserkerStance, "Stance is Berserker"),
                    //Buff.CastBuff("Battle Stance",          ret => StyxWoW.Me.Shapeshift != CLUSettings.Instance.Warrior.StanceSelection && CLUSettings.Instance.Warrior.StanceSelection == ShapeshiftForm.BattleStance, "Stance is Battle"),

                            Spell.CastSpell("Charge", ret => Me.CurrentTarget != null && CLU.LocationContext == GroupLogic.Battleground &&
                                (Macro.Manual || Unit.IsTrainingDummy(Me.CurrentTarget)) && Me.CurrentTarget.DistanceSqr > 3.2 * 3.2, "Charge"),

                            Spell.CastOnUnitLocation("Heroic Leap", ret => Me.CurrentTarget, ret => Me.CurrentTarget != null &&
                                CLU.LocationContext == GroupLogic.Battleground && (Macro.Manual ||
                                Unit.IsTrainingDummy(Me.CurrentTarget)) && Me.CurrentTarget.DistanceSqr > 3.2 * 3.2 &&
                                SpellManager.Spells["Charge"].CooldownTimeLeft.Seconds > 1 &&
                                SpellManager.Spells["Charge"].CooldownTimeLeft.Seconds < 18, "Heroic Leap")

                            ));
            }
        }

        public override Composite Resting
        {
            get { return Rest.CreateDefaultRestBehaviour(); }
        }

        public override Composite PVPRotation
        {
            get { return this.SingleRotation; }
        }

        public override Composite PVERotation
        {
            get { return this.SingleRotation; }
        }
    }
}