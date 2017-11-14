﻿using System.Linq;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;

namespace SkywrathMagePlus
{
    internal class Extensions
    {
        private MenuManager Menu { get; }

        public Extensions(Config config)
        {
            Menu = config.Menu;
        }

        public bool Active(Hero target)
        {
            var stunDebuff = target.Modifiers.FirstOrDefault(x => x.IsStunDebuff);

            var BorrowedTime = target.GetAbilityById(AbilityId.abaddon_borrowed_time);
            var PowerCogs = target.GetAbilityById(AbilityId.rattletrap_power_cogs);
            var BlackHole = target.GetAbilityById(AbilityId.enigma_black_hole);
            var FiendsGrip = target.GetAbilityById(AbilityId.bane_fiends_grip);
            var DeathWard = target.GetAbilityById(AbilityId.witch_doctor_death_ward);

            return (target.MovementSpeed < 240
                || (stunDebuff != null && stunDebuff.Duration >= 1)
                || target.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                || target.HasModifier("modifier_rod_of_atos_debuff")
                || target.HasModifier("modifier_crystal_maiden_frostbite")
                || target.HasModifier("modifier_crystal_maiden_freezing_field")
                || target.HasModifier("modifier_naga_siren_ensnare")
                || target.HasModifier("modifier_meepo_earthbind")
                || target.HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
                || target.HasModifier("modifier_legion_commander_duel")
                || target.HasModifier("modifier_kunkka_torrent")
                || target.HasModifier("modifier_enigma_black_hole_pull")
                || (BlackHole != null && BlackHole.IsInAbilityPhase)
                || target.HasModifier("modifier_ember_spirit_searing_chains")
                || target.HasModifier("modifier_dark_troll_warlord_ensnare")
                || target.HasModifier("modifier_rattletrap_cog_marker")
                || (PowerCogs != null && PowerCogs.IsInAbilityPhase)
                || target.HasModifier("modifier_axe_berserkers_call")
                || target.HasModifier("modifier_faceless_void_chronosphere_freeze")
                || (FiendsGrip != null && FiendsGrip.IsInAbilityPhase)
                || (DeathWard != null && DeathWard.IsInAbilityPhase)
                || target.HasModifier("modifier_winter_wyvern_cold_embrace"))
                && (BorrowedTime == null || BorrowedTime.Owner.Health > 2000 || BorrowedTime.Cooldown > 0)
                && !target.HasModifier("modifier_dazzle_shallow_grave")
                && !target.HasModifier("modifier_spirit_breaker_charge_of_darkness")
                && !target.HasModifier("modifier_pugna_nether_ward_aura");
        }

        public bool Disable(Hero target)
        {
            var QueenofPainBlink = target.GetAbilityById(AbilityId.queenofpain_blink);
            var AntiMageBlink = target.GetAbilityById(AbilityId.antimage_blink);
            var ManaVoid = target.GetAbilityById(AbilityId.antimage_mana_void);
            var Duel = target.GetAbilityById(AbilityId.legion_commander_duel);
            var Doom = target.GetAbilityById(AbilityId.doom_bringer_doom);
            var TimeWalk = target.GetAbilityById(AbilityId.faceless_void_time_walk);
            var ChronoSphere = target.GetAbilityById(AbilityId.faceless_void_chronosphere);
            var DeathWard = target.GetAbilityById(AbilityId.witch_doctor_death_ward);
            var PowerCogs = target.GetAbilityById(AbilityId.rattletrap_power_cogs);
            var Ravage = target.GetAbilityById(AbilityId.tidehunter_ravage);
            var BerserkersCall = target.GetAbilityById(AbilityId.axe_berserkers_call);
            var PrimalSplit = target.GetAbilityById(AbilityId.brewmaster_primal_split);
            var GuardianAngel = target.GetAbilityById(AbilityId.omniknight_guardian_angel);
            var SonicWave = target.GetAbilityById(AbilityId.queenofpain_sonic_wave);
            var SlithereenCrush = target.GetAbilityById(AbilityId.slardar_slithereen_crush);
            var FingerofDeath = target.GetAbilityById(AbilityId.lion_finger_of_death);
            var LagunaBlade = target.GetAbilityById(AbilityId.lina_laguna_blade);

            return (QueenofPainBlink != null && QueenofPainBlink.IsInAbilityPhase)
                || (AntiMageBlink != null && AntiMageBlink.IsInAbilityPhase)
                || (ManaVoid != null && ManaVoid.IsInAbilityPhase)
                || (Duel != null && Duel.IsInAbilityPhase)
                || (Doom != null && Doom.IsInAbilityPhase)
                || (TimeWalk != null && TimeWalk.IsInAbilityPhase)
                || (ChronoSphere != null && ChronoSphere.IsInAbilityPhase)
                || (DeathWard != null && DeathWard.IsInAbilityPhase)
                || (PowerCogs != null && PowerCogs.IsInAbilityPhase)
                || (Ravage != null && Ravage.IsInAbilityPhase)
                || (BerserkersCall != null && BerserkersCall.IsInAbilityPhase)
                || (PrimalSplit != null && PrimalSplit.IsInAbilityPhase)
                || (GuardianAngel != null && GuardianAngel.IsInAbilityPhase)
                || (SonicWave != null && SonicWave.IsInAbilityPhase)
                || (SlithereenCrush != null && SlithereenCrush.IsInAbilityPhase)
                || (FingerofDeath != null && FingerofDeath.IsInAbilityPhase)
                || (LagunaBlade != null && LagunaBlade.IsInAbilityPhase);
        }

        public bool Cancel(Hero target)
        {
            return !target.IsMagicImmune() && !target.IsInvulnerable()
                && !target.HasAnyModifiers("modifier_abaddon_borrowed_time", "modifier_item_combo_breaker_buff")
                && !target.HasAnyModifiers("modifier_winter_wyvern_winters_curse_aura", "modifier_winter_wyvern_winters_curse")
                && !DuelAghanimsScepter(target);
        }

        public bool ConcussiveShotTarget(Hero target, Hero targetHit)
        {
            if (!Menu.ConcussiveShotTargetItem)
            {
                return true;
            }

            if (targetHit == null)
            {
                return false;
            }

            if (target == targetHit)
            {
                return true;
            }

            if (target.Distance2D(targetHit) < 200)
            {
                return true;
            }

            return false;
        }

        public bool DuelAghanimsScepter(Hero target)
        {
            var duelAghanimsScepter = false;
            if (target.HasModifier("modifier_legion_commander_duel"))
            {
                duelAghanimsScepter = EntityManager<Hero>.Entities.Any(x =>
                                                                       x.HeroId == HeroId.npc_dota_hero_legion_commander &&
                                                                       x.IsValid &&
                                                                       x.IsVisible &&
                                                                       x.IsAlive &&
                                                                       x.HasAghanimsScepter());
            }

            return duelAghanimsScepter;
        }
    }
}
