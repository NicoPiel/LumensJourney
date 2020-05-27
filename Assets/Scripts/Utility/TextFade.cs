using System.Collections;
using TMPro;
using Unity.Burst;
using UnityEngine;

namespace Utility
{
    [BurstCompile]
    public class TextFade : MonoBehaviour
    {
        public static IEnumerator FadeTextToFullAlpha(float t, TMP_Text text)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            while (text.color.a < 1.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }

        public static IEnumerator FadeTextToZeroAlpha(float t, TMP_Text text)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            while (text.color.a > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
    }
}