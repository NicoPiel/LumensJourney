using System.Collections;
using System.Collections.Generic;
using Assets.Player.Script;
using Assets.SaveSystem;
using Assets.UI.PlayerUI.Scripts;
using Core;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public class BankScript : MonoBehaviour
{
    private BoxCollider2D _interactCollider2D;
    private bool _entered;
    private bool _menuOpen;
    private int storedLightShards;

    public UnityEvent onLightShardsStoredInBank;

    // Start is called before the first frame update
    void Start()
    {
        
        _interactCollider2D = transform.GetComponentInChildren<BoxCollider2D>();
        _entered = false;
        _menuOpen = false;
        
        storedLightShards = GameManager.GetSaveSystem().BankedShards;
        onLightShardsStoredInBank = new UnityEvent();
    }

    public void Update()
    {
        if (_entered && Input.GetKeyDown(KeyCode.E))
        {
            if (_menuOpen)
            {
                GameManager.GetMenuManagerScript().UnloadCurrentMenu();
                _menuOpen = false;
            }
            else
            {
                Tooltip.HideTooltip_Static();
                _menuOpen = true;
                GameManager.GetMenuManagerScript().LoadMenu("BankMenu");
                
            }
        }
        
    }

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Test");
        if (other.gameObject.CompareTag("Player"))
        {
            Tooltip.ShowTooltip_Static("Press E");
            _entered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Test");
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.GetMenuManagerScript().UnloadCurrentMenu();
            _menuOpen = false;
            Tooltip.HideTooltip_Static();
            _entered = false;
        }
    }

    public void StoreAllLightShardsInBank()
    {
        storedLightShards += GameManager.GetPlayerScript().GetLightShardAmount();
        GameManager.GetPlayerScript().PlayerSetLightShards(0);
        GameManager.GetSaveSystem().BankedShards = storedLightShards;
        
        onLightShardsStoredInBank.Invoke();
        
        GameManager.GetSaveSystem().CreateSave();
    }

    public int GetStoredLightShards()
    {
        return storedLightShards;
    }
}