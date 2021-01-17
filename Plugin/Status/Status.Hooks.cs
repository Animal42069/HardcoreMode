using AIChara;
using HarmonyLib;
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
            if (playerController == null ||
                (!PlayerDeath.Value && !AgentDeath.Value) || !Permadeath.Value)
                return true;

            if (playerController.ChaFileControl == __instance)
            {
                if (playerController["health"] == 0)
                    return false;
            }
            else
                foreach (var controller in agentControllers.Where(n => n != null))
                    if (controller.ChaFileControl == __instance && controller["health"] == 0)
                        return false;

            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Manager.Map), "InitSearchActorTargetsAll")]
        public static void MapManager_InitSearchActorTargetsAll()
        {
            Status.Initialize();
            FoodMenu.Initialize();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AIProject.UI.StatusUI), "RefreshEquipments")]
        public static void StatusUI_RefreshEquipments(int id)
        {
            if (id > 0)
                Status.UpdateSelectedAgent(id - 1);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(EnviroSky), "AssignAndStart")]
        public static void EnviroSky_AssignAndStart(EnviroSky __instance)
        {
            Status.InitializeTime(__instance);
        }

    }
}
