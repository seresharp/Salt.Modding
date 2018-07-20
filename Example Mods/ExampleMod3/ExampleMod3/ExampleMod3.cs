using System.Collections.Generic;
using System.Text;
using ProjectTower.player;
using Modding;

namespace ExampleMod3
{
    public class ExampleMod3 : Mod
    {
        public override void Init()
        {
            ModHooks.Instance.ClassesLoaded += AddClass;
        }

        //Add a new class
        private void AddClass(List<ClassCatalog.PortableClass> classes)
        {
            classes.Add(new ClassCatalog.PortableClass()
            {
                name = new StringBuilder("OP"),         //Name is a StringBuilder, not a string
                helm = "pumpkin_helm",                  //Ghastly Gourd because it's light and has great stats
                armor = "",                             //No chest armor that you can wear at 0 is worth having
                gloves = "blacksmith_gloves",           //Blacksmith's Gloves for +3 STR
                boots = "",                             //No boots worth having
                loadout = new string[,]
                {
                    { "sword_scissor", "", "" },        //Jaws of Death on both equip slots
                    { "sword_scissor", "", "" }
                },
                consumable = new string[]
                {
                    "buff_holy/99999",                  //Blessed pages and stained pages are OP
                    "buff_dark/99999",
                    "salt10/99999",                     //Chest of salt for leveling
                    "horn_will/99999",                  //Warhorn is really good
                    "return_bell/99999",                //Bell of return is just nice to have
                    "throw_potato/99999",               //Potato
                },
                incantation = new string[]
                {
                    "",                                 //Not gonna worry about incantations
                    "",
                    "",
                    "",
                    "",
                    ""
                },
                ring = new string[]
                {
                    "ring_damage",                      //Bloodflower Ring for +10% damage
                    "ring_stamina",                     //Mossy Ring because it stacks with warhorn
                    "ring_stam",                        //2x Brightcoral Ring because Jaws uses a million stamina
                    "ring_stam"
                },
                startingSkill = new int[]
                {
                    0,                                  //Sword level 1
                    52                                  //Sword level 2
                },
                startingLevel = 3
            });
        }

        public override string GetModName() => "OP Class";
    }
}
