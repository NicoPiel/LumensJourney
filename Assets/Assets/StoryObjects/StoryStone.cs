using Core;
using DialogueSystem.Scripts;
using UnityEngine;
using Utility.Tooltip;

namespace Assets.StoryObjects
{
    public class StoryStone : MonoBehaviour
    {
        private bool _inRange;
        private bool _isRead;

        private void Start()
        {
            _inRange = false;
        }

        private void Update()
        {
            if (_inRange && GameManager.isPressingInteractButton && !DialogueMenu.IsShown())
            {
                if (!_isRead)
                {
                    if (GameManager.GetDialogueManager().StartNextStoryStone()) _isRead = true;
                }
                else
                {
                    GameManager.GetDialogueManager().StartCurrentStoryStone();
                }

                Tooltip.HideTooltip_Static();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.ShowTooltip_Static("Press E to read.");
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
