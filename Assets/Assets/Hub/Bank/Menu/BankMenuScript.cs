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
            if (_bankScript == null)
            {
                _bankScript =  GameObject.Find("Bank").GetComponent<BankScript>();
                _bankScript.onLightShardsStoredInBank.AddListener(OnLightShardsChanged);
            }
        
            shardsOnPlayer.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
            shardsInBank.text = _bankScript.GetStoredLightShards().ToString();
        }

        private void OnLightShardsChanged()
        {
            shardsOnPlayer.text = GameManager.GetPlayerScript().GetLightShardAmount().ToString();
            shardsInBank.text = _bankScript.GetStoredLightShards().ToString();
        }

        public void OnStoredButtonPressed()
        {
            _bankScript.StoreAllLightShardsInBank();
        }
    }
}
