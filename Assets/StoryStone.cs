using System.Collections.Generic;
using Assets.MenuManager.Scripts;
using Core;
using UnityEngine;
using Utility.Tooltip;

public class StoryStone : MonoBehaviour
{
    public string npcName;

    private bool _inRange;
    private bool _isRead;

    private void Start()
    {
        _inRange = false;
    }

    private void Update()
    {
        if (_inRange && Input.GetKeyDown(KeyCode.E) && !DialogueMenu.IsShown())
        {
            if (!_isRead)
            {
                GameManager.GetDialogueManager().StartNextStoryStone();
                _isRead = true;
            }
            else
            {
                StartCoroutine(GameManager.GetDialogueManager()
                    .StartDialogue("Ancient Stone", new List<string>
                    {
                        "You have already read this inscription.",
                    }));
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
