using AIProject;
using AIProject.SaveData;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		static partial class FoodMenu
		{
			public static IOrderedEnumerable<Tuple<StuffItem, string, int, float>> GetEdible()
			{
				if (!Map.IsInstance() || Map.Instance.Player == null)
					return null;

				Manager.Resources resources = Manager.Resources.Instance;
				Manager.Resources.GameInfoTables gameInfo = resources.GameInfo;
				List<Tuple<StuffItem, string, int, float>> foods = new List<Tuple<StuffItem, string, int, float>>();
				Dictionary<int, Dictionary<int, Dictionary<int, FoodParameterPacket>>> foodParamTable =
					resources.GameInfo.FoodParameterTable;

				foreach (StuffItem food in Map.Instance.Player.PlayerData.ItemList)
					if (foodParamTable.TryGetValue(
							food.CategoryID,
							out Dictionary<int, Dictionary<int, FoodParameterPacket>> paramTable
						) &&
						paramTable.TryGetValue(food.ID, out Dictionary<int, FoodParameterPacket> _))
					{
						StuffItemInfo info = gameInfo.GetItem(food.CategoryID, food.ID);

						foods.Add(new Tuple<StuffItem, string, int, float>(
							food,
							EnglishNames.Value ? GetEnglishFoodName(food.CategoryID, food.ID) : info.Name,
							info.Rate,
							GetCalorieRate(food.CategoryID, food.ID) / 65f
						));
					}

				return foods.OrderByDescending(v => v.Item4);
			}

			public static float GetFoodRecovered(int rate)
			{
				return Mathf.Clamp(rate * FoodRate.Value / 100, FoodMinRate.Value, 100);
			}

			private static string GetEnglishFoodName(int categoryID, int infoID)
			{
				if (categoryID == 2 && infoID == 0)
					return "Red Fruit";

				if (categoryID == 7)
                {
					switch (infoID)
                    {
						case 0: return "Egg";
						case 1: return "Boiled Egg";
						case 2: return "Roasted Fish";
						case 3: return "Fried Potato";
						case 4: return "Bananatato Salad";
						case 5: return "Coconut Soup";
						case 6: return "Stir-Fry";
						case 7: return "Cooked Cup";
						case 8: return "Omelet";
						case 9: return "Crab Cream Stew";
						case 10: return "Fish Sauté";
						case 11: return "Rich Bouillabaisse";
						case 12: return "Dark Matter";
					}
				}

				return "No Translation";
			}

			private static float GetCalorieRate(int categoryID, int infoID)
			{
				if (categoryID == 2 && infoID == 0)
					return 100f;

				if (categoryID == 7)
				{
					switch (infoID)
					{
						case 0: return 80f;
						case 1: return 80f;
						case 2: return 225f;
						case 3: return 250f;
						case 4: return 150f;
						case 5: return 1630f;
						case 6: return 220f;
						case 7: return 200f;
						case 8: return 200f;
						case 9: return 2150f;
						case 10: return 500f;
						case 11: return 770f;
						case 12: return 20f;
					}
				}

				return 0f;
			}
		}
	}
}
