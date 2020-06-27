using Core;
using TMPro;
using UnityEngine;

namespace Assets.UI.LightShards.Script
{
    public class LightShardDisplayScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text textBox;

        private void Start()
        {
            textBox.text = GameManager.GetPlayerScript().GetPlayerInventory().GetLightShardAmount().ToString();
            GameManager.GetEventHandler().onPlayerLightShardsChanged.AddListener(UpdateLightShardDisplay);
        }

        private void UpdateLightShardDisplay()
        {
            textBox.text = GameManager.GetPlayerScript().GetPlayerInventory().GetLightShardAmount().ToString();
        }
    }
}
