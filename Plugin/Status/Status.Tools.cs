using AIChara;
using AIProject;
using AIProject.Definitions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        static public partial class Status
        {
            static readonly List<LifeStatsController> agentWarn = new List<LifeStatsController>();
            static bool healthWarn = false;
            static bool foodWarn = false;
            static bool waterWarn = false;
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

            static void WarningCheck(LifeStatsController controller, int threshold)
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

            static void WarningCheck(string key, int threshold, ref bool flag)
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

            public static void UpdateWarnings()
            {
                WarningCheck("health", HealthWarn.Value, ref healthWarn);
                WarningCheck("food", FoodWarn.Value, ref foodWarn);
                WarningCheck("water", WaterWarn.Value, ref waterWarn);
                WarningCheck("stamina", StaminaWarn.Value, ref staminaWarn);

                int agentThreshold = AgentWarn.Value;

                foreach (var controller in agentControllers.Where(n => n != null))
                    WarningCheck(controller, agentThreshold);
            }

            public static void TryDelete(ChaControl chaCtrl)
            {
                string path = chaCtrl.chaFile.ConvertCharaFilePath(
                    chaCtrl.chaFile.charaFileName,
                    chaCtrl.sex
                );
                string directory = System.IO.Path.GetDirectoryName(path);

                Manager.Map.Instance.AgentTable[0].RemoveActor(Manager.Map.Instance.AgentTable[0]);

                if (Directory.Exists(directory) && File.Exists(path))
                    File.Delete(path);
            }

            public static void ResetAgentStats(LifeStatsController controller)
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

            public static void PenalizeAgentStats(LifeStatsController controller)
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
        }
    }
}
