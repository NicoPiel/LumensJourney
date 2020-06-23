using TMPro;
using Unity.Burst;
using UnityEngine;

namespace Utility.Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip _instance;
        
        [SerializeField] private TMP_Text tooltipText;
        [SerializeField] private RectTransform backgroundRectTransform;

        private void Awake()
        {
            _instance = this;
            HideTooltip();
        }
        
        private void ShowTooltip(string tooltipString)
        {
            gameObject.SetActive(true);

            tooltipText.text = tooltipString;
            var backgroundSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
            backgroundRectTransform.sizeDelta = backgroundSize;
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