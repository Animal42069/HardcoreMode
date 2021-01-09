using AIProject.UI;
using AIProject.SaveData;
using Manager;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        static partial class FoodMenu
        {
            static StuffItem currentItem;
            static GameObject inventoryEatObject;
            static GameObject storageEatObject;
            static GameObject refrigeratorEatObject;
            static GameObject inventoryDrinkObject;
            static GameObject storageDrinkObject;
            static GameObject refrigeratorDrinkObject;
            public static void Initialize()
            {
                if (inventoryEatObject == null)
                    inventoryEatObject = CreateConsumeButton("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/InventoryUI(Clone)/InfoPanel/Infomation/InfoLayout",
                        new Vector3(-135, 40, 0), new Vector2(65, -20), ConsumeFromInventoryUI, "sp_ai_category_00_07");

                if (storageEatObject == null)
                    storageEatObject = CreateConsumeButton("MapScene/MapUI(Clone)/CommandCanvas/ItemBoxUI(Clone)/Top/Panel/SendPanel/Infomation/Send",
                    new Vector3(-165, 0, 0), new Vector2(60, -15), ConsumeFromItemBoxUI, "sp_ai_category_00_07");

                if (refrigeratorEatObject == null)
                    refrigeratorEatObject = CreateConsumeButton("MapScene/MapUI(Clone)/CommandCanvas/RefrigeratorUI(Clone)/Top/Panel/SendPanel/Infomation/Send",
                    new Vector3(-165, 0, 0), new Vector2(60, -15), ConsumeFromRefrigeratorUI, "sp_ai_category_00_07");

                if (inventoryDrinkObject == null)
                    inventoryDrinkObject = CreateConsumeButton("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/InventoryUI(Clone)/InfoPanel/Infomation/InfoLayout",
                    new Vector3(-135, 40, 0), new Vector2(65, -20), ConsumeFromInventoryUI, "sp_ai_category_00_06");

                if (storageDrinkObject == null)
                    storageDrinkObject = CreateConsumeButton("MapScene/MapUI(Clone)/CommandCanvas/ItemBoxUI(Clone)/Top/Panel/SendPanel/Infomation/Send",
                    new Vector3(-165, 0, 0), new Vector2(60, -15), ConsumeFromItemBoxUI, "sp_ai_category_00_06");

                if (refrigeratorDrinkObject == null)
                    refrigeratorDrinkObject = CreateConsumeButton("MapScene/MapUI(Clone)/CommandCanvas/RefrigeratorUI(Clone)/Top/Panel/SendPanel/Infomation/Send",
                    new Vector3(-165, 0, 0), new Vector2(60, -15), ConsumeFromRefrigeratorUI, "sp_ai_category_00_06");
            }

            static void ConsumeFromInventoryUI()
            {
                if (currentItem == null || currentItem.Count <= 0)
                    return;

                int itemCount = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/InventoryUI(Clone)/InfoPanel").GetComponent<ItemInfoRemoveUI>().Count;    
                ItemListUI itemUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/InventoryUI(Clone)/InventoryViewer(Clone)/ListPanel").GetComponent<ItemListUI>();
                List<StuffItem> itemList = Singleton<Game>.Instance.WorldData.PlayerData.ItemList;

                ConsumeItem(ref currentItem, itemCount, ref itemUI, ref itemList);
            }

            static void ConsumeFromItemBoxUI()
            {
                if (currentItem == null || currentItem.Count <= 0)
                    return;

                ItemListUI itemUI;
                List<StuffItem> itemList;
                int itemCount = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/ItemBoxUI(Clone)/Top/Panel/SendPanel").GetComponent<ItemSendPanelUI>().Count;
                bool fromBox = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/ItemBoxUI(Clone)/Top/Panel/SendPanel").GetComponent<ItemSendPanelUI>().takeout;

                if (fromBox)
                {
                    itemUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/ItemBoxUI(Clone)/Top/Panel/ItemBoxPanel/Layout/InventoryViewer(Clone)/ListPanel").GetComponent<ItemListUI>();
                    itemList = Singleton<Game>.Instance.WorldData.Environment.ItemListInStorage;
                }
                else
                {
                    itemUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/ItemBoxUI(Clone)/Top/Panel/InventoryPanel/Layout/InventoryViewer(Clone)/ListPanel").GetComponent<ItemListUI>();
                    itemList = Singleton<Game>.Instance.WorldData.PlayerData.ItemList;
                }

                ConsumeItem(ref currentItem, itemCount, ref itemUI, ref itemList);
            }

            static void ConsumeFromRefrigeratorUI()
            {
                if (currentItem == null || currentItem.Count <= 0)
                    return;

                ItemListUI itemUI;
                List<StuffItem> itemList;
                int itemCount = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/RefrigeratorUI(Clone)/Top/Panel/SendPanel").GetComponent<ItemSendPanelUI>().Count;
                bool fromBox = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/RefrigeratorUI(Clone)/Top/Panel/SendPanel").GetComponent<ItemSendPanelUI>().takeout;

                if (fromBox)
                {
                    itemUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/RefrigeratorUI(Clone)/Top/Panel/ItemBoxPanel/Layout/InventoryViewer(Clone)/ListPanel").GetComponent<ItemListUI>();
                    itemList = Singleton<Game>.Instance.WorldData.Environment.ItemListInPantry;
                }
                else
                {
                    itemUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/RefrigeratorUI(Clone)/Top/Panel/InventoryPanel/Layout/InventoryViewer(Clone)/ListPanel").GetComponent<ItemListUI>();
                    itemList = Singleton<Game>.Instance.WorldData.PlayerData.ItemList;
                }

                ConsumeItem(ref currentItem, itemCount, ref itemUI, ref itemList);
            }

            static void ConsumeItem(ref StuffItem itemToConsume, int amountToConsume, ref ItemListUI itemUI, ref List<StuffItem> itemList)
            {
                if (amountToConsume <= 0 || itemUI == null)
                    return;

                float caloriesPerItem = GetCalories(itemToConsume) * CalorieRate.Value / CaloriePool.Value;
                float waterPerItem = GetWaterContent(itemToConsume) * WaterRate.Value / WaterPool.Value;
                if (caloriesPerItem > 0)
                {
                    int maxAmountToConsume = 1 + (int)((100 - playerController["food"]) / caloriesPerItem);
                    if (amountToConsume > maxAmountToConsume)
                        amountToConsume = maxAmountToConsume;
                }
                else if (waterPerItem > 0)
                {
                    int maxAmountToConsume = 1 + (int)((100 - playerController["water"]) / waterPerItem);
                    if (amountToConsume > maxAmountToConsume)
                        amountToConsume = maxAmountToConsume;
                }

                if (amountToConsume > itemToConsume.Count)
                    amountToConsume = itemToConsume.Count;

                playerController["food"] += amountToConsume * caloriesPerItem;
                playerController["water"] += amountToConsume * waterPerItem;
                playerController["stamina"] += amountToConsume * GetStaminaModifier(itemToConsume);

                itemToConsume.Count -= amountToConsume;
                if (itemToConsume.Count <= 0)
                {
                    if (itemList != null && itemList.Contains(itemToConsume))
                        itemList.Remove(itemToConsume);

                    itemUI.RemoveItemNode(itemUI.CurrentID);
                }

                itemUI.Refresh();
            }

            public static void SetCurrentItem(StuffItem Item)
            {
                currentItem = Item;
                inventoryEatObject.SetActive(IsFoodItem(Item));
                storageEatObject.SetActive(IsFoodItem(Item));
                refrigeratorEatObject.SetActive(IsFoodItem(Item));
                inventoryDrinkObject.SetActive(IsDrinkItem(Item));
                storageDrinkObject.SetActive(IsDrinkItem(Item));
                refrigeratorDrinkObject.SetActive(IsDrinkItem(Item));
            }         
        }
    }
}
