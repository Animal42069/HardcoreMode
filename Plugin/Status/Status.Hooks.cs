using AIChara;
using AIProject;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		static bool movePatch = false;

		[HarmonyPrefix, HarmonyPatch(typeof(NavMeshAgent), "Move")]
		public static bool Prefix_NavMeshAgent_Move(NavMeshAgent __instance, Vector3 offset)
		{
			if (!PlayerLife.Value || __instance != Manager.Map.Instance.Player.NavMeshAgent)
				return true;

			if (movePatch)
			{
				movePatch = false;

				return true;
			}

			bool flag =
				playerController["food"] <= LowFood.Value ||
				playerController["stamina"] <= LowStamina.Value;

			if (!flag ||
				Input.GetKey(KeyCode.LeftShift) ||
				Input.GetKey(KeyCode.RightShift))
				return true;

			movePatch = true;

			LocomotionProfile.PlayerSpeedSetting speed =
				Manager.Resources.Instance.LocomotionProfile.PlayerSpeed;
			Vector3 platformVelocity = new Traverse(__instance).Field("platformVelocity").GetValue<Vector3>();

			if (platformVelocity == null)
				return true;

			platformVelocity = new Vector3(platformVelocity.x, 0f, platformVelocity.z);

			offset -= platformVelocity;
			offset *= speed.walkSpeed / speed.normalSpeed;

			__instance.Move(offset + platformVelocity);

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
		}

		[HarmonyPostfix, HarmonyPatch(typeof(AIProject.UI.StatusUI), "RefreshEquipments")]
		public static void StatusUI_RefreshEquipments(int id)
        {
			if (id > 0)
				Status.UpdateSelectedAgent(id - 1);
		}
	}
}
