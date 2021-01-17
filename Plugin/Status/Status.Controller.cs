using AIProject;
using AIProject.Definitions;

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

                bool allowPlayerDeath = PlayerDeath.Value;
                bool healthLoss = false;

                if (playerController["food"] > 0f)
                {
                    playerController["food"] -= (5.357f * CalorieLoss.Value / CaloriePool.Value) * hourDelta / (Sleep.asleep ? 3 : 1);
                }
                else if (allowPlayerDeath && playerController["health"] > 0)
                {
                    playerController["health"] -= HealthLoss.Value * hourDelta;
                    healthLoss = true;

                    if (playerController["health"] == 0 && Permadeath.Value)
                        TryDelete(Manager.Map.Instance.Player.ChaControl);
                }

                if (playerController["water"] > 0f)
                {
                    playerController["water"] -= (5.357f * WaterLoss.Value / WaterPool.Value) * hourDelta / (Sleep.asleep ? 3 : 1);
                }
                else if (allowPlayerDeath && playerController["health"] > 0)
                {
                    playerController["health"] -= HealthLoss.Value * hourDelta;
                    healthLoss = true;

                    if (playerController["health"] == 0 && Permadeath.Value)
                        TryDelete(Manager.Map.Instance.Player.ChaControl);
                }

                if (Sleep.asleep)
                {
                    playerController["stamina"] += StaminaRate.Value * hourDelta;

                    if (allowPlayerDeath && !healthLoss)
                        playerController["health"] += HealthRate.Value * hourDelta;
                }
                else
                {
                    playerController["stamina"] -= StaminaLoss.Value * hourDelta;
                }

                playerStatsLow = playerController["food"] <= LowFood.Value ||
                                 playerController["water"] <= LowWater.Value ||
                                 playerController["stamina"] <= LowStamina.Value;
            }

            private static void UpdateAgent(LifeStatsController controller, float hourDelta)
            {
                if (!AgentDeath.Value)
                    return;

                if (controller["health"] > 0)
                {
                    bool healthLoss = false;

                    if (controller.agent.StateType == State.Type.Collapse)
                    {
                        controller["health"] -= AgentHealthLoss.Value * hourDelta;
                        healthLoss = true;
                    }

                    if (controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] < AgentLowFood.Value)
                    {
                        controller["health"] -= AgentHealthLossHunger.Value * hourDelta;
                        healthLoss = true;
                    }

                    var thirstLevel = 100 - controller.agent.AgentData.DesireTable[15];
                    controller["water"] -= 5.357f * hourDelta;
                    if (thirstLevel > 35 && controller["water"] < thirstLevel)
                        controller["water"] = thirstLevel;

                    if (controller["water"] < AgentLowWater.Value)
                    {
                        controller["health"] -= AgentHealthLossThirst.Value * hourDelta;
                        healthLoss = true;
                    }

                    switch (controller.agent.AgentData.SickState.ID)
                    {
                        case Sickness.ColdID: controller["health"] -= AgentHealthLossCold.Value * hourDelta; healthLoss = true; break;
                        case Sickness.HeatStrokeID: controller["health"] -= AgentHealthLossHeat.Value * hourDelta; healthLoss = true; break;
                        case Sickness.HurtID: controller["health"] -= AgentHealthLossHurt.Value * hourDelta; healthLoss = true; break;
                        case Sickness.StomachacheID: controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] -= AgentFoodLossStomachache.Value * hourDelta; break;
                        case Sickness.OverworkID: controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Physical] -= AgentStaminaLossOverwork.Value * hourDelta; break;
                    }

                    if (!healthLoss && (controller.agent.StateType == State.Type.Sleep || controller.agent.StateType == State.Type.Immobility))
                        controller["health"] += AgentHealthRate.Value * hourDelta;

                    if (controller["health"] == 0)
                    {
                        if (Permadeath.Value)
                            TryDelete(controller.agent.ChaControl);

                        if (AgentRevive.Value)
                            MapUIContainer.AddNotify($"{controller.agent.CharaName} is incapacitated.");
                        else
                            MapUIContainer.AddNotify($"{controller.agent.CharaName} died.");
                    }
                }
                else if (AgentRevive.Value && !Permadeath.Value)
                {
                    controller["revive"] -= 100f * hourDelta / 24f;

                    if (controller["revive"] == 0)
                    {
                        controller["revive", "water", "stamina", "health"] = 100f;

                        if (AgentReviveReset.Value)
                            ResetAgentStats(controller);
                        else if (AgentRevivePenalty.Value)
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
                playerController.statusHUD.SetVisible(visible, PlayerDeath.Value, PlayerStats.Value);

                foreach (var controller in agentControllers.Where(n => n != null))
                    controller.statusHUD.SetVisible(visible, AgentDeath.Value, AgentDeath.Value);
            }
        }
    }
}
