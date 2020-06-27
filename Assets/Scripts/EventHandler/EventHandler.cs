using System.Collections;
using System.Collections.Generic;
using Assets.Items.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    //Imported from Playerscript:
    public class OnPlayerStatChanged : UnityEvent<string>{}
    public class OnItemAddedToInventory: UnityEvent<GameItem>{}
    
    public UnityEvent onPlayerTakeDamage;
    public UnityEvent onPlayerLifeChanged;
    public UnityEvent onPlayerTakeHeal;
    public UnityEvent onInventoryChanged;
    public UnityEvent onPlayerLightLevelChanged;
    public UnityEvent onPlayerLightShardsChanged;
    public UnityEvent onPlayerDied;
    public OnPlayerStatChanged onPlayerStatChanged;
    public OnItemAddedToInventory onItemAddedToInventory;
    
    //Imported from GameManager:
    public UnityEvent onNewGameStarted;
    public UnityEvent onGameLoaded;
    public UnityEvent onPlayerSpawned;
    public UnityEvent onHubEntered;
    public UnityEvent onNewLevel;
    
    //Imported from SaveSystem:
    public UnityEvent onBeforeSave;
    public UnityEvent onGameSaved;
    
    //Imported from DialogueManager:
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;
    
    //Imported from Enemy:
    public UnityEvent onEnemyDeath;
    public UnityEvent onEnemyDamageTaken;
    
    //Imported from Generator:
    public UnityEvent onDungeonGenerated;
    public UnityEvent onDungeonChanged;
    
    //Imported from BankMenu:
    public UnityEvent onLightShardsStoredInBank;
    public UnityEvent onChestClosed;
    public UnityEvent onChestOpened;
    // Start is called before the first frame update
    void Awake()
    {
        onPlayerTakeDamage = new UnityEvent();
        onPlayerLifeChanged = new UnityEvent();
        onPlayerTakeHeal = new UnityEvent();
        onInventoryChanged = new UnityEvent();
        onPlayerLightLevelChanged = new UnityEvent();
        onPlayerLightShardsChanged = new UnityEvent();
        onPlayerDied = new UnityEvent();
        onPlayerStatChanged = new OnPlayerStatChanged();
        onItemAddedToInventory = new OnItemAddedToInventory();
        
        onGameLoaded = new UnityEvent();
        onNewGameStarted = new UnityEvent();
        onNewLevel = new UnityEvent();
        onHubEntered = new UnityEvent();
        onPlayerSpawned = new UnityEvent();
        
        onBeforeSave = new UnityEvent();
        onGameSaved = new UnityEvent();
        
        onDialogueStart = new UnityEvent();
        onDialogueEnd = new UnityEvent();
        
        onDungeonGenerated = new UnityEvent();
        onDungeonChanged = new UnityEvent();
        
        onEnemyDeath = new UnityEvent();
        onEnemyDamageTaken = new UnityEvent();
        
        onLightShardsStoredInBank = new UnityEvent();
        onChestClosed = new UnityEvent();
        onChestOpened = new UnityEvent();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
