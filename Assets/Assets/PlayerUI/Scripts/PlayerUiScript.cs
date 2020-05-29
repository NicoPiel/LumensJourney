using System;
using Assets.Healthbar.Scripts;
using Assets.ItemBar.Scripts;
using Resources.LightBar.Scripts;
using Unity.Burst;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Utility;

namespace Assets.PlayerUI.Scripts
{
    [BurstCompile]
    public class PlayerUiScript : MonoBehaviour
    {
        public GameObject healthbar;

        public GameObject itembar;

        public GameObject lightbar;

        public GameObject lightShardCounter;

        public GameObject levelTooltip;

        private static PlayerUiScript _instance;
        // Start is called before the first frame update

        public void Awake()
        {
            _instance = this;
        }

        public HealthbarScript GetHealthBarScript()
        {
            return healthbar.GetComponent<HealthbarScript>();
        }

        public ItemBarScript GetItemBarScript()
        {
            return itembar.GetComponent<ItemBarScript>();
        }

        public LightBarScript GetLightBarScript()
        {
            return lightbar.GetComponent<LightBarScript>();
        }

        public LightShardDisplayScript GetLightShardDisplayScript()
        {
            return lightShardCounter.GetComponent<LightShardDisplayScript>();
        }

        public GameObject GetTooltip()
        {
            return levelTooltip;
        }

        public static PlayerUiScript GetPlayerUiScript()
        {
            return _instance;
        }
    }
}