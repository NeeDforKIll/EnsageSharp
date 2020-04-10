﻿using Ensage;

namespace TinkerFastComboPlus
{
    internal sealed class Abilities
    {
        public Abilities(Hero owner)
        {
            //Spells
            foreach (var spell in owner.Spellbook.Spells)
            {
                if (!spell.IsValid)
                {
                    continue;
                }

                switch (spell.Id)
                {
                    case AbilityId.tinker_laser:
                        {
                            Laser = spell;
                        }
                        break;

                    case AbilityId.tinker_heat_seeking_missile:
                        {
                            Rocket = spell;
                        }
                        break;

                    case AbilityId.tinker_rearm:
                        {
                            Refresh = spell;
                        }
                        break;

                    case AbilityId.tinker_march_of_the_machines:
                        {
                            March = spell;
                        }
                        break;
                }
            }

            foreach (var item in owner.Inventory.Items)
            {
                if (!item.IsValid)
                {
                    continue;
                }

                switch (item.Id)
                {
                    case AbilityId.item_blink:
                        {
                            Blink = item;
                        }
                        break;

                    case AbilityId.item_dagon:
                    case AbilityId.item_dagon_2:
                    case AbilityId.item_dagon_3:
                    case AbilityId.item_dagon_4:
                    case AbilityId.item_dagon_5:
                        {
                            Dagon = item;
                        }
                        break;

                    case AbilityId.item_rod_of_atos:
                        {
                            Atos = item;
                        }
                        break;

                    case AbilityId.item_soul_ring:
                        {
                            Soulring = item;
                        }
                        break;

                    case AbilityId.item_sheepstick:
                        {
                            Sheep = item;
                        }
                        break;

                    case AbilityId.item_ethereal_blade:
                        {
                            Ethereal = item;
                        }
                        break;

                    case AbilityId.item_shivas_guard:
                        {
                            Shiva = item;
                        }
                        break;

                    case AbilityId.item_ghost:
                        {
                            Ghost = item;
                        }
                        break;

                    case AbilityId.item_cyclone:
                        {
                            Cyclone = item;
                        }
                        break;

                    case AbilityId.item_force_staff:
                        {
                            Forcestaff = item;
                        }
                        break;

                    case AbilityId.item_glimmer_cape:
                        {
                            Glimmer = item;
                        }
                        break;

                    case AbilityId.item_bottle:
                        {
                            Bottle = item;
                        }
                        break;

                    case AbilityId.item_veil_of_discord:
                        {
                            Veil = item;
                        }
                        break;

                    case AbilityId.item_tpscroll:
                        {
                            TownPortalScroll = item;
                        }
                        break;

                    case AbilityId.item_travel_boots:
                    case AbilityId.item_travel_boots_2:
                        {
                            travel = item;
                        }
                        break;
                }
            }
        }

        public Ability Laser { get; }

        public Ability Rocket { get; }

        public Ability March { get; }

        public Ability Refresh { get; }

        public Item Blink { get; }

        public Item Dagon { get; }

        public Item Sheep { get; }

        public Item Soulring { get; }

        public Item Ethereal { get; }

        public Item Shiva { get; }

        public Item Ghost { get; }

        public Item Cyclone { get; }

        public Item Forcestaff { get; }

        public Item Glimmer { get; }

        public Item Bottle { get; }

        public Item Veil { get; }

        public Item Atos { get; }

        private readonly Item TownPortalScroll;

        private readonly Item travel;

        public Item Travel
        {
            get
            {
                if (travel != null && TownPortalScroll != null)
                {
                    return TownPortalScroll;
                }

                return null;
            }
        }
    }
}