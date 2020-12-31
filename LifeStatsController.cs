﻿using AIProject;
using AIProject.Definitions;
using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Chara;
using MessagePack;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HardcoreMode
{
	public class LifeStatsController : CharaCustomFunctionController
	{
		public AgentActor agent;
		Dictionary<string, float> stats = new Dictionary<string, float>();
		public StatusHUD statusHUD;

		protected override void OnCardBeingSaved(GameMode currentGameMode)
		{
			if (stats.Count == 0 ||
				(agent == null &&
				(!Manager.Map.IsInstance() || Manager.Map.Instance.Player?.ChaControl != ChaControl)))
			{
				SetExtendedData(null);
				return;
			}

			PluginData data = new PluginData
			{
				version = 1
			};

			data.data.Add("stats", LZ4MessagePackSerializer.Serialize(stats));

			SetExtendedData(data);
		}

		protected override void OnReload(GameMode currentGameMode, bool maintainState)
		{
			PluginData data = GetExtendedData();

			if (data != null)
			{
				Dictionary<string, float> newStats;

				try
				{
					newStats = LZ4MessagePackSerializer
						.Deserialize<Dictionary<string, float>>((byte[])data.data["stats"]);
				}
				catch (Exception err)
				{
					Debug.Log($"[Hardcore Mode] Failed to load extended data.\n{err}");
					return;
				}

				stats = newStats;
			}
		}

		protected override void Start()
		{
			base.Start();

			if (ChaControl.gameObject.transform.parent.name.Contains("Merchant"))
				return;

			if (ChaControl.gameObject.transform.parent.name.Contains("Player"))
				statusHUD = new StatusHUD("", new Vector3(-950, 515, 0));
			else
				statusHUD = new StatusHUD(ChaControl.fileParam.fullname, new Vector3(-950, 450, 0));
			HardcoreMode.AddController(this);
		}

		protected override void OnDestroy()
        {
			statusHUD.Destroy();
			HardcoreMode.RemoveController(this);

			base.OnDestroy();
		}

		protected void LateUpdate()
		{
			if (HardcoreMode.AgentDeath.Value &&
				agent != null &&
				stats["health"] == 0 &&
				agent.StateType != State.Type.Collapse &&
				agent.StateType != State.Type.Immobility)
				agent.StartWeakness();
		}

		public float this[string key]
		{
			get
			{
				if (!stats.ContainsKey(key))
					stats[key] = 100f;

				return stats[key];
			}
			set
			{
				if (!stats.ContainsKey(key))
					stats[key] = 100f;

				if (value != stats[key])
					stats[key] = Mathf.Clamp(value, 0f, 100f);
			}
		}

		public float this[params string[] keys]
		{
			set
			{
				foreach (string key in keys)
					this[key] = value;
			}
		}
	}
}
