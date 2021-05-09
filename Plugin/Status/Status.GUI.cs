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
            public static bool visibileHUD = true;

            static Text agentThirst;
            static Text playerStomach;
            static Text playerFatigue;
            static Text playerThirst;
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
                var statusGuage = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/PlayerContent/Guage");
                var playerContentUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/PlayerContent");
                var agentContentUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/AgentContent");

                if (playerContentUI == null || agentContentUI == null || statusGuage == null)
                    return;
                try
                {
                    playerContentUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("SexLabel")).FirstOrDefault().localPosition = new Vector3(-270, -440, 0);
                    var genderTransform = playerContentUI.GetComponentsInChildren<Transform>(true).Where(x => x.localPosition.x < -30 && x.localPosition.y > -20).FirstOrDefault();
                    if (genderTransform != null)
                    {
                        genderTransform.GetComponent<RectTransform>().localPosition = new Vector3(-290, -390, 0);
                        genderTransform.GetComponent<RectTransform>().sizeDelta = new Vector3(120, 50, 0);
                    }
                    agentContentUI.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Motivation")).FirstOrDefault().GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("Text")).FirstOrDefault().rectTransform.sizeDelta = new Vector3(120, 40, 0);
                    agentContentUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("Sick")).FirstOrDefault().localPosition = new Vector3(-290, -390, 0);
                    agentContentUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("SickIcon")).FirstOrDefault().localPosition = new Vector3(240, -120, 0);

                    if (playerStomachObject == null)
                    {
                        var hungerUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/AgentContent/Hunger");
                        hungerUI.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("Text")).FirstOrDefault().offsetMax = new Vector2(100, 0);
                        var hungerLabel = hungerUI.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("Hungerlabel")).FirstOrDefault();
                        hungerLabel.transform.localScale = new Vector3(1, 0.75f, 1);
                        agentThirst = GameObject.Instantiate(hungerLabel);
                        agentThirst.name = "ThirstLabel";
                        agentThirst.transform.SetParent(hungerLabel.transform.parent);
                        agentThirst.transform.localScale = new Vector3(1, 0.75f, 1);
                        agentThirst.transform.localPosition = new Vector3(148, -70, 0);
                        agentThirst.color = new Color(0, 0.75f, 1, 1);
                        hungerLabel.color = new Color(0, 1, 0, 1);
                        var per = hungerUI.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("Per")).FirstOrDefault();
                        var thirstPer = GameObject.Instantiate(per);
                        thirstPer.name = "ThirstPer";
                        thirstPer.transform.SetParent(per.transform.parent);
                        thirstPer.transform.localScale = new Vector3(1, 1, 1);
                        thirstPer.transform.localPosition = new Vector3(150, -6, 0);
                        thirstPer.color = new Color(0, 0.75f, 1, 1);
                        per.color = new Color(0, 1, 0, 1);

                        playerStomachObject = Instantiate(hungerUI);
                        playerStomachObject.transform.SetParent(playerContentUI.transform);
                        playerStomachObject.transform.localPosition = new Vector3(140, 0, 0);
                        playerStomachObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                    playerStomach = playerStomachObject.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("Hungerlabel")).FirstOrDefault();
                    playerThirst = playerStomachObject.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("ThirstLabel")).FirstOrDefault();
                    var playerThirstPer = playerStomachObject.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("ThirstPer")).FirstOrDefault();
                    playerThirstPer.transform.localScale = new Vector3(1, 1, 1);
                    playerThirstPer.transform.localPosition = new Vector3(150, -6, 0);
                    playerThirstPer.color = new Color(0, 0.75f, 1, 1);

                    if (playerFatigueObject == null)
                    {
                        var staminaUI = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/AgentContent/Physical");
                        playerFatigueObject = Instantiate(staminaUI);
                        playerFatigueObject.transform.SetParent(playerContentUI.transform);
                        playerFatigueObject.transform.localPosition = new Vector3(-40, 0, 0);
                        playerFatigueObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                    playerFatigue = playerFatigueObject.GetComponentsInChildren<Text>(true).Where(x => x.name.Contains("PhysicalLabel")).FirstOrDefault();

                    if (playerHealthObject == null)
                    {
                        playerHealthObject = Instantiate(statusGuage);
                        playerHealthObject.transform.SetParent(playerContentUI.transform);
                        playerHealthObject.name = "PlayerHealth";
                        playerHealthObject.transform.localPosition = new Vector3(-310f, -370f, 0);
                        playerHealthObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                    playerHealthObject.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().localScale = new Vector3(1f, 1.1f, 1f);
                    playerHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().color = new Color(1, 1, 1, 1);
                    playerHealthObject.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("PlayerHealth")).FirstOrDefault().sizeDelta = new Vector2(254, 25);
                    playerHealthBar = playerHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("PlayerHealth")).FirstOrDefault();
                    playerHealthBar.color = new Color(1, 0, 0, 1f);

                    if (agentHealthObject == null)
                    {
                        agentHealthObject = Instantiate(statusGuage);
                        agentHealthObject.transform.SetParent(agentContentUI.transform);
                        agentHealthObject.name = "AgentHealth";
                        agentHealthObject.transform.localPosition = new Vector3(-310f, -370f, 0);
                        agentHealthObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                    agentHealthObject.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().localScale = new Vector3(1f, 1.1f, 1f);
                    agentHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().color = new Color(1, 1, 1, 1);
                    agentHealthObject.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("AgentHealth")).FirstOrDefault().sizeDelta = new Vector2(254, 25);
                    agentHealthBar = agentHealthObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("AgentHealth")).FirstOrDefault();
                    agentHealthBar.color = new Color(1, 0, 0, 1f);

                    initialized = true;

                    UpdateCellPhoneVisibility(PlayerDeath.Value != DeathType.None, PlayerStats.Value, AgentDeath.Value != DeathType.None);
                }
                catch
                {
                    Debug.Log("Hardcore Mode Failed to Initialize");
                    initialized = false;
                    visibileHUD = false;
                    SetHudVisibility(visibileHUD);
                }
            }

            public static void Uninitialize()
            {
                initialized = false;
                visibileHUD = false;
                SetHudVisibility(visibileHUD);
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
                if (!visibileHUD)
                    return;

                if (playerController != null && playerController.statusHUD != null)
                    playerController.statusHUD.Update(playerController["health"], playerController["stamina"], playerController["food"], playerController["water"]);
            }

            static void UpdatePlayerStatusWindow()
            {
                playerHealthBar.fillAmount = playerController["health"] / 100;
                playerStomach.text = $"{playerController["food"]:F0}";
                playerThirst.text = $"{playerController["water"]:F0}";
                playerFatigue.text = $"{(100 - playerController["stamina"]):F0}";
            }

            static void UpdateAgentHUDs()
            {
                if (!visibileHUD)
                    return;

                foreach (var controller in agentControllers.Where(n => n != null))
                {
                    if (controller.statusHUD != null)
                    {
                        controller.statusHUD.Update(controller["health"],
                            controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Physical],
                            controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger],
                            controller["water"],
                            controller["revive"]);
                    }
                }
            }

            static void UpdateAgentStatusWindow()
            {
                foreach (var controller in agentControllers.Where(n => n != null))
                {
                    if (selectedAgent != null && selectedAgent.ChaControl == controller.ChaControl)
                    {
                        agentHealthBar.fillAmount = controller["health"] / 100;
                        agentThirst.text = $"{controller["water"]:F0}";
                        return;
                    }
                }
            }

            public static void UpdateSelectedAgent(int selectionID)
            {
                Singleton<Manager.Map>.Instance.AgentTable.TryGetValue(selectionID, out selectedAgent);
            }
        }
    }
}
