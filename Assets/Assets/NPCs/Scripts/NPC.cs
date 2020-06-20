using Assets.MenuManager.Scripts;
using Core;
using UnityEngine;
using Utility;

namespace Assets.NPCs.Scripts
{
    public class NPC : MonoBehaviour
    {
        public string npcName;

        private bool _inRange;

        private void Start()
        {
            _inRange = false;
        }

        private void Update()
        {
            if (_inRange && Input.GetKeyDown(KeyCode.E) && !DialogueMenu.IsShown())
            {
                GameManager.GetDialogueManager().StartCorrectDialogue(npcName);

                Tooltip.HideTooltip_Static();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.ShowTooltip_Static("Press E to talk.");
                _inRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.HideTooltip_Static();
                _inRange = false;
            }
        }
    }
}