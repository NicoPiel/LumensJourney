using System;
using Assets.UI.Healthbar.Scripts;
using Assets.UI.ItemBar.Scripts;
using Assets.UI.LightBar.Scripts;
using Assets.UI.LightShards.Script;
using Unity.Burst;
using UnityEngine;

namespace Assets.UI.PlayerUI.Scripts
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

        public GameObject GetLevelTooltip()
        {
            return levelTooltip;
        }

        
        public static PlayerUiScript GetPlayerUiScript()
        {
            return _instance;
        }
        
    }
}