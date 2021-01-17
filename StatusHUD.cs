using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HardcoreMode
{
    public class StatusHUD
    {
        private readonly GameObject statusHUD;
        private readonly Image healthBarGuage;
        private readonly Image staminaBarGuage;
        private readonly Image foodBarGuage;
        private readonly Image waterBarGuage;
        private readonly bool initialized = false;

        public StatusHUD(string characterName, Vector3 position)
        {
            initialized = false;

            var commandCanvas = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas");
            if (commandCanvas == null)
                return;

            try
            {
                statusHUD = new GameObject { name = "HardcoreHUD" };
                statusHUD.transform.SetParent(commandCanvas.transform);
                statusHUD.transform.localScale = new Vector3(1, 1, 1);
                statusHUD.transform.localPosition = position;

                var healthBarObject = CreateStatusBar("HealthBar", new Vector3(0, 0, 0), statusHUD.transform);
                healthBarGuage = healthBarObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("HealthBar")).FirstOrDefault();
                healthBarGuage.color = new Color(1, 0, 0, 0.5f);

                var staminaBarObject = CreateStatusBar("StaminaBar", new Vector3(0, -25, 0), statusHUD.transform);
                staminaBarGuage = staminaBarObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("StaminaBar")).FirstOrDefault();
                staminaBarGuage.color = new Color(1, 1, 0, 0.5f);

                var foodBarObject = CreateStatusBar("FoodBar", new Vector3(90, -50, 0), statusHUD.transform);
                foodBarObject.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("FoodBar")).FirstOrDefault().sizeDelta = new Vector2(95, 25);
                foodBarGuage = foodBarObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("FoodBar")).FirstOrDefault();
                foodBarGuage.color = new Color(0, 1, 0, 0.5f);

                var waterBarObject = CreateStatusBar("WaterBar", new Vector3(0, -50, 0), statusHUD.transform);
                waterBarObject.GetComponentsInChildren<RectTransform>(true).Where(x => x.name.Contains("WaterBar")).FirstOrDefault().sizeDelta = new Vector2(95, 25);
                waterBarGuage = waterBarObject.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("WaterBar")).FirstOrDefault();
                waterBarGuage.color = new Color(0, 0, 1, 0.5f);


                if (healthBarGuage != null && staminaBarGuage != null && foodBarGuage != null && waterBarGuage != null)
                    initialized = true;

                var textObject = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/Name/NameLabel");
                var AgentName = UnityEngine.Object.Instantiate(textObject);
                AgentName.transform.SetParent(healthBarObject.transform);
                AgentName.name = "AgentName";
                AgentName.transform.localPosition = new Vector3(92.5f, 2.5f, 0f);
                AgentName.transform.localScale = new Vector3(0.75f, 0.75f, 1);
                AgentName.GetComponentsInChildren<Text>(true).FirstOrDefault().text = characterName;
                AgentName.GetComponentsInChildren<Text>(true).FirstOrDefault().color = Color.black;
                AgentName.GetComponentsInChildren<RectTransform>(true).FirstOrDefault().sizeDelta = new Vector2(180f, 20f);
            }
            catch
            {
                Console.WriteLine($"Hardcore Mode Failed to Initialze HUD for {characterName}");
                Destroy();
                initialized = false;
            }
        }

        public void SetPosition(Vector3 position)
        {
            if (!initialized)
                return;

            statusHUD.transform.localPosition = position;
        }

        public void SetScale(Vector3 scale)
        {
            if (!initialized)
                return;

            statusHUD.transform.localScale = scale;
        }

        public void SetVisible(bool isVisible, bool displayHealth = true, bool displayStats = true)
        {
            if (!initialized)
                return;

            statusHUD.SetActive(isVisible);
            healthBarGuage.gameObject.SetActive(displayHealth);
            staminaBarGuage.gameObject.SetActive(displayStats);
            foodBarGuage.gameObject.SetActive(displayStats);
        }

        public void Update(float health, float stamina, float food, float water)
        {
            if (!initialized || !statusHUD.activeSelf)
                return;

            healthBarGuage.fillAmount = health / 100;
            staminaBarGuage.fillAmount = stamina / 100;
            foodBarGuage.fillAmount = food / 100;
            waterBarGuage.fillAmount = water / 100;
        }

        public void Destroy()
        {
            GameObject.Destroy(statusHUD);
        }

        public GameObject CreateStatusBar(string statusBarName, Vector3 localPosition, Transform parent)
        {
            var statusGuage = GameObject.Find("MapScene/MapUI(Clone)/CommandCanvas/MenuUI(Clone)/CellularUI/Interface Panel/StatusUI(Clone)/Content/PlayerContent/Guage");

            if (statusGuage == null)
                return new GameObject();

            GameObject statusBar = UnityEngine.Object.Instantiate(statusGuage);
            statusBar.transform.SetParent(parent);
            statusBar.name = statusBarName;
            statusBar.transform.localPosition = localPosition;
            statusBar.transform.localScale = new Vector3(1, 1, 1);
            statusBar.GetComponentsInChildren<Image>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().color = new Color(0.25f, 0.25f, 0.25f, 1);
            statusBar.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("Image")).FirstOrDefault().localScale = new Vector3(1f, 1.1f, 1f);

            return statusBar;
        }
    }
}
