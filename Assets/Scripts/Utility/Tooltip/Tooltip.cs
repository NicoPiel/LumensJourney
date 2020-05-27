using TMPro;
using Unity.Burst;
using UnityEngine;

namespace Utility
{
    [BurstCompile]
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip _instance;
        
        private Camera _uiCamera;
        private TMP_Text _tooltipText;
        private RectTransform _backgroundRectTransform;

        private void Awake()
        {
            _instance = this;
            
            _backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
            _tooltipText = transform.Find("text").GetComponent<TMP_Text>();
            HideTooltip();
        }
        
        private void ShowTooltip(string tooltipString)
        {
            _uiCamera = Camera.main;
            gameObject.SetActive(true);

            _tooltipText.text = tooltipString;
            var backgroundSize = new Vector2(_tooltipText.preferredWidth, _tooltipText.preferredHeight);
            _backgroundRectTransform.sizeDelta = backgroundSize;
        }

        private void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        public static void ShowTooltip_Static(string tooltipString)
        {
            _instance.ShowTooltip(tooltipString);
        }

        public static void HideTooltip_Static()
        {
            _instance.HideTooltip();
        }
    }
}