using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HardcoreMode
{
	public partial class HardcoreMode
	{
		public static LifeStatsController playerController;
		static readonly List<LifeStatsController> controllersQueue = new List<LifeStatsController>();
		static readonly List<LifeStatsController> agentControllers = new List<LifeStatsController>();
		static readonly List<LifeStatsController> agentControllersDump = new List<LifeStatsController>();

		public void Update()
		{
			Status.Update();
			FoodMenu.Update();
			Sleep.Update();
		}

		public void LateUpdate()
		{
			FoodMenu.LateUpdate();
			Dead.LateUpdate();
		}

		public void OnGUI()
		{
			UpdateControllers();

			FoodMenu.OnGUI();
			Sleep.OnGUI();
			Dead.OnGUI();
		}
	}
}
