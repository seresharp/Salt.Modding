using LootEdit.loot;
using ProjectTower.map.pickups;
using ProjectTower.player;
using Modding;

namespace ExampleMod1
{
    public class ExampleMod1 : Mod
    {
        public override void Init()
        {
            ModHooks.Instance.PickupCreated += MakePotatos;
        }

        //Turn every pickup except for key items into 100 potatos
        private void MakePotatos(Pickup pickup)
        {
            foreach (InvLoot loot in pickup.loot)
            {
                if (loot.category != LootCatalog.CATEGORY_KEYS)
                {
                    loot.SetIndex("throw_potato");
                    loot.count = 100;
                }
            }
        }

        public override string GetModName() => "Potato";
    }
}
