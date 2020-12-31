using AIProject;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		public static void AddController(LifeStatsController controller)
		{
			if (!agentControllers.Contains(controller))
				controllersQueue.Add(controller);
		}

		public static void RemoveController(LifeStatsController controller)
		{
			if (agentControllers.Contains(controller))
				agentControllersDump.Add(controller);
		}

		public static void UpdateControllers()
		{
			if (playerController?.ChaControl == null)
				playerController = null;

			if (!Map.IsInstance() ||
				Map.Instance.Player == null ||
				Map.Instance.AgentTable == null)
				return;

			bool updateHUDs = false;
			foreach (LifeStatsController controller in controllersQueue)
			{
				if (Map.Instance.Player.ChaControl == controller.ChaControl)
				{
					playerController = controller;
					playerController.statusHUD.SetScale(new Vector3(1.25f, 1f, 1f));
					playerController.statusHUD.SetVisibility(Status.visible, PlayerDeath.Value, PlayerLife.Value);
					continue;
				}

				foreach (KeyValuePair<int, AgentActor> agent in Map.Instance.AgentTable)
				{
					if (agent.Value.ChaControl == controller.ChaControl)
					{
						controller.agent = agent.Value;
						updateHUDs = true;
						agentControllers.Add(controller);
						break;
					}
				}
			}

			controllersQueue.Clear();

			foreach (LifeStatsController controller in agentControllersDump)
			{
				updateHUDs = true;
				agentControllers.Remove(controller);
			}

			agentControllersDump.Clear();

			if (!updateHUDs)
				return;

			int hud = 0;
			foreach (var controller in agentControllers.Where(n => n != null))
				controller.statusHUD.SetPosition(new Vector3(-950, 450 - 65 * hud++, 0));
		}
	}
}
