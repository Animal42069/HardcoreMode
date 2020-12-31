﻿using AIChara;
using AIProject;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		static public partial class Status
		{
			static readonly List<LifeStatsController> agentWarn = new List<LifeStatsController>();
			static bool healthWarn = false;
			static bool foodWarn = false;
			static bool staminaWarn = false;

			public static int Timer(ref float timer, float max)
            {
                timer += Time.deltaTime;

                if (timer < max)
                    return 0;

                int mult = (int)Mathf.Floor(timer / max);
                timer %= max;

                return mult;
            }

            static void TryWarn(LifeStatsController controller, int threshold)
			{
				float stat = controller["health"];

				if (agentWarn.Contains(controller))
				{
					if (stat > threshold)
						agentWarn.Remove(controller);
				}
				else if (stat <= threshold)
				{
					agentWarn.Add(controller);

					string name = controller.agent.CharaName;
					string preposition = stat < threshold ? "below" : "at";

					MapUIContainer.AddNotify($"{name}'s health is {preposition} {threshold}%!");
				}
			}

			static void TryWarn(string key, int threshold, ref bool flag)
			{
				float stat = playerController[key];

				if (flag)
				{
					if (stat > threshold)
						flag = false;
				}
				else if (stat <= threshold)
				{
					flag = true;
					string preposition = stat < threshold ? "below" : "at";

					MapUIContainer.AddNotify($"Your {key} is {preposition} {threshold}%!");
				}
			}

			public static void TryWarn()
			{
				TryWarn("health", HealthWarn.Value, ref healthWarn);
				TryWarn("food", FoodWarn.Value, ref foodWarn);
				TryWarn("stamina", StaminaWarn.Value, ref staminaWarn);

				int agentThreshold = AgentWarn.Value;

				foreach (var controller in agentControllers.Where(n => n != null))
					TryWarn(controller, agentThreshold);
			}

			public static void TryDelete(ChaControl chaCtrl)
			{
				string path = chaCtrl.chaFile.ConvertCharaFilePath(
					chaCtrl.chaFile.charaFileName,
					chaCtrl.sex
				);
				string directory = Path.GetDirectoryName(path);

				Manager.Map.Instance.AgentTable[0].RemoveActor(Manager.Map.Instance.AgentTable[0]);

				if (Directory.Exists(directory) && File.Exists(path))
					File.Delete(path);
			}

		}
	}
}
