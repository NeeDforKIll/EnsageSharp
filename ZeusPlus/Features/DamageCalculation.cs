﻿using System.Collections.Generic;
using System.Linq;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;

using ZeusPlus.Extensioms;

namespace ZeusPlus.Features
{
    internal class DamageCalculation
    {
        private Config Config { get; }

        private MenuManager Menu { get; }

        private ZeusPlus Main { get; }

        private Unit Owner { get; }

        public DamageCalculation(Config config)
        {
            Config = config;
            Menu = config.Menu;
            Main = config.Main;
            Owner = config.Main.Context.Owner;

            UpdateManager.Subscribe(OnUpdate);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void OnUpdate()
        {
            var heroes = EntityManager<Hero>.Entities.Where(x => x.IsValid && !x.IsIllusion).ToList();

            DamageList.Clear();

            foreach (var hero in heroes.Where(x => x.IsAlive && x.IsEnemy(Owner)).ToList())
            {
                List<BaseAbility> abilities = new List<BaseAbility>();

                var damage = 0.0f;
                var readyDamage = 0.0f;
                var totalDamage = 0.0f;

                if (hero.IsVisible)
                {
                    // Veil
                    var Veil = Main.Veil;
                    if (Veil != null && Veil.Ability.IsValid && Menu.AutoKillStealToggler.Value.IsEnabled(Veil.ToString()))
                    {
                        abilities.Add(Veil);
                    }

                    // Ethereal
                    var Ethereal = Main.Ethereal;
                    if (Ethereal != null && Ethereal.Ability.IsValid && Menu.AutoKillStealToggler.Value.IsEnabled(Ethereal.ToString()))
                    {
                        abilities.Add(Ethereal);
                    }

                    // Shivas
                    var Shivas = Main.Shivas;
                    if (Shivas != null && Shivas.Ability.IsValid && Menu.AutoKillStealToggler.Value.IsEnabled(Shivas.ToString()))
                    {
                        abilities.Add(Shivas);
                    }

                    // StaticField
                    var StaticField = Main.StaticField;
                    if (StaticField.Ability.Level > 0 && Menu.AutoKillStealToggler.Value.IsEnabled(StaticField.ToString()))
                    {
                        abilities.Add(StaticField);
                    }

                    // LightningBolt
                    var LightningBolt = Main.LightningBolt;
                    if (LightningBolt.Ability.Level > 0 && Menu.AutoKillStealToggler.Value.IsEnabled(LightningBolt.ToString()))
                    {
                        abilities.Add(LightningBolt);
                    }

                    // Dagon
                    var Dagon = Main.Dagon;
                    if (Dagon != null && Dagon.Ability.IsValid && Menu.AutoKillStealToggler.Value.IsEnabled(Dagon.ToString()))
                    {
                        abilities.Add(Dagon);
                    }

                    // ArcLightning
                    var ArcLightning = Main.ArcLightning;
                    if (ArcLightning.Ability.Level > 0 && Menu.AutoKillStealToggler.Value.IsEnabled(ArcLightning.ToString()))
                    {
                        abilities.Add(ArcLightning);
                    }

                    // Nimbus
                    var Nimbus = Main.Nimbus;
                    if (Nimbus.Ability.Level > 0 && Menu.AutoKillStealToggler.Value.IsEnabled(Nimbus.ToString()))
                    {
                        abilities.Add(Nimbus);
                    }

                    // Thundergods Wrath
                    var ThundergodsWrath = Main.ThundergodsWrath;
                    if (ThundergodsWrath.Ability.Level > 0 && Menu.AutoKillStealToggler.Value.IsEnabled(ThundergodsWrath.ToString()))
                    {
                        abilities.Add(ThundergodsWrath);
                    }
                }

                var damageCalculation = new Combo(abilities.ToArray());
                var damageReduction = -DamageReduction(hero, heroes);
                var damageBlock = DamageBlock(hero, heroes);

                damage += DamageHelpers.GetSpellDamage(damageCalculation.GetDamage(hero) + damageBlock, 0, damageReduction);
                readyDamage += DamageHelpers.GetSpellDamage(damageCalculation.GetDamage(hero, true, false) + damageBlock, 0, damageReduction);
                totalDamage += DamageHelpers.GetSpellDamage(damageCalculation.GetDamage(hero, false, false) + damageBlock, 0, damageReduction);

                damage -= LivingArmor(hero, heroes, damageCalculation.Abilities);
                readyDamage -= LivingArmor(hero, heroes, damageCalculation.Abilities);
                totalDamage -= LivingArmor(hero, heroes, damageCalculation.Abilities);

                if (hero.HasModifiers(BlockModifiers) || hero.IsInvulnerable())
                {
                    damage = 0.0f;
                    readyDamage = 0.0f;
                }

                DamageList.Add(new Damage(hero, damage, readyDamage, totalDamage, hero.Health));
            }
        }

        public static string[] BlockModifiers { get; } =
        {
            "modifier_abaddon_borrowed_time",
            "modifier_item_combo_breaker_buff",
            "modifier_winter_wyvern_winters_curse_aura",
            "modifier_winter_wyvern_winters_curse",
            "modifier_templar_assassin_refraction_absorb",
            "modifier_oracle_fates_edict"
        };

        private float LivingArmor(Hero hero, List<Hero> heroes, IReadOnlyCollection<BaseAbility> abilities)
        {
            if (!hero.HasModifier("modifier_treant_living_armor"))
            {
                return 0;
            }

            var treant = heroes.FirstOrDefault(x => x.IsEnemy(Owner) && x.HeroId == HeroId.npc_dota_hero_treant);
            var ability = treant.GetAbilityById(AbilityId.treant_living_armor);
            var block = ability.GetSpecialData("damage_block");

            var count = abilities.Where(x => x.GetDamage(hero) > block).Count();

            return count * block;
        }

        private float DamageReduction(Hero hero, List<Hero> heroes)
        {
            var value = 0.0f;

            // Bristleback
            var bristleback = hero.GetAbilityById(AbilityId.bristleback_bristleback);
            if (bristleback != null && bristleback.Level != 0)
            {
                var brist = bristleback.Owner as Hero;
                if (brist.FindRotationAngle(Owner.Position) > 1.90f)
                {
                    value -= bristleback.GetSpecialData("back_damage_reduction") / 100f;
                }
                else if (brist.FindRotationAngle(Owner.Position) > 1.20f)
                {
                    value -= bristleback.GetSpecialData("side_damage_reduction") / 100f;
                }
            }

            // Modifier Centaur Stampede
            if (hero.HasModifier("modifier_centaur_stampede"))
            {
                var centaur = heroes.FirstOrDefault(x => x.IsEnemy(Owner) && x.HeroId == HeroId.npc_dota_hero_centaur);
                if (centaur.HasAghanimsScepter())
                {
                    var ability = centaur.GetAbilityById(AbilityId.centaur_stampede);

                    value -= ability.GetSpecialData("damage_reduction") / 100f;
                }
            }

            // Modifier Kunkka Ghostship
            if (hero.HasModifier("modifier_kunkka_ghost_ship_damage_absorb"))
            {
                var kunkka = heroes.FirstOrDefault(x => x.IsEnemy(Owner) && x.HeroId == HeroId.npc_dota_hero_kunkka);
                var ability = kunkka.GetAbilityById(AbilityId.kunkka_ghostship);

                value -= ability.GetSpecialData("ghostship_absorb") / 100f;
            }

            // Modifier Wisp Overcharge
            if (hero.HasModifier("modifier_wisp_overcharge"))
            {
                var wisp = heroes.FirstOrDefault(x => x.IsEnemy(Owner) && x.HeroId == HeroId.npc_dota_hero_wisp);
                var ability = wisp.GetAbilityById(AbilityId.wisp_overcharge);

                value -= ability.GetSpecialData("bonus_damage_pct") / 100f;
            }

            // Modifier Bloodseeker Bloodrage
            if (hero.HasModifier("modifier_bloodseeker_bloodrage") || Owner.HasModifier("modifier_bloodseeker_bloodrage"))
            {
                var bloodseeker = heroes.FirstOrDefault(x => x.HeroId == HeroId.npc_dota_hero_bloodseeker);
                var ability = bloodseeker.GetAbilityById(AbilityId.bloodseeker_bloodrage);

                value += ability.GetSpecialData("damage_increase_pct") / 100f;
            }

            // Modifier Medusa Mana Shield
            if (hero.HasModifier("modifier_medusa_mana_shield"))
            {
                var ability = hero.GetAbilityById(AbilityId.medusa_mana_shield);

                if (hero.Mana >= 50)
                {
                    value -= ability.GetSpecialData("absorption_tooltip") / 100f;
                }
            }

            // Modifier Ursa Enrage
            if (hero.HasModifier("modifier_ursa_enrage"))
            {
                var ability = hero.GetAbilityById(AbilityId.ursa_enrage);
                value -= ability.GetSpecialData("damage_reduction") / 100f;
            }

            // Modifier Chen Penitence
            if (hero.HasModifier("modifier_chen_penitence"))
            {
                var chen = heroes.FirstOrDefault(x => x.IsAlly(Owner) && x.HeroId == HeroId.npc_dota_hero_chen);
                var ability = chen.GetAbilityById(AbilityId.chen_penitence);

                value += ability.GetSpecialData("bonus_damage_taken") / 100f;
            }

            return value;
        }

        private float DamageBlock(Hero hero, List<Hero> heroes)
        {
            var value = 0.0f;

            // Modifier Hood Of Defiance Barrier
            if (hero.HasModifier("modifier_item_hood_of_defiance_barrier"))
            {
                var item = hero.GetItemById(AbilityId.item_hood_of_defiance);
                if (item != null)
                {
                    value -= item.GetSpecialData("barrier_block");
                }
            }

            // Modifier Pipe Barrier
            if (hero.HasModifier("modifier_item_pipe_barrier"))
            {
                var pipehero = heroes.FirstOrDefault(x => x.IsEnemy(Owner) && x.Inventory.Items.Any(v => v.Id == AbilityId.item_pipe));
                if (pipehero != null)
                {
                    var ability = pipehero.GetItemById(AbilityId.item_pipe);

                    value -= ability.GetSpecialData("barrier_block");
                }
            }

            // Modifier Infused Raindrop
            if (hero.HasModifier("modifier_item_infused_raindrop"))
            {
                var item = hero.GetItemById(AbilityId.item_infused_raindrop);
                if (item != null && item.Cooldown <= 0)
                {
                    value -= item.GetSpecialData("magic_damage_block");
                }
            }

            // Modifier Abaddon Aphotic Shield
            if (hero.HasModifier("modifier_abaddon_aphotic_shield"))
            {
                var abaddon = heroes.FirstOrDefault(x => x.IsEnemy(Owner) && x.HeroId == HeroId.npc_dota_hero_abaddon);
                var ability = abaddon.GetAbilityById(AbilityId.abaddon_aphotic_shield);

                value -= ability.GetSpecialData("damage_absorb");

                var talent = abaddon.GetAbilityById(AbilityId.special_bonus_unique_abaddon);
                if (talent != null && talent.Level > 0)
                {
                    value -= talent.GetSpecialData("value");
                }
            }

            // Modifier Ember Spirit Flame Guard
            if (hero.HasModifier("modifier_ember_spirit_flame_guard"))
            {
                var ability = hero.GetAbilityById(AbilityId.ember_spirit_flame_guard);
                if (ability != null)
                {
                    value -= ability.GetSpecialData("absorb_amount");

                    var emberSpirit = ability.Owner as Hero;
                    var talent = emberSpirit.GetAbilityById(AbilityId.special_bonus_unique_ember_spirit_1);
                    if (talent != null && talent.Level > 0)
                    {
                        value -= talent.GetSpecialData("value");
                    }
                }
            }

            return value;
        }

        public List<Damage> DamageList { get; } = new List<Damage>();

        public class Damage
        {
            public Hero GetHero { get; }

            public float GetDamage { get; }

            public float GetReadyDamage { get; }

            public float GetTotalDamage { get; }

            public uint GetHealth { get; }

            public Damage(Hero hero, float damage, float readyDamage, float totalDamage, uint health)
            {
                GetHero = hero;
                GetDamage = damage;
                GetReadyDamage = readyDamage;
                GetTotalDamage = totalDamage;
                GetHealth = health;
            }
        }
    }  
}
