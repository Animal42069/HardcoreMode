using AIProject;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		static partial class Status
		{
			public static bool visible = true;

			static Text playerStomach;
			static Text playerFatigue;
			static Image playerHealthBar;
			static Image agentHealthBar;
			static GameObject playerStomachObject;
			static GameObject playerFatigueObject;
			static GameObject playerHealthObject;
			static GameObject agentHealthObject;

			static bool initialized = false;
			static AgentActor selectedAgent;

			public static void Initialize()
			{
				Console.WriteLine("Hardcore Initialize");

				var statusGuage = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/PlayerContent/Guage");
				var playerContentUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/PlayerContent");
				var agentContentUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/AgentContent");

				if (playerContentUI == null || agentContentUI == null || statusGuage == null)
					return;

				playerContentUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("SexLabel")).FirstOrDefault().localPosition = new Vector3(-270, -440, 0);
				playerContentUI.GetComponentsInChildren<Text>(true).Where(x => x.text.Contains("Gender")).FirstOrDefault().rectTransform.localPosition = new Vector3(-290, -390, 0);
				playerContentUI.GetComponentsInChildren<Text>(true).Where(x => x.text.Contains("Gender")).FirstOrDefault().rectTransform.sizeDelta = new Vector3(120, 50, 0);

				agentContentUI.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Motivation")).FirstOrDefault().GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("Text")).FirstOrDefault().rectTransform.sizeDelta = new Vector3(120, 40, 0);
				agentContentUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("Sick")).FirstOrDefault().localPosition = new Vector3(-290, -390, 0);	
				agentContentUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("SickIcon")).FirstOrDefault().localPosition = new Vector3(240, -120, 0);

				if (playerStomachObject == null)
				{
					var hungerUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/AgentContent/Hunger");
					hungerUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("Text")).FirstOrDefault().offsetMax = new Vector2(100, 0);
					playerStomachObject = Instantiate(hungerUI);
					playerStomachObject.transform.SetParent(playerContentUI.transform);
					playerStomachObject.transform.localPosition = new Vector3(140, 0, 0);
					playerStomachObject.transform.localScale = new Vector3(1, 1, 1);
				}
				playerStomach = playerStomachObject.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("Hungerlabel")).FirstOrDefault();

				if (playerFatigueObject == null)
				{
					var staminaUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/AgentContent/Physical");
					playerFatigueObject = Instantiate(staminaUI);
					playerFatigueObject.transform.SetParent(playerContentUI.transform);
					playerFatigueObject.transform.localPosition = new Vector3(-40, 0, 0);
					playerFatigueObject.transform.localScale = new Vector3(1, 1, 1);
				}
				playerFatigue = playerFatigueObject.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("PhysicalLabel")).FirstOrDefault();

				playerHealthObject = Instantiate(statusGuage);
				playerHealthObject.transform.SetParent(playerContentUI.transform);
				playerHealthObject.name = "PlayerHealth";
				playerHealthObject.transform.localPosition = new Vector3(-310f, -370f, 0);
				playerHealthObject.transform.localScale = new Vector3(1, 1, 1);
				playerHealthObject.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().localScale = new Vector3(1f, 1.1f, 1f);
				playerHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().color = new Color(1, 1, 1, 1);
				playerHealthObject.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("PlayerHealth")).FirstOrDefault().sizeDelta = new Vector2(254, 25);
				playerHealthBar = playerHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("PlayerHealth")).FirstOrDefault();
				playerHealthBar.color = new Color(1, 0, 0, 1f);

				agentHealthObject = Instantiate(statusGuage);
				agentHealthObject.transform.SetParent(agentContentUI.transform);
				agentHealthObject.name = "AgentHealth";
				agentHealthObject.transform.localPosition = new Vector3(-310f, -370f, 0);
				agentHealthObject.transform.localScale = new Vector3(1, 1, 1);
				agentHealthObject.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().localScale = new Vector3(1f, 1.1f, 1f);
				agentHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().color = new Color(1, 1, 1, 1);
				agentHealthObject.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("AgentHealth")).FirstOrDefault().sizeDelta = new Vector2(254, 25);
				agentHealthBar = agentHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("AgentHealth")).FirstOrDefault();
				agentHealthBar.color = new Color(1, 0, 0, 1f);

				initialized = true;

				UpdateCellPhoneVisibility(PlayerDeath.Value, PlayerLife.Value, AgentDeath.Value);

				Console.WriteLine($"Hardcore Initialized");
			}

			public static void UpdateCellPhoneVisibility(bool playerHealth, bool playerStats, bool agentHealth)
            {
				if (!initialized)
					return;

				playerHealthObject.SetActive(playerHealth);
				playerFatigueObject.SetActive(playerStats);
				playerStomachObject.SetActive(playerStats);
				agentHealthObject.SetActive(agentHealth);
			}

			static void UpdatePlayerHUD()
            {
				if (!initialized)
					return;

				playerHealthBar.fillAmount = playerController["health"] / 100;
				playerStomach.text = $"{playerController["food"]:F0}";
				playerFatigue.text = $"{(100 - playerController["stamina"]):F0}";

				if (playerController != null && playerController.statusHUD != null)
					playerController.statusHUD.Update(playerController["health"], playerController["stamina"], playerController["food"]);
			}

			static void UpdateAgentHUDs()
			{
				if (!initialized)
					return;

				foreach (var controller in agentControllers.Where(n => n != null))
				{
					if (controller.statusHUD != null)
						controller.statusHUD.Update(controller["health"], 
							controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Physical], 
							controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger]);

					if (selectedAgent != null && selectedAgent.ChaControl == controller.ChaControl)
						agentHealthBar.fillAmount = controller["health"] / 100;
				}
			}

			public static void UpdateSelectedAgent(int selectionID)
			{
				Singleton<Manager.Map>.Instance.AgentTable.TryGetValue(selectionID, out selectedAgent);
			}
		}
	}
}
