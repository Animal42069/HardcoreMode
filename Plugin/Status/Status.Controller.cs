using AIProject;
using AIProject.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		static partial class Status
		{
			public static void Update()
			{
				if (playerController == null)
					return;

				if (StatusKey.Value.IsDown())
				{
					visible = !visible;
					playerController.statusHUD.SetVisibility(visible, PlayerDeath.Value, PlayerLife.Value);

					foreach (var controller in agentControllers.Where(n => n != null))
						controller.statusHUD.SetVisibility(visible, AgentDeath.Value, AgentDeath.Value);
				}

				float dt = (Time.deltaTime * 24f) / (60f * Manager.Map.Instance.EnvironmentProfile.DayLengthInMinute);

				foreach (var controller in agentControllers.Where(n => n != null))
					Update_Agent(controller, dt);

				Update_Player(dt);
				UpdatePlayerHUD();
				UpdateAgentHUDs();
				TryWarn();
			}

			static void Update_Player(float dt)
			{
				bool usePlayerStats = PlayerLife.Value;
				bool allowPlayerDeath = PlayerDeath.Value;

				if (playerController["food"] > 0f)
				{
					if (usePlayerStats)
						playerController["food"] -= FoodLoss.Value * dt / (Sleep.asleep ? FoodLossSleepFactor.Value : 1);
				}
				else if (allowPlayerDeath && playerController["health"] > 0)
				{
					playerController["health"] -= HealthLoss.Value * dt;

					if (playerController["health"] == 0 && Permadeath.Value)
						TryDelete(Manager.Map.Instance.Player.ChaControl);
				}

				if (Sleep.asleep)
				{
					float factor = dt;

					if (usePlayerStats)
						playerController["stamina"] += StaminaRate.Value * factor;

					if (allowPlayerDeath)
						playerController["health"] += HealthRate.Value * factor;
				}
				else if (usePlayerStats)
				{
					playerController["stamina"] -= StaminaLoss.Value * dt;
				}
			}

			static void Update_Agent(LifeStatsController controller, float dt)
			{
				if (controller["health"] > 0)
				{
					if (AgentDeath.Value)
					{
						if (controller.agent.StateType == State.Type.Sleep || controller.agent.StateType == State.Type.Immobility)
							controller["health"] += AgentHealthRate.Value * dt;
						else if (controller.agent.StateType == State.Type.Collapse)
							controller["health"] -= AgentHealthLoss.Value * dt;

						if (controller.agent.AgentData.StatsTable[(int)AIProject.Definitions.Status.Type.Hunger] < AgentLowFood.Value)
							controller["health"] -= AgentHealthLossHunger.Value * dt;

						if (controller.agent.AgentData.SickState.ID == Sickness.ColdID)
							controller["health"] -= AgentHealthLossCold.Value * dt;
						else if (controller.agent.AgentData.SickState.ID == Sickness.HeatStrokeID)
							controller["health"] -= AgentHealthLossHeat.Value * dt;
						else if (controller.agent.AgentData.SickState.ID == Sickness.HurtID)
							controller["health"] -= AgentHealthLossHurt.Value * dt;

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
					controller["revive"] -= 100f * dt / 24f;

					if (controller["revive"] == 0)
					{
						controller["revive", "food", "stamina", "health"] = 100f;

						if (AgentReviveReset.Value)
						{
							List<int> keys = controller.ChaControl.fileGameInfo.flavorState.Keys.ToList();

							foreach (int key in keys)
								controller.agent.AgentData.SetFlavorSkill(key, 0);

							controller.agent.SetPhase(0);

							controller.ChaControl.fileGameInfo.lifestyle = -1;

							List<int> skillKeys = controller.ChaControl.fileGameInfo.normalSkill.Keys.ToList();
							foreach (int key in skillKeys)
								controller.ChaControl.fileGameInfo.normalSkill.Remove(key);

							List<int> hSkillKeys = controller.ChaControl.fileGameInfo.hSkill.Keys.ToList();
							foreach (int key in hSkillKeys)
								controller.ChaControl.fileGameInfo.hSkill.Remove(key);
						}
						else if (AgentRevivePenalty.Value)
                        {
							List<int> keys = controller.ChaControl.fileGameInfo.flavorState.Keys.ToList();

							foreach (int key in keys)
							{
								if (key == (int)FlavorSkill.Type.Wariness || key == (int)FlavorSkill.Type.Darkness)
									controller.agent.AgentData.SetFlavorSkill(key, controller.agent.GetFlavorSkill(key) + 200);
								else
									controller.agent.AgentData.SetFlavorSkill(key, controller.agent.GetFlavorSkill(key) - 200);
							}

							int phase = controller.ChaControl.fileGameInfo.phase;
							if (phase > 0)
								controller.agent.SetPhase(phase - 1);

							if (phase <= 3)
								controller.ChaControl.fileGameInfo.lifestyle = -1;

							List<int> skillKeys = controller.ChaControl.fileGameInfo.normalSkill.Keys.ToList();

							foreach (int key in skillKeys)
                            {
								if (UnityEngine.Random.Range(1, 100) <= 20)
									controller.ChaControl.fileGameInfo.normalSkill.Remove(key);
							}

							List<int> hSkillKeys = controller.ChaControl.fileGameInfo.hSkill.Keys.ToList();
							foreach (int key in hSkillKeys)
							{
								if (UnityEngine.Random.Range(1, 100) <= 20)
									controller.ChaControl.fileGameInfo.hSkill.Remove(key);
							}
						}

						MapUIContainer.AddNotify($"{controller.agent.CharaName} has been revived.");
					}
				}
			}
		}
	}
}
