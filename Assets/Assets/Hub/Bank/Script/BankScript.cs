using Core;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Assets.Hub.Bank.Script
{
    public class BankScript : MonoBehaviour
    {
        private BoxCollider2D _interactCollider2D;
        private SpriteRenderer _spriteRenderer;
        private bool _entered;
        private bool _menuOpen;
        private int _storedLightShards;

        public Sprite openChest;
        public Sprite closedChest;

        public UnityEvent onLightShardsStoredInBank;

        // Start is called before the first frame update
        void Start()
        {
        
            _interactCollider2D = transform.GetComponentInChildren<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _entered = false;
            _menuOpen = false;
        
            _storedLightShards = GameManager.GetSaveSystem().BankedShards;
            onLightShardsStoredInBank = new UnityEvent();
        }

        public void Update()
        {
            if (_entered && Input.GetKeyDown(KeyCode.E))
            {
                if (_menuOpen)
                {
                    GameManager.GetMenuManagerScript().UnloadCurrentMenu();
                    _spriteRenderer.sprite = closedChest;
                    _menuOpen = false;
                }
                else
                {
                    Tooltip.HideTooltip_Static();
                    _menuOpen = true;
                    _spriteRenderer.sprite = openChest;
                    GameManager.GetMenuManagerScript().LoadMenu("BankMenu");
                }
            }

        }

        // Update is called once per frame
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Tooltip.ShowTooltip_Static("Press E");
                _entered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameManager.GetMenuManagerScript().UnloadCurrentMenu();
                _menuOpen = false;
                _spriteRenderer.sprite = closedChest;
                Tooltip.HideTooltip_Static();
                _entered = false;
            }
        }

        public void StoreAllLightShardsInBank()
        {
            _storedLightShards += GameManager.GetPlayerScript().GetLightShardAmount();
            GameManager.GetPlayerScript().PlayerSetLightShards(0);
            GameManager.GetSaveSystem().BankedShards = _storedLightShards;
        
            onLightShardsStoredInBank.Invoke();
        
            GameManager.GetSaveSystem().CreateSave();
        }

        public int GetStoredLightShards()
        {
            return _storedLightShards;
        }
    }
}