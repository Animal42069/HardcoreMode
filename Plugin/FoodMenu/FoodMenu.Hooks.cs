using HarmonyLib;
using AIProject.UI;
using AIProject.SaveData;
using System;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemInfoRemoveUI), "Refresh", typeof(StuffItem))]
        public static void ItemInfoUIRefresh(StuffItem item)
        {
            FoodMenu.SetCurrentItem(item);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemSendPanelUI), "Refresh", typeof(StuffItem))]
        public static void ItemSendPanelUIRefresh(StuffItem item)
        {
            FoodMenu.SetCurrentItem(item);
        }


    }
}
