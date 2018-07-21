using System;
using System.Collections.Generic;
using LootEdit.loot;
using ProjectTower.character;
using ProjectTower.menu.levels;
using ProjectTower.player;
using MonoMod;
using Modding.Attributes;

using CharMgr = Modding.Patches.ProjectTower.character.CharMgr;
using Player = Modding.Patches.ProjectTower.player.Player;

namespace Modding.Patches.ProjectTower.gamestate
{
    [MonoModPatch("ProjectTower.gamestate.NewGameManager")]
    public class NewGameManager
    {
        [MonoModIgnore]
        [RemoveBetween(new string[] { "callvirt" }, 
            new object[] { "System.Void ProjectTower.player.challenges.PlayerChallenges::WriteFlags()" }, 
            new string[] { "newobj" }, 
            new object[] { "System.Void ProjectTower.player.InvLoot::.ctor()" })]
        [MiddleHook(new string[] { "callvirt" },
            new object[] { "System.Void ProjectTower.player.challenges.PlayerChallenges::WriteFlags()" }, 
            "GiveStartingItems")]
        public static void NewGame(Player p) { }

        public static void GiveStartingItems(Player p)
        {
            Character character = CharMgr.GetPlayerCharacter(p);
            CreateCharacter createCharacter = (CreateCharacter)p.menuMgr.menuLevel[3];

            ClassCatalog.PortableClass startClass = ClassCatalog.startingClass[createCharacter.startingClass];

            List<string> items = new List<string>();
            items.AddRange(startClass.consumable);
            items.AddRange(startClass.ring);
            items.AddRange(startClass.incantation);

            if (SuppliesCatalog.supplies[createCharacter.startingSupplies].loot != null)
            {
                items.AddRange(SuppliesCatalog.supplies[createCharacter.startingSupplies].loot);
            }

            int consumableSlot = 0;
            int ringSlot = 0;
            int incantationSlot = 0;

            foreach (string item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string name = item;
                    int count = 1;

                    int slashIdx = item.IndexOf('/');
                    if (slashIdx != -1)
                    {
                        name = item.Substring(0, slashIdx);
                        count = Convert.ToInt32(item.Substring(slashIdx + 1));
                    }

                    InvLoot loot = new InvLoot();
                    loot.InitFromName(name);
                    int invIdx = p.playerInv.Add(loot, false, count);

                    if (loot.category == LootCatalog.CATEGORY_CONSUMABLE)
                    {
                        if (consumableSlot < character.equipment.consumable.Length)
                        {
                            character.equipment.consumable[consumableSlot].SetFromInventory(invIdx, character);
                            consumableSlot++;
                        }
                    }
                    else if (loot.category == LootCatalog.CATEGORY_RING)
                    {
                        if (ringSlot < character.equipment.ring.Length)
                        {
                            character.equipment.ring[ringSlot].SetFromInventory(invIdx, character);
                            ringSlot++;
                        }
                    }
                    else if (loot.category == LootCatalog.CATEGORY_MAGIC)
                    {
                        if (incantationSlot < character.equipment.incantation.Length)
                        {
                            character.equipment.consumable[incantationSlot].SetFromInventory(invIdx, character);
                            incantationSlot++;
                        }
                    }
                }
            }
        }
    }
}
