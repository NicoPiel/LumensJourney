using System.Collections;
using Assets.UI.PlayerUI.Scripts;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Utility.Tooltip;

namespace Assets.NPCs.Scripts
{
    public class Guardian : MonoBehaviour
    {
        public string npcName;

        protected bool inRange;

        private Transform _playerTransform;
        
        private new void Start()
        {
            inRange = false;
            GameManager.GetDialogueManager().onDialogueEnd.AddListener(OnDialogueEnd);
            _playerTransform = GameManager.GetPlayer().transform;
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            inRange = true;
            GameManager.GetDialogueManager().StartCorrectDialogue(npcName);
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.HideTooltip_Static();
                inRange = false;
            }
        }

        private void OnDialogueEnd()
        {
            if (inRange)
            {
                GameManager.cameFromGuardian = true;
                GameManager.GetSaveSystem().RunsCompleted++;
                
                StartCoroutine(Methods.LoadYourSceneAsync("Hub"));
                GameManager.GetPlayerScript().transform.position = new Vector3(-2, -2, 0);
            }
        }
    }
}
