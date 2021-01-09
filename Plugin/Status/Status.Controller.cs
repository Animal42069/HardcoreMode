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

            public static void Initialize(EnviroSky sky)
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

            static void UpdatePlayer(float hourDelta)
            {
                if (!PlayerStats.Value)
                    return;

                bool allowPlayerDeath = PlayerDeath.Value;

                if (playerController["food"] > 0f)
                {
                    playerController["food"] -= (5.357f * CalorieLoss.Value / CaloriePool.Value) * hourDelta / (Sleep.asleep ? 3 : 1);
                }
                else if (allowPlayerDeath && playerController["health"] > 0)
                {
                    playerController["health"] -= HealthLoss.Value * hourDelta;

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

                    if (playerController["health"] == 0 && Permadeath.Value)
                        TryDelete(Manager.Map.Instance.Player.ChaControl);
                }

                if (Sleep.asleep)
                {
                    playerController["stamina"] += StaminaRate.Value * hourDelta;

                    if (allowPlayerDeath)
                        playerController["health"] += HealthRate.Value * hourDelta;
                }
                else
                {
                    playerController["stamina"] -= StaminaLoss.Value * hourDelta;
                }
            }

            static void UpdateAgent(LifeStatsController controller, float hourDelta)
            {
                if (controller["health"] > 0)
                {
                    if (AgentDeath.Value)
                    {
                        if (controller.agent.StateType == State.Type.Sleep || controller.agent.StateType == State.Type.Immobility)
                            controller["health"] += AgentHealthRate.Value * hourDelta;
                        else if (controller.agent.StateType == State.Type.Collapse)
                            controller["health"] -= AgentHealthLoss.Value * hourDelta;

                        if (controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] < AgentLowFood.Value)
                            controller["health"] -= AgentHealthLossHunger.Value * hourDelta;

                        var thirstLevel = 100 - controller.agent.AgentData.DesireTable[15];
                        controller["water"] -= 5.357f * hourDelta;
                        if (thirstLevel > 35 && controller["water"] < thirstLevel)
                            controller["water"] = thirstLevel;

                        if (controller["water"] < AgentLowWater.Value)
                            controller["health"] -= AgentHealthLossThirst.Value * hourDelta;

                        switch(controller.agent.AgentData.SickState.ID)
                        {
                            case Sickness.ColdID: controller["health"] -= AgentHealthLossCold.Value * hourDelta; break;
                            case Sickness.HeatStrokeID: controller["health"] -= AgentHealthLossHeat.Value * hourDelta; break;
                            case Sickness.HurtID: controller["health"] -= AgentHealthLossHurt.Value * hourDelta; break;
                            case Sickness.StomachacheID: controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] -= AgentFoodLossStomachache.Value * hourDelta; break;
                            case Sickness.OverworkID: controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Physical] -= AgentStaminaLossOverwork.Value * hourDelta; break;
                        }

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
                }
                else if (AgentRevive.Value && !Permadeath.Value)
                {
                    controller["revive"] -= 100f * hourDelta / 24f;

                    if (controller["revive"] == 0)
                    {
                        controller["revive", "food", "stamina", "health"] = 100f;

                        if (AgentReviveReset.Value)
                            ResetAgentStats(controller);
                        else if (AgentRevivePenalty.Value)
                            PenalizeAgentStats(controller);

                        MapUIContainer.AddNotify($"{controller.agent.CharaName} has been revived.");
                    }
                }
            }

            static void UpdateLifeStats()
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

            static void UpdateHUDVisibility()
            {

                if (StatusKey.Value.IsDown())
                {
                    visibileHUD = !visibileHUD;

                    playerController.statusHUD.SetVisible(visibileHUD, PlayerDeath.Value, PlayerStats.Value);

                    foreach (var controller in agentControllers.Where(n => n != null))
                        controller.statusHUD.SetVisible(visibileHUD, AgentDeath.Value, AgentDeath.Value);
                }
            }
        }
    }
}
