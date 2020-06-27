using Assets.Hub.Bank.Script;
using Core;
using TMPro;
using UnityEngine;

namespace Assets.Hub.Bank.Menu
{
    public class BankMenuScript : MonoBehaviour
    {
        public TMP_Text shardsOnPlayer;
        public TMP_Text shardsInBank;
        private BankScript _bankScript;

        private void OnEnable()
        {
            GetComponent<RectTransform>().localScale = Vector3.zero;
            LeanTween.scale(GetComponent<RectTransform>(), new Vector3(0.5f, 0.5f, 1), 0.2f);
            if (_bankScript == null)
            {
                _bankScript =  GameObject.Find("Bank").GetComponent<BankScript>();
                GameManager.GetEventHandler().onLightShardsStoredInBank.AddListener(OnLightShardsChanged);
            }
        
            shardsOnPlayer.text = GameManager.GetPlayerScript().GetPlayerInventory().GetLightShardAmount().ToString();
            shardsInBank.text = _bankScript.GetStoredLightShards().ToString();
        }

        private void OnLightShardsChanged()
        {
            shardsOnPlayer.text = GameManager.GetPlayerScript().GetPlayerInventory().GetLightShardAmount().ToString();
            shardsInBank.text = _bankScript.GetStoredLightShards().ToString();
        }

        public void OnStoredButtonPressed()
        {
            _bankScript.StoreAllLightShardsInBank();
        }
    }
}
