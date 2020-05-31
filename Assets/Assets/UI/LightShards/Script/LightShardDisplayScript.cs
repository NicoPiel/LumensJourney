using Core;
using TMPro;
using UnityEngine;

namespace Assets.UI.LightShards.Script
{
    public class LightShardDisplayScript : MonoBehaviour
    {
        private TMP_Text _textbox;

        public void Awake()
        {
            _textbox = gameObject.GetComponentInChildren<TMP_Text>();
        
        }

        public void Start()
        {
            _textbox.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
            GameManager.GetPlayerScript().onPlayerLightShardsChanged.AddListener(UpdateLightShardDisplay);
        }

        public void UpdateLightShardDisplay()
        {
            _textbox.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
        }
    }
}
