using Core;
using DialogueSystem.Scripts;
using UnityEngine;
using Utility.Tooltip;

namespace Assets.NPCs.Scripts
{
    public class NPC : MonoBehaviour
    {
        public string npcName;

        protected bool inRange;

        protected void Start()
        {
            inRange = false;
        }

        protected void Update()
        {
            if (inRange && GameManager.isPressingInteractButton && !DialogueMenu.IsShown())
            {
                GameManager.GetDialogueManager().StartCorrectDialogue(npcName);
                Tooltip.HideTooltip_Static();
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.ShowTooltip_Static("Press E to talk.");
                inRange = true;
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.HideTooltip_Static();
                inRange = false;
            }
        }
    }
}