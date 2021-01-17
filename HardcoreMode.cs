using BepInEx;
using BepInEx.Configuration;
using KKAPI.Chara;
using System.Linq;
using UnityEngine;
using KeyboardShortcut = BepInEx.Configuration.KeyboardShortcut;

namespace HardcoreMode
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInProcess("AI-Syoujyo")]
    public partial class HardcoreMode : BaseUnityPlugin
    {
        const string GUID = "com.fairbair.hardcoremode";
        const string Name = "Hardcore Mode";
        const string Version = "2.0.1";
        const string BEHAVIOR = "HardcoreMode.LifeStats";

        const string SECTION_GENERAL = "General";
        const string SECTION_SURVIVAL = "Survival Stats";
        const string SECTION_SLEEP = "Sleep Override";
        const string SECTION_LOSS = "Stat Loss (per Game-Hour)";
        const string SECTION_RECOVER = "Stat Recovery";
        const string SECTION_PENALTY = "Penalties";
        const string SECTION_WARN = "Warnings";

        const string DESCRIPTION_PLAYER_LIFE =
            "This enables the player survival stats. " +
            "Does not include the 'Health' stat.";
        const string DESCRIPTION_PLAYER_DEATH =
            "Enables the player's health stat and allows the player to die. " +
            "The player is permanently dead and you will have to change characters. " +
            "You can revive the dead character by loading them as an agent.";
        const string DESCRIPTION_AGENT_DEATH =
            "Enables the agent's health stat and allows the agents to die. " +
            "When dead, the agent is permanently collapsed.";
        const string DESCRIPTION_AGENT_REVIVE =
            "Allows the agents to revive from death after 1 in-game day. " +
            "Revival is halted when this is disabled.";

        const string DESCRIPTION_SET_HOURS_ASLEEP =
            "Allows you to set the total number of hours asleep. " +
            "A window will be shown along with the sleep dialog.";
        const string DESCRIPTION_WAKE_HOUR =
            "The hour at which your character wakes up. " +
            "If you sleep after the designated hour, " +
            "you'll wake up the next day at the same hour. " +
            "This does not apply if you enable 'Set Hours Asleep.'";

        const string DESCRIPTION_AGENT_HEALTH_LOSS =
            "The amount of health lost overtime when an agent has collapsed. " +
            "Putting them to bed will halt the loss rate. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_HEALTH_LOSS_COLD =
            "The amount of health lost overtime when an agent has a cold. " +
            "Curing the cold will halt the loss rate. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_HEALTH_LOSS_HEAT =
            "The amount of health lost overtime when an agent has heatstroke. " +
            "Curing the heatstroke will halt the loss rate. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_HEALTH_LOSS_HURT =
            "The amount of health lost overtime when an agent is injured. " +
            "Curing the injury will halt the loss rate. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_HEALTH_LOSS_HUNGER =
            "The amount of health lost overtime when an agent is hungry. " +
            "When the agent eats, it will halt the loss rate. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_HEALTH_LOSS_THIRST =
            "The amount of health lost overtime when an agent is thirsty. " +
            "When the agent drinks, it will halt the loss rate. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_FOOD_LOSS_STOMACHACHE =
            "The amount of food lost overtime when an agent has a stomachache. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_AGENT_STAMINA_LOSS_OVERWORK =
            "The amount of stamina is lost overtime when an agent is overworked. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_HEALTH_LOSS =
            "The amount of health lost overtime when the player has 0% food. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_STAMINA_LOSS =
            "The amount of stamina lost overtime. " +
            "100% means they lose 100% of their stamina over the set interval.";
        const string DESCRIPTION_CALORIE_POOL =
            "The total number of calories the player can 'store'.";
        const string DESCRIPTION_CALORIE_LOSS =
            "The number of calories the player loses per day.";
        const string DESCRIPTION_WATER_POOL =
            "The total number of milliliters of water the player can 'store'.";
        const string DESCRIPTION_WATER_LOSS =
            "The number of milliliters of water the player loses per day.";

        const string DESCRIPTION_AGENT_HEALTH_RATE =
            "How much health is recovered overtime when an agent sleeps? " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_HEALTH_RATE =
            "How much health is recovered overtime when the player sleeps? " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_STAMINA_RATE =
            "How much stamina is recovered overtime when the player sleeps? " +
            "100% means they recover 100% of their stamina per minute. " +
            "Interval is in in-game hours.";
        const string DESCRIPTION_CALORIE_RATE =
            "How many calories are replenished by food." +
            "100 means they gain 100% of the food's calories.";
        const string DESCRIPTION_WATER_RATE =
            "How much water is replenished by food/drinks." +
            "100 means they gain 100% of the item's amount.";

        const string DESCRIPTION_AGENT_LOW_FOOD =
            "Hunger value when agent will begin to lose health due to hunger.";
        const string DESCRIPTION_AGENT_LOW_WATER =
            "Thirst value when agent will begin to lose health due to thirst.";
        const string DESCRIPTION_LOW_FOOD =
            "Having your food below or equal to this value will force you to walk.";
        const string DESCRIPTION_LOW_WATER =
            "Having your water below or equal to this value will force you to walk.";
        const string DESCRIPTION_LOW_STAMINA =
            "Having your stamina below or equal to this value will force you to walk.";
        const string DESCRIPTION_PLAYER_DEATH_RESET =
            "When enabled, if the player dies all agents stats reset to 0, hearts to 1 and lose all skills.";
        const string DESCRIPTION_AGENT_REVIVE_RESET =
            "When enabled, this will reset their stats to 0, hearts to 1 and lose all skills when they revive.";
        const string DESCRIPTION_AGENT_REVIVE_PENALTY =
            "When enabled, this will increase darkness and wariness by 200, decrease all other stats by 200 " +
            "Reduce hearts by 1 level, and randomly lose skills";
        const string DESCRIPTION_PERMADEATH =
            "When enabled, dying will DELETE the card. " +
            "This includes both the player character and the agent. " +
            "Cards that are deleted WILL NOT be sent to the recycle bin.";

        const string DESCRIPTION_HEALTH_WARN =
            "The game warns you if your health falls below or equal to this value.";
        const string DESCRIPTION_AGENT_WARN =
            "The game warns you if an agent's health falls below or equal to this value.";
        const string DESCRIPTION_FOOD_WARN =
            "The game warns you if your food falls below or equal to this value.";
        const string DESCRIPTION_WATER_WARN =
            "The game warns you if your water falls below or equal to this value.";
        const string DESCRIPTION_STAMINA_WARN =
            "The game warns you if your stamina falls below or equal to this value.";

        internal static ConfigEntry<int> WindowIDSleep { get; set; }
        internal static ConfigEntry<int> WindowIDDead { get; set; }

        internal static ConfigEntry<KeyboardShortcut> StatusKey { get; set; }
        internal static ConfigEntry<bool> PlayerStats { get; set; }
        internal static ConfigEntry<bool> PlayerDeath { get; set; }
        internal static ConfigEntry<bool> AgentDeath { get; set; }
        internal static ConfigEntry<bool> AgentRevive { get; set; }

        internal static ConfigEntry<bool> SleepAnytime { get; set; }
        internal static ConfigEntry<bool> SetHoursAsleep { get; set; }
        internal static ConfigEntry<int> WakeHour { get; set; }

        internal static ConfigEntry<float> AgentHealthLoss { get; set; }
        internal static ConfigEntry<float> AgentHealthLossCold { get; set; }
        internal static ConfigEntry<float> AgentHealthLossHeat { get; set; }
        internal static ConfigEntry<float> AgentHealthLossHurt { get; set; }
        internal static ConfigEntry<float> AgentHealthLossHunger { get; set; }
        internal static ConfigEntry<float> AgentHealthLossThirst { get; set; }
        internal static ConfigEntry<float> AgentFoodLossStomachache { get; set; }
        internal static ConfigEntry<float> AgentStaminaLossOverwork { get; set; }

        internal static ConfigEntry<float> HealthLoss { get; set; }
        internal static ConfigEntry<float> CalorieLoss { get; set; }
        internal static ConfigEntry<float> CaloriePool { get; set; }
        internal static ConfigEntry<float> WaterLoss { get; set; }
        internal static ConfigEntry<float> WaterPool { get; set; }
        internal static ConfigEntry<float> StaminaLoss { get; set; }

        internal static ConfigEntry<float> AgentHealthRate { get; set; }
        internal static ConfigEntry<float> HealthRate { get; set; }
        internal static ConfigEntry<float> StaminaRate { get; set; }
        internal static ConfigEntry<float> CalorieRate { get; set; }
        internal static ConfigEntry<float> WaterRate { get; set; }

        internal static ConfigEntry<float> AgentLowFood { get; set; }
        internal static ConfigEntry<float> AgentLowWater { get; set; }
        internal static ConfigEntry<float> LowFood { get; set; }
        internal static ConfigEntry<float> LowWater { get; set; }
        internal static ConfigEntry<float> LowStamina { get; set; }
        internal static ConfigEntry<bool> AgentReviveReset { get; set; }
        internal static ConfigEntry<bool> AgentRevivePenalty { get; set; }
        internal static ConfigEntry<bool> Permadeath { get; set; }
        internal static ConfigEntry<bool> PlayerDeathReset { get; set; }

        
        internal static ConfigEntry<int> HealthWarn { get; set; }
        internal static ConfigEntry<int> AgentWarn { get; set; }
        internal static ConfigEntry<int> FoodWarn { get; set; }
        internal static ConfigEntry<int> WaterWarn { get; set; }
        internal static ConfigEntry<int> StaminaWarn { get; set; }

        public void Awake()
        {
            WindowIDSleep = Config.Bind(SECTION_GENERAL, "__Window ID (Sleep Hours)", 83464);
            WindowIDDead = Config.Bind(SECTION_GENERAL, "__Window ID (Dead)", 83465);

            StatusKey = Config.Bind(SECTION_SURVIVAL, "Status UI Key", new KeyboardShortcut(KeyCode.T));
            (PlayerStats = Config.Bind(SECTION_SURVIVAL, "Player Life Stats", true, DESCRIPTION_PLAYER_LIFE)).SettingChanged += (s, e) =>
            {
                playerController.statusHUD.SetVisible(Status.visibileHUD, PlayerDeath.Value, PlayerStats.Value);
                Status.UpdateCellPhoneVisibility(PlayerDeath.Value, PlayerStats.Value, AgentDeath.Value);
            };
            (PlayerDeath = Config.Bind(SECTION_SURVIVAL, "Player Death", true, DESCRIPTION_PLAYER_DEATH)).SettingChanged += (s, e) =>
            {
                playerController.statusHUD.SetVisible(Status.visibileHUD, PlayerDeath.Value, PlayerStats.Value);
                Status.UpdateCellPhoneVisibility(PlayerDeath.Value, PlayerStats.Value, AgentDeath.Value);
            };
            (AgentDeath = Config.Bind(SECTION_SURVIVAL, "Agent Death", true, DESCRIPTION_AGENT_DEATH)).SettingChanged += (s, e) =>
            {
                foreach (var controller in agentControllers.Where(n => n != null))
                    controller.statusHUD.SetVisible(Status.visibileHUD, AgentDeath.Value, AgentDeath.Value);

                Status.UpdateCellPhoneVisibility(PlayerDeath.Value, PlayerStats.Value, AgentDeath.Value);
            };

            AgentRevive = Config.Bind(SECTION_SURVIVAL, "Agent Revival", true, DESCRIPTION_AGENT_REVIVE);
            SleepAnytime = Config.Bind(SECTION_SLEEP, "Sleep Anytime", true);
            SetHoursAsleep = Config.Bind(SECTION_SLEEP, "Set Hours Asleep", true, DESCRIPTION_SET_HOURS_ASLEEP);
            WakeHour = Config.Bind(SECTION_SLEEP, "Wake Up Hour", 8, new ConfigDescription(DESCRIPTION_WAKE_HOUR, new AcceptableValueRange<int>(0, 23)));

            AgentHealthLoss = Config.Bind(SECTION_LOSS, "Agent Health Loss - Collapsed", 2f, new ConfigDescription(DESCRIPTION_AGENT_HEALTH_LOSS, new AcceptableValueRange<float>(0, 100)));
            AgentHealthLossCold = Config.Bind(SECTION_LOSS, "Agent Health Loss - Cold", 1f, new ConfigDescription(DESCRIPTION_AGENT_HEALTH_LOSS_COLD, new AcceptableValueRange<float>(0, 100)));
            AgentHealthLossHeat = Config.Bind(SECTION_LOSS, "Agent Health Loss - Heatstroke", 4f, new ConfigDescription(DESCRIPTION_AGENT_HEALTH_LOSS_HEAT, new AcceptableValueRange<float>(0, 100)));
            AgentHealthLossHurt = Config.Bind(SECTION_LOSS, "Agent Health Loss - Hurt", 6f, new ConfigDescription(DESCRIPTION_AGENT_HEALTH_LOSS_HURT, new AcceptableValueRange<float>(0, 100)));
            AgentHealthLossHunger = Config.Bind(SECTION_LOSS, "Agent Health Loss - Hunger", 2f, new ConfigDescription(DESCRIPTION_AGENT_HEALTH_LOSS_HUNGER, new AcceptableValueRange<float>(0, 100)));
            AgentHealthLossThirst = Config.Bind(SECTION_LOSS, "Agent Health Loss - Thirst", 2f, new ConfigDescription(DESCRIPTION_AGENT_HEALTH_LOSS_THIRST, new AcceptableValueRange<float>(0, 100)));
            AgentFoodLossStomachache = Config.Bind(SECTION_LOSS, "Agent Food Loss - Stomachache", 2f, new ConfigDescription(DESCRIPTION_AGENT_FOOD_LOSS_STOMACHACHE, new AcceptableValueRange<float>(0, 100)));
            AgentStaminaLossOverwork = Config.Bind(SECTION_LOSS, "Agent Stamina Loss - Overwork", 4f, new ConfigDescription(DESCRIPTION_AGENT_STAMINA_LOSS_OVERWORK, new AcceptableValueRange<float>(0, 100)));

            HealthLoss = Config.Bind(SECTION_LOSS, "Health Loss", 2.5f, new ConfigDescription(DESCRIPTION_HEALTH_LOSS, new AcceptableValueRange<float>(0, 100)));
            StaminaLoss = Config.Bind(SECTION_LOSS, "Stamina Loss", 4f, new ConfigDescription(DESCRIPTION_STAMINA_LOSS, new AcceptableValueRange<float>(0, 100)));
            CaloriePool = Config.Bind(SECTION_LOSS, "Calorie Pool", 7500f, new ConfigDescription(DESCRIPTION_CALORIE_POOL, new AcceptableValueRange<float>(2500, 25000)));
            CalorieLoss = Config.Bind(SECTION_LOSS, "Calorie Loss Per Day", 2500f, DESCRIPTION_CALORIE_LOSS);
            WaterPool = Config.Bind(SECTION_LOSS, "Water Pool", 4000f, new ConfigDescription(DESCRIPTION_WATER_POOL, new AcceptableValueRange<float>(2000, 20000)));
            WaterLoss = Config.Bind(SECTION_LOSS, "Water Loss Per Day", 2000f, DESCRIPTION_WATER_LOSS);

            AgentLowFood = Config.Bind(SECTION_PENALTY, "Agent Low Food Threshold", 10f, new ConfigDescription(DESCRIPTION_AGENT_LOW_FOOD, new AcceptableValueRange<float>(0, 100)));
            AgentHealthRate = Config.Bind(SECTION_RECOVER, "Agent Health Recovery (per Game-Hour)", 3f, DESCRIPTION_AGENT_HEALTH_RATE);
            HealthRate = Config.Bind(SECTION_RECOVER, "Player Health Recovery (per Game-Hour)", 3f, DESCRIPTION_HEALTH_RATE);
            StaminaRate = Config.Bind(SECTION_RECOVER, "Stamina Recovery (per Game-Hour)", 10f, new ConfigDescription(DESCRIPTION_STAMINA_RATE, new AcceptableValueRange<float>(0, 100)));
            CalorieRate = Config.Bind(SECTION_RECOVER, "Calorie Multiplier", 100f, new ConfigDescription(DESCRIPTION_CALORIE_RATE, new AcceptableValueRange<float>(0, 500)));
            WaterRate = Config.Bind(SECTION_RECOVER, "Water Multiplier", 100f, new ConfigDescription(DESCRIPTION_WATER_RATE, new AcceptableValueRange<float>(0, 500)));

            AgentLowFood = Config.Bind(SECTION_PENALTY, "Agent Low Food Threshold", 0f, new ConfigDescription(DESCRIPTION_AGENT_LOW_FOOD, new AcceptableValueRange<float>(0, 100)));
            AgentLowWater = Config.Bind(SECTION_PENALTY, "Agent Low Water Threshold", 0f, new ConfigDescription(DESCRIPTION_AGENT_LOW_WATER, new AcceptableValueRange<float>(0, 100)));
            LowFood = Config.Bind(SECTION_PENALTY, "Low Food Threshold", 0f, new ConfigDescription(DESCRIPTION_LOW_FOOD, new AcceptableValueRange<float>(0, 100)));
            LowWater = Config.Bind(SECTION_PENALTY, "Low Water Threshold", 0f, new ConfigDescription(DESCRIPTION_LOW_WATER, new AcceptableValueRange<float>(0, 100)));

            LowStamina = Config.Bind(SECTION_PENALTY, "Low Stamina Threshold", 0f, new ConfigDescription(DESCRIPTION_LOW_STAMINA, new AcceptableValueRange<float>(0, 100)));
            AgentReviveReset = Config.Bind(SECTION_PENALTY, "Agent Revive Reset", false, DESCRIPTION_AGENT_REVIVE_RESET);
            AgentRevivePenalty = Config.Bind(SECTION_PENALTY, "Agent Revive Penalty", true, DESCRIPTION_AGENT_REVIVE_PENALTY);
            Permadeath = Config.Bind(SECTION_PENALTY, "Permadeath", false, DESCRIPTION_PERMADEATH);
            PlayerDeathReset = Config.Bind(SECTION_PENALTY, "Player Death Agent Reset", true, DESCRIPTION_PLAYER_DEATH_RESET);          

            HealthWarn = Config.Bind(SECTION_WARN, "Health Warning", 30, new ConfigDescription(DESCRIPTION_HEALTH_WARN, new AcceptableValueRange<int>(0, 100)));
            AgentWarn = Config.Bind(SECTION_WARN, "Agent Health Warning", 30, new ConfigDescription(DESCRIPTION_AGENT_WARN, new AcceptableValueRange<int>(0, 100)));
            FoodWarn = Config.Bind(SECTION_WARN, "Food Warning", 20, new ConfigDescription(DESCRIPTION_FOOD_WARN, new AcceptableValueRange<int>(0, 100)));
            WaterWarn = Config.Bind(SECTION_WARN, "Water Warning", 20, new ConfigDescription(DESCRIPTION_WATER_WARN, new AcceptableValueRange<int>(0, 100)));
            StaminaWarn = Config.Bind(SECTION_WARN, "Stamina Warning", 20, new ConfigDescription(DESCRIPTION_STAMINA_WARN, new AcceptableValueRange<int>(0, 100)));

            CharacterApi.RegisterExtraBehaviour<LifeStatsController>(BEHAVIOR);
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(HardcoreMode));
        }
    }
}
