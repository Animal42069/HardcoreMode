using AIProject;
using AIProject.Definitions;
using AIProject.SaveData;
using Manager;
using System.Linq;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        static partial class Status
        {
            public static EnviroSky enviroSky;
            private static float lastUpdateTime;
            public static bool playerStatsLow = false;

            public static void InitializeTime(EnviroSky sky)
            {
                enviroSky = sky;
                lastUpdateTime = enviroSky.internalHour;
            }

            public static void Update()
            {
                if (!initialized || playerController == null)
                    return;

                UpdateHUDVisibility();
                UpdateLifeStats();
                UpdatePlayerStatusWindow();
                UpdateAgentStatusWindow();
                UpdatePlayerHUD();
                UpdateAgentHUDs();
                UpdateWarnings();
            }

            private static void UpdatePlayer(float hourDelta)
            {
                if (!PlayerStats.Value)
                    return;

                bool allowPlayerDeath = PlayerDeath.Value != DeathType.None;

                playerController["food"] -= (5.357f * CalorieLoss.Value / CaloriePool.Value) * hourDelta / (Sleep.asleep ? 3 : 1);
                playerController["water"] -= (5.357f * WaterLoss.Value / WaterPool.Value) * hourDelta / (Sleep.asleep ? 3 : 1);
                if (Sleep.asleep)
                    playerController["stamina"] += StaminaRate.Value * hourDelta;
                else
                    playerController["stamina"] -= StaminaLoss.Value * hourDelta;

                playerStatsLow = playerController["food"] <= LowFood.Value ||
                                 playerController["water"] <= LowWater.Value ||
                                 playerController["stamina"] <= LowStamina.Value;

                if (!allowPlayerDeath || playerController["health"] <= 0)
                    return;

                bool healthLoss = false;

                if (playerController["food"] <= 0)
                {
                    playerController["health"] -= HealthLoss.Value * hourDelta;
                    healthLoss = true;
                }

                if (playerController["water"] <= 0)
                {
                    playerController["health"] -= HealthLoss.Value * hourDelta;
                    healthLoss = true;
                }

                if (playerController["health"] == 0 && (PlayerDeath.Value == DeathType.PermaDeath))
                    TryDelete(Manager.Map.Instance.Player.ChaControl);

                if (healthLoss)
                    return;

                if (Sleep.asleep)
                    playerController["health"] += HealthRate.Value * hourDelta * 3;
                else
                    playerController["health"] += HealthRate.Value * hourDelta;
            }

            private static void UpdateAgent(LifeStatsController controller, float hourDelta)
            {
                if (AgentDeath.Value == DeathType.None)
                    return;

                if (controller["health"] > 0)
                {
                    bool healthLoss = false;

                    if (controller.agent.StateType == State.Type.Collapse)
                    {
                        controller["health"] -= AgentHealthLoss.Value * hourDelta;
                        healthLoss = true;
                    }

                    if (controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] <= AgentLowFood.Value)
                    {
                        controller["health"] -= AgentHealthLossHunger.Value * hourDelta;
                        healthLoss = true;
                    }

                    var thirstLevel = 100 - controller.agent.AgentData.DesireTable[15];
                    controller["water"] -= 5.357f * hourDelta;
                    if (thirstLevel > 35 && controller["water"] < thirstLevel)
                        controller["water"] = thirstLevel;

                    if (controller["water"] <= AgentLowWater.Value)
                    {
                        controller["health"] -= AgentHealthLossThirst.Value * hourDelta;
                        healthLoss = true;
                    }

                    switch (controller.agent.AgentData.SickState.ID)
                    {
                        case AIProject.Definitions.Sickness.ColdID: controller["health"] -= AgentHealthLossCold.Value * hourDelta; healthLoss = true; break;
                        case AIProject.Definitions.Sickness.HeatStrokeID: controller["health"] -= AgentHealthLossHeat.Value * hourDelta; healthLoss = true; break;
                        case AIProject.Definitions.Sickness.HurtID: controller["health"] -= AgentHealthLossHurt.Value * hourDelta; healthLoss = true; break;
                        case AIProject.Definitions.Sickness.StomachacheID: controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] -= AgentFoodLossStomachache.Value * hourDelta; break;
                        case AIProject.Definitions.Sickness.OverworkID: controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Physical] -= AgentStaminaLossOverwork.Value * hourDelta; break;
                    }

                    if (!healthLoss)
                    {
                        if ((controller.agent.StateType == State.Type.Sleep || controller.agent.StateType == State.Type.Immobility))
                            controller["health"] += AgentHealthRate.Value * hourDelta * 3;
                        else
                            controller["health"] += AgentHealthRate.Value * hourDelta;
                    }

                    if (controller["health"] == 0)
                    {
                        if (AgentDeath.Value == DeathType.PermaDeath)
                            TryDelete(controller.agent.ChaControl);

                        if (AgentDeath.Value == DeathType.Incapacitated)
                            MapUIContainer.AddNotify($"{controller.agent.CharaName} is incapacitated.");
                        else
                            MapUIContainer.AddNotify($"{controller.agent.CharaName} died.");
                    }
                }
                else if (AgentDeath.Value == DeathType.Incapacitated && controller.agent.StateType == State.Type.Immobility)
                {
                    controller["revive"] += 100f * hourDelta / 24f;

                    if (controller["revive"] >= 100)
                    {
                        controller["water", "stamina", "health"] = 100f;
                        controller["revive"] = 0;

                        if (AgentRevivePenalty.Value == RevivePenalty.StatReset)
                            ResetAgentStats(controller);
                        else if (AgentRevivePenalty.Value == RevivePenalty.StatLoss)
                            PenalizeAgentStats(controller);

                        MapUIContainer.AddNotify($"{controller.agent.CharaName} has been revived.");
                    }
                }
            }

            private static void UpdateLifeStats()
            {
                if (enviroSky == null || enviroSky.GameTime.ProgressTime == EnviroTime.TimeProgressMode.None)
                    return;

                float thisUpdateTime = enviroSky.internalHour;
                float timeHourDelta = thisUpdateTime - lastUpdateTime;

                if (thisUpdateTime < lastUpdateTime)
                    timeHourDelta += 24;

                if (timeHourDelta < 0.02)
                    return;

                lastUpdateTime = thisUpdateTime;

                if (timeHourDelta > 1)
                    return;

                foreach (var controller in agentControllers.Where(n => n != null))
                    UpdateAgent(controller, timeHourDelta);

                UpdatePlayer(timeHourDelta);
            }

            private static void UpdateHUDVisibility()
            {
                if (StatusKey.Value.IsDown())
                {
                    visibileHUD = !visibileHUD;
                    SetHudVisibility(visibileHUD);
                }
            }

            public static void SetHudVisibility(bool visible)
            {
                playerController.statusHUD.SetVisible(visible, PlayerDeath.Value != DeathType.None, PlayerStats.Value);

                foreach (var controller in agentControllers.Where(n => n != null))
                    controller.statusHUD.SetVisible(visible, AgentDeath.Value != DeathType.None, AgentDeath.Value != DeathType.None);
            }

            public static void RevivePlayer()
            {
                playerController["food", "water", "stamina", "health"] = 100f;

                var playerData = Singleton<Game>.Instance.WorldData.PlayerData;
                if (playerData == null)
                    return;

                playerData.LastAcquiredItem = new StuffItem(0, -1, 0);
                playerData.EquipedBackItem = new StuffItem(0, -1, 0);
                playerData.EquipedFishingItem = new StuffItem(0, -1, 0);
                playerData.EquipedGloveItem = new StuffItem(0, -1, 0);
                playerData.EquipedHeadItem = new StuffItem(0, -1, 0);
                playerData.EquipedLampItem = new StuffItem(0, -1, 0);
                playerData.EquipedNeckItem = new StuffItem(-0, -1, 0);
                playerData.EquipedNetItem = new StuffItem(0, -1, 0);
                playerData.EquipedPickelItem = new StuffItem(0, -1, 0);
                playerData.EquipedShovelItem = new StuffItem(0, -1, 0);
                playerData.FishingSkill = new AIProject.SaveData.Skill();
                playerData.ItemList.Clear();
                playerData.CraftPossibleTable.Clear();
                playerData.FirstCreatedItemTable.Clear();

                var playerControl = Manager.Map.Instance.Player.ChaControl;
                if (playerControl != null)
                {
                    playerControl.ChangeExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Back, -1);
                    playerControl.ChangeExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Head, -1);
                    playerControl.ChangeExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Neck, -1);
                    playerControl.ChangeExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Waist, -1);
                    playerControl.ShowExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Back, false);
                    playerControl.ShowExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Head, false);
                    playerControl.ShowExtraAccessory(AIChara.ChaControlDefine.ExtraAccessoryParts.Neck, false);
                }
            }
        }
    }
}
