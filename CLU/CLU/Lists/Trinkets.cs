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

using System.Collections.Generic;
using System.Linq;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace CLU.Lists
{
    public static class Trinkets
    {
        public static IEnumerable<WoWItem> CurrentTrinkets
        {
            get {
                return ObjectManager.GetObjectsOfType<WoWItem>().Where(trinket => TrinketIds.Contains(trinket.Entry)).ToList();
            }
        }

        private static HashSet<uint> TrinketIds
        {
            get {
                return _trinkets;
            }
        }

        private static readonly HashSet<uint> _trinkets = new HashSet<uint> {
            // ID           Name                                        Level               Source
            58183,      // Soul Casket                                  359                 Vendors
            58184,      // Core of Ripeness                             359                 Vendors
            // 59354,       // Jar of Ancient Remedies                      359                 MaloriakBlackwing Descent
            59461,      // Fury of Angerforge                           359                 Zone DropBlackwing Descent
            // 59514,       // Heart of Ignacious                           359                 Boss DropThe Bastion of Twilight
            59515,      // Vial of Stolen Memories                      359                 Boss DropThe Bastion of Twilight
            60233,      // Shard of WoeHeroic                           379                 Zone DropThe Bastion of Twilight
            61026,      // Vicious Gladiator's Emblem of Cruelty        365                 ArgalothBaradin Hold
            61030,      // Vicious Gladiator's Emblem of Proficiency    365
            61031,      // Vicious Gladiator's Emblem of Meditation     365                 ArgalothBaradin Hold
            61032,      // Vicious Gladiator's Emblem of Tenacity       365                 ArgalothBaradin Hold
            61033,      // Vicious Gladiator's Badge of Conquest        365                 ArgalothBaradin Hold
            61034,      // Vicious Gladiator's Badge of Victory         365
            61035,      // Vicious Gladiator's Badge of Dominance       365
            62048,      // Darkmoon Card: Earthquake                    359                 Darkmoon Earthquake DeckDarkmoon Faire
            // 64645,       // Tyrande's Favorite Doll                      359                 Tyrande's Favorite DollArchaeology
            // 65029,       // Jar of Ancient RemediesHeroic                372                 MaloriakBlackwing Descent
            65109,      // Vial of Stolen MemoriesHeroic                372                 Boss DropThe Bastion of Twilight
            // 65110,       // Heart of IgnaciousHeroic                     372                 Boss DropThe Bastion of Twilight
            68709,      // Unsolvable Riddle                            359
            68712,      // Impatience of Youth                          359
            68713,      // Mirror of Broken Images                      359
            68915,      // Scales of Life                               378                 Zone DropFirelands
            68926,      // Jaws of Defeat                               378                 Majordomo StaghelmFirelands
            68972,      // Apparatus of Khaz'goroth                     378                 Zone DropFirelands
            // 68996,       // Stay of Execution                            378                 Naresir Stormfury
            68998,      // Rune of Zeth                                 378                 Naresir Stormfury
            69000,      // Fiery Quintessence                           378                 Naresir Stormfury
            69001,      // Ancient Petrified Seed                       378                 Naresir Stormfury
            69002,      // Essence of the Eternal Flame                 378                 Naresir Stormfury
            69109,      // Scales of LifeHeroic                         391                 Lurah Wrathvine
            69111,      // Jaws of DefeatHeroic                         391                 Majordomo StaghelmFirelands
            69113,      // Apparatus of Khaz'gorothHeroic               391                 Lurah Wrathvine
            69762,      // Miniature Voodoo Mask                        353                 Hex Lord MalacrassZul'Aman (H)
            // 70141,       // Dwyer's Caber                                365                 Varlan HighboughVendor in Molten Front
            70142,      // Moonwell Chalice                             365                 Ayla ShadowstormVendor in Molten Front
            70143,      // Moonwell Phial                               365                 Ayla ShadowstormVendor in Molten Front
            70144,      // Ricket's Magnetic Fireball                   365                 Damek BloombeardVendor in Molten Front
            70396,      // Ruthless Gladiator's Emblem of Cruelty       384                 Drop, PvP
            70397,      // Ruthless Gladiator's Emblem of Meditation    384                 Drop, PvP
            70398,      // Ruthless Gladiator's Emblem of Tenacity      384                 Drop, PvP
            70399,      // Ruthless Gladiator's Badge of Conquest       384                 Drop, PvP
            70400,      // Ruthless Gladiator's Badge of Victory        384                 Drop, PvP
            70401,      // Ruthless Gladiator's Badge of Dominance      384                 Drop, PvP
            70517,      // Vicious Gladiator's Badge of Conquest        371                 PvP
            70518,      // Vicious Gladiator's Badge of Dominance       371                 PvP
            70519,      // Vicious Gladiator's Badge of Victory         371                 PvP
            70563,      // Vicious Gladiator's Emblem of Cruelty        371                 PvP
            70564,      // Vicious Gladiator's Emblem of Meditation     371                 PvP
            70565,      // Vicious Gladiator's Emblem of Tenacity       371                 PvP
            71333,      // Bitterer Balebrew Charm                      365                 Coren DirebrewBlackrock Depths (N)
            71334,      // Bubblier Brightbrew Charm                    365                 Coren DirebrewBlackrock Depths (N)
            71338,      // Brawler's Trophy                             365                 Coren DirebrewBlackrock Depths (N)
            72304,      // Ruthless Gladiator's Badge of Conquest       390                 PvP
            72359,      // Ruthless Gladiator's Emblem of Cruelty       390                 PvP
            72360,      // Ruthless Gladiator's Emblem of Tenacity      390                 PvP
            72361,      // Ruthless Gladiator's Emblem of Meditation    390                 PvP
            72448,      // Ruthless Gladiator's Badge of Dominance      390                 PvP
            72450,      // Ruthless Gladiator's Badge of Victory        390                 PvP
            72898,      // Foul Gift of the Demon Lord                  378                 Minor Cache of the AspectsWell of Eternity (H)
            72899,      // Varo'then's Brooch                           378                 Minor Cache of the AspectsWell of Eternity (H)
            // 72901,       // Rosary of Light                              378                 Archbishop BenedictusHour of Twilight (H)
            73496,      // Cataclysmic Gladiator's Badge of Victory     403                 Drop, PvP
            73498,      // Cataclysmic Gladiator's Badge of Dominance   403                 Drop, PvP
            73591,      // Cataclysmic Gladiator's Emblem of Meditation 403                 Drop, PvP
            73592,      // Cataclysmic Gladiator's Emblem of Tenacity   403                 Drop, PvP
            73593,      // Cataclysmic Gladiator's Emblem of Cruelty    403                 Drop, PvP
            73648,      // Cataclysmic Gladiator's Badge of Conquest    403                 Drop, PvP
            77113,      // Kiroptyric Sigil                             397                 Vendors
            77114,      // Bottled Wishes                               397                 Vendors
            77115,      // Reflection of the Light                      397                 Vendors
            77116,      // Rotting Skull                                397                 Vendors
            77117,      // Fire of the Deep                             397                 Vendors            
            // Patch 5 - MoP LEVEL 90 TRINKETS ==============================================================================================
            87576, 		// Bitterest Balebrew Charm	
            89081, 		// Blossom of Pure Snow
            86791, 		// Bottle of Infinite Stars	Elegon
            87057, 		// Bottle of Infinite Stars	Elegon
            86132, 		// Bottle of Infinite Stars	Elegon
            87571, 		// Brawler's Statue
            // 87575, 		// Bubbliest Brightbrew Charm
            84344, 		// Dreadful Gladiator's Badge of Conquest
            84488, 		// Dreadful Gladiator's Badge of Dominance
            84490, 		// Dreadful Gladiator's Badge of Victory
            84399, 		// Dreadful Gladiator's Emblem of Cruelty
            84401, 		// Dreadful Gladiator's Emblem of Meditation
            84400, 		// Dreadful Gladiator's Emblem of Tenacity
            89082, 		// Hawkmaster's Talon
            89083, 		// Iron Belly Wok
            87079, 		// Jade Bandit Figurine
            86043, 		// Jade Bandit Figurine
            86772, 		// Jade Bandit Figurine
            87082, 		// Jade Charioteer Figurine
            86042, 		// Jade Charioteer Figurine
            86771, 		// Jade Charioteer Figurine
            87081, 		// Jade Courtesan Figurine
            86045, 		// Jade Courtesan Figurine
            86774, 		// Jade Courtesan Figurine
            87080, 		// Jade Magistrate Figurine
            86044, 		// Jade Magistrate Figurine
            86773, 		// Jade Magistrate Figurine
            87083, 		// Jade Warlord Figurine
            86046, 		// Jade Warlord Figurine
            86775, 		// Jade Warlord Figurine
            89079, 		// Lao-Chin's Liquid Courage
            84934, 		// Malevolent Gladiator's Badge of Conquest
            84940, 		// Malevolent Gladiator's Badge of Dominance
            84942, 		// Malevolent Gladiator's Badge of Victory
            84936, 		// Malevolent Gladiator's Emblem of Cruelty
            84939, 		// Malevolent Gladiator's Emblem of Meditation
            84938, 		// Malevolent Gladiator's Emblem of Tenacity
            84944, 		// Malevolent Gladiator's Medallion of Cruelty - Horde -  (Removes all movement impairing effects and all effects which cause loss of control of your character.) 
            84943, 		// Malevolent Gladiator's Medallion of Cruelty - Alliance -  (Removes all movement impairing effects and all effects which cause loss of control of your character.) 
            84933, 		// Malevolent Gladiator's Medallion of Meditation - Horde -  (Removes all movement impairing effects and all effects which cause loss of control of your character.) 
            84932, 		// Malevolent Gladiator's Medallion of Meditation - Alliance -  (Removes all movement impairing effects and all effects which cause loss of control of your character.) 
            84931, 		// Malevolent Gladiator's Medallion of Tenacity - Horde -  (Removes all movement impairing effects and all effects which cause loss of control of your character.)
			84945, 		// Malevolent Gladiator's Medallion of Tenacity - Alliance -  (Removes all movement impairing effects and all effects which cause loss of control of your character.)             
            79329, 		// Relic of Niuzao	Darkmoon Ox Deck (Dodge)
            89080, 		// Scroll of Revered Ancestors (spirit)
            // 6948, // hearthstone...to initialize the list of no trinket found
            // 39769, //test
        };
    }
}