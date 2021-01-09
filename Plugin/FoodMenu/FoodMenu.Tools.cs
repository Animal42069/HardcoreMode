using AIProject.SaveData;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        static partial class FoodMenu
        {

            private enum ItemCategory
            {
                Plants = 1,
                Fruits = 2,
                Drinks = 6,
                Food = 7
            }

            private enum PlantItem
            {
                QuailShroom = 4,
                BitterMelon = 5,
                BehemothBitter = 8
            }

            private enum FruitItem
            {
                RedFruit = 0,
                Coconut = 3
            }

            private enum FoodItem
            {
                Egg,
                BoiledEgg,
                RoastedFish,
                FriedPotato,
                CoconutSoup,
                BananatatoSalad,
                StirFry,
                CookedCup,
                Omelet,
                CrabCreamStew,
                FishSaute,
                RichBouillabaisse,
                DarkMatter
            };

            private enum DrinkItem
            {
                RiverWater,
                DrinkingWater,
                HotWater,
                GarnetJuice,
                HotCoffee,
                CoconutLiquor
            };

            private static float GetCalories(StuffItem item)
            {
                if ((ItemCategory)item.CategoryID == ItemCategory.Plants)
                {
                    switch ((PlantItem)item.ID)
                    {
                        case PlantItem.QuailShroom: return 10f;
                        case PlantItem.BitterMelon: return 25f;
                        case PlantItem.BehemothBitter: return 40f;
                        default: return 0f;
                    }
                }

                if ((ItemCategory)item.CategoryID == ItemCategory.Fruits)
                {
                    switch ((FruitItem)item.ID)
                    {
                        case FruitItem.RedFruit: return 90f;
                        case FruitItem.Coconut: return 1000f;
                        default: return 0f;
                    }
                }

                if ((ItemCategory)item.CategoryID == ItemCategory.Food)
                {
                    switch ((FoodItem)item.ID)
                    {
                        case FoodItem.Egg: return -50f;
                        case FoodItem.BoiledEgg: return 80f;
                        case FoodItem.RoastedFish: return 250f;
                        case FoodItem.FriedPotato: return 250f;
                        case FoodItem.BananatatoSalad: return 150f;
                        case FoodItem.CoconutSoup: return 1500f;
                        case FoodItem.StirFry: return 225f;
                        case FoodItem.CookedCup: return 250f;
                        case FoodItem.Omelet: return 200f;
                        case FoodItem.CrabCreamStew: return 2000f;
                        case FoodItem.FishSaute: return 800f;
                        case FoodItem.RichBouillabaisse: return 1250f;
                        case FoodItem.DarkMatter: return -200f;
                        default: return 0f;
                    }
                }       

                if ((ItemCategory)item.CategoryID == ItemCategory.Drinks)
                {
                    switch ((DrinkItem)item.ID)
                    {
                        case DrinkItem.RiverWater: return -200f;
                        case DrinkItem.GarnetJuice: return 100f;
                        case DrinkItem.HotCoffee: return -150f;
                        case DrinkItem.CoconutLiquor: return 1000f;
                        default: return 0f;
                    }
                }

                return 0f;
            }

            private static float GetWaterContent(StuffItem item)
            {
                if ((ItemCategory)item.CategoryID == ItemCategory.Plants)
                {
                    switch ((PlantItem)item.ID)
                    {
                        case PlantItem.QuailShroom: return 10f;
                        case PlantItem.BitterMelon: return 200f;
                        case PlantItem.BehemothBitter: return 400f;
                        default: return 0f;
                    }
                }

                if ((ItemCategory)item.CategoryID == ItemCategory.Fruits)
                {
                    switch ((FruitItem)item.ID)
                    {
                        case FruitItem.RedFruit: return 100f;
                        case FruitItem.Coconut: return 300f;
                        default: return 0f;
                    }
                }

                if ((ItemCategory)item.CategoryID == ItemCategory.Food)
                {
                    switch ((FoodItem)item.ID)
                    {
                        case FoodItem.Egg: return 50f;
                        case FoodItem.BoiledEgg: return 50f;
                        case FoodItem.RoastedFish: return 50f;
                        case FoodItem.FriedPotato: return 100f;
                        case FoodItem.BananatatoSalad: return 150f;
                        case FoodItem.CoconutSoup: return 500f;
                        case FoodItem.StirFry: return 200f;
                        case FoodItem.CookedCup: return 250f;
                        case FoodItem.Omelet: return 50f;
                        case FoodItem.CrabCreamStew: return 400f;
                        case FoodItem.FishSaute: return 200f;
                        case FoodItem.RichBouillabaisse: return 400f;
                        case FoodItem.DarkMatter: return -100f;
                        default: return 0f;
                    }
                }

                if ((ItemCategory)item.CategoryID == ItemCategory.Drinks)
                {
                    switch ((DrinkItem)item.ID)
                    {
                        case DrinkItem.RiverWater: return 150f;
                        case DrinkItem.DrinkingWater: return 250f;
                        case DrinkItem.HotWater: return 250f;
                        case DrinkItem.GarnetJuice: return 400f;
                        case DrinkItem.HotCoffee: return 250f;
                        case DrinkItem.CoconutLiquor: return 350f;
                        default: return 0f;
                    }
                }

                return 0f;
            }

            private static float GetStaminaModifier(StuffItem item)
            {
                if ((ItemCategory)item.CategoryID == ItemCategory.Food
                    && (FoodItem)item.ID == FoodItem.StirFry)
                    return 2.5f;

                if ((ItemCategory)item.CategoryID == ItemCategory.Drinks)
                {
                    switch ((DrinkItem)item.ID)
                    {
                        case DrinkItem.GarnetJuice: return 2.5f;
                        case DrinkItem.HotCoffee: return 10f;
                        case DrinkItem.CoconutLiquor: return -15f;
                        default: return 0f;
                    }
                }

                return 0f;
            }

            private static bool IsFoodItem(StuffItem item)
            {
                if ((ItemCategory)item.CategoryID == ItemCategory.Food)
                    return true;

                if ((ItemCategory)item.CategoryID == ItemCategory.Plants)
                {
                    switch ((PlantItem)item.ID)
                    {
                        case PlantItem.QuailShroom: return true;
                        case PlantItem.BitterMelon: return true;
                        case PlantItem.BehemothBitter: return true;
                        default: return false;
                    }
                }

                if ((ItemCategory)item.CategoryID == ItemCategory.Fruits)
                {
                    switch ((FruitItem)item.ID)
                    {
                        case FruitItem.RedFruit: return true;
                        case FruitItem.Coconut: return true;
                        default: return false;
                    }
                }

                return false;
            }

            private static bool IsDrinkItem(StuffItem item)
            {
                return ((ItemCategory)item.CategoryID == ItemCategory.Drinks);
            }

            public static GameObject CreateConsumeButton(string parentLocation, Vector3 position, Vector2 sideDelta, UnityAction buttonCall, string buttonIcon)
            {
                var removeButton = GameObject.Find(parentLocation + "/RemoveButton");
                if (removeButton == null)
                    return null;
           
                GameObject buttonObject = Instantiate(removeButton);
                buttonObject.name = "ConsumeButton";
                buttonObject.transform.SetParent(removeButton.transform.parent);
                buttonObject.transform.localPosition = position;
                buttonObject.transform.localScale = new Vector3(1, 1, 1);
                buttonObject.GetComponentInChildren<RectTransform>(true).sizeDelta = sideDelta;
                var consumeButton = buttonObject.GetComponentInChildren<Button>(true);

                ColorBlock buttonColorBlock = consumeButton.colors;
                buttonColorBlock.highlightedColor = new Color(0.35f, 0.75f, 0.25f, 1);
                consumeButton.colors = buttonColorBlock;
                consumeButton.transition = Selectable.Transition.ColorTint;

                buttonObject.GetComponentInChildren<Image>(true).sprite =
                    GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/InventoryUI(Clone)/InventoryViewer(Clone)/CategoryUI/CategoryLayout/Category/Viewport/Content")
                    .GetComponentsInChildren<Image>(true).Where(x => x.sprite != null && x.sprite.name.Contains(buttonIcon)).FirstOrDefault()?.sprite;

                consumeButton.onClick.RemoveAllListeners();
                consumeButton.onClick.AddListener(buttonCall);

                buttonObject.SetActive(false);
                return buttonObject;
            }
        }
    }
}
