using AIChara;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        [HarmonyPrefix, HarmonyPatch(typeof(Manager.Input), "IsDown", typeof(KeyCode))]
        public static bool PrefixInputIsDown(KeyCode key, ref bool __result)
        {
            if (!Status.playerStatsLow || (key != KeyCode.LeftShift && key != KeyCode.RightShift))
                return true;

            __result = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaFile), "SaveFile", typeof(BinaryWriter), typeof(bool), typeof(int))]
        public static bool Prefix_ChaFile_SaveFile(ChaFile __instance, BinaryWriter bw, bool savePng, int lang)
        {
            if (playerController == null)
                return true;

            if (PlayerDeath.Value == DeathType.PermaDeath && playerController.ChaFileControl == __instance)
            {
                if (playerController["health"] == 0)
                    return false;
            }
            else if (AgentDeath.Value == DeathType.PermaDeath)
            {
                foreach (var controller in agentControllers.Where(n => n != null))
                    if (controller.ChaFileControl == __instance && controller["health"] == 0)
                        return false;
            }

            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Manager.Map), "InitSearchActorTargetsAll")]
        public static void MapManager_InitSearchActorTargetsAll()
        {
            Status.Initialize();
            FoodMenu.Initialize();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Manager.Housing), "EndHousing")]
        private static void Housing_EndHousing()
        {
            Status.Initialize();
            FoodMenu.Initialize();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AIProject.UI.StatusUI), "RefreshEquipments")]
        public static void StatusUI_RefreshEquipments(int id)
        {
            if (id <= 0)
                return;

            Status.UpdateSelectedAgent(id - 1);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(EnviroSky), "AssignAndStart")]
        public static void EnviroSky_AssignAndStart(EnviroSky __instance)
        {
            Status.InitializeTime(__instance);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Manager.Map), "ReleaseMap")]
        public static void MapManager_ReleaseMap()
        {
            Status.Uninitialize();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Manager.Housing), "StartHousing")]
        private static void Housing_StartHousing()
        {
            Status.Uninitialize();
        }
    }
}
