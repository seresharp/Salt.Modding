using System.Collections.Generic;
using ProjectTower.player;
using SkillTreeEdit.skilltree;
using Microsoft.Xna.Framework;
using Modding;

namespace ExampleMod2
{
    //This mod simplifies the skill tree into just the skill nodes (in columns), as well as a single node for each stat
    //Who wants to deal with that huge vanilla skill tree?
    public class ExampleMod2 : Mod
    {
        public override void Init()
        {
            ModHooks.Instance.SkillTreeLoaded += ModifySkillTree;
            ModHooks.Instance.StatsUpdated += FixHealthManaPots;
        }

        //The game isn't equipped to handle health and mana potion nodes being upgraded past 1
        //We need to increment the stats for them manually to make it do anything
        private void FixHealthManaPots(PlayerStats stats)
        {
            for (int i = 0; i < SkillTree.nodes.Length; i++)
            {
                SkillNode node = SkillTree.nodes[i];
                if ((node.type == SkillNode.TYPE_HEALTH_POT || node.type == SkillNode.TYPE_MANA_POT) && stats.treeUnlocks[i] > 1)
                {
                    stats.itemClass[node.type] += stats.treeUnlocks[i] - 1;
                }
            }
        }

        private void ModifySkillTree(SkillNode[] nodes)
        {
            List<int>[] nodeCategories = new List<int>[SkillNode.TOTAL_CLASSES];

            for (int i = 0; i < nodes.Length; i++)
            {
                //Remove every node from the skill tree
                nodes[i].parent = new int[2] { -1, -1 };
                nodes[i].loc = new Vector2(float.NaN, float.NaN);

                //Split the nodes into lists based on type
                if (nodeCategories[nodes[i].type] == null)
                {
                    nodeCategories[nodes[i].type] = new List<int>();
                }

                nodeCategories[nodes[i].type].Add(i);
            }

            //Sort the types in ascending order of cost so that skill nodes are in the proper 1->2->etc order
            for (int i = 0; i < nodeCategories.Length; i++)
            {
                if (nodeCategories[i] != null)
                {
                    nodeCategories[i].Sort((x, y) => nodes[x].cost.CompareTo(nodes[y].cost));
                }
            }

            //Set up the order we want the skill columns to be in
            int[] classes = new int[] { SkillNode.TYPE_LIGHT_ARMOR_CLASS, SkillNode.TYPE_ARMOR_CLASS, SkillNode.TYPE_SHIELD_CLASS, //Armor & shields
                SkillNode.TYPE_STAFF_CLASS, SkillNode.TYPE_WAND_CLASS, SkillNode.TYPE_MAGIC_CLASS, SkillNode.TYPE_PRAYER_CLASS, //Magic
                SkillNode.TYPE_BOW_CLASS, SkillNode.TYPE_CROSSBOW_CLASS, //Ranged weapons
                SkillNode.TYPE_WHIP_CLASS, SkillNode.TYPE_POLEAXE_CLASS, SkillNode.TYPE_DAGGER_CLASS, //Light weapons
                SkillNode.TYPE_SWORD_CLASS, SkillNode.TYPE_GREATHAMMER_CLASS, SkillNode.TYPE_MACE_CLASS //Heavy weapons
            };

            for (int i = 0; i < classes.Length; i++)
            {
                int type = classes[i];
                if (nodeCategories[type] != null)
                {
                    for (int j = 0; j < nodeCategories[type].Count; j++)
                    {
                        //Create columns for the skills
                        //The skill tree coordinate system is center origin
                        int nodeIdx = nodeCategories[type][j];
                        nodes[nodeIdx].loc = new Vector2(-1050 + i * 150, (nodes[nodeIdx].cost - 1) * -150);

                        //Parent to the initial node if this is the base, otherwise the last node in this column
                        //16 is a magic number used by the game to reference the first node
                        nodes[nodeIdx].parent = new int[2] { j == 0 ? 16 : nodeCategories[type][j - 1], -1 };
                    }
                }
            }
            
            //Place the magic first node centered below the columns
            nodes[16].loc = new Vector2(0, 300);
            nodes[16].max = int.MaxValue;

            //Place a mana pot node below the health pot node
            SkillNode manaNode = nodes[nodeCategories[SkillNode.TYPE_MANA_POT][0]];
            manaNode.loc = new Vector2(0, 600);
            manaNode.max = int.MaxValue;
            manaNode.parent = new int[2] { 16, -1 };

            //Place nodes for the 6 stats to the left and right of that
            SkillNode magNode = nodes[nodeCategories[SkillNode.TYPE_MAG][0]];
            magNode.loc = new Vector2(-450, 600);
            magNode.max = int.MaxValue;
            magNode.parent = new int[2] { 16, -1 };

            SkillNode wisNode = nodes[nodeCategories[SkillNode.TYPE_WIS][0]];
            wisNode.loc = new Vector2(-300, 600);
            wisNode.max = int.MaxValue;
            wisNode.parent = new int[2] { 16, -1 };

            SkillNode willNode = nodes[nodeCategories[SkillNode.TYPE_WILL][0]];
            willNode.loc = new Vector2(-150, 600);
            willNode.max = int.MaxValue;
            willNode.parent = new int[2] { 16, -1 };

            SkillNode strNode = nodes[nodeCategories[SkillNode.TYPE_STR][0]];
            strNode.loc = new Vector2(150, 600);
            strNode.max = int.MaxValue;
            strNode.parent = new int[2] { 16, -1 };

            SkillNode dexNode = nodes[nodeCategories[SkillNode.TYPE_DEX][0]];
            dexNode.loc = new Vector2(300, 600);
            dexNode.max = int.MaxValue;
            dexNode.parent = new int[2] { 16, -1 };

            SkillNode endNode = nodes[nodeCategories[SkillNode.TYPE_END][0]];
            endNode.loc = new Vector2(450, 600);
            endNode.max = int.MaxValue;
            endNode.parent = new int[2] { 16, -1 };
        }

        public override string GetModName() => "Simple Skill Tree";
    }
}
