using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Player.Script;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.MenuManager.Scripts
{
    public class DialogueMenu : MonoBehaviour
    {
        private static DialogueMenu _instance;

        public static UnityEvent onDialogueShown;
        public static UnityEvent onDialogueHidden;
        public static UnityEvent onStartOfLine;
        public static UnityEvent onEndOfLine;

        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TMP_Text namePlate;
        [SerializeField] private TMP_Text dialogueText;

        public bool shown;
        public bool endOfLine;

        private GameObject _dialogueMenu;
        private Transform _dialogueMenuTransform;

        private void Awake()
        {
            _instance = this;

            onDialogueShown = new UnityEvent();
            onDialogueHidden = new UnityEvent();
            onStartOfLine = new UnityEvent();
            onEndOfLine = new UnityEvent();
        }

        private void Start()
        {
            namePlate.text = string.Empty;
            dialogueText.text = string.Empty;

            _dialogueMenu = this.gameObject;
            _dialogueMenuTransform = GetComponent<RectTransform>();
            
            onStartOfLine.AddListener(OnStartOfLine);
            onEndOfLine.AddListener(OnEndOfLine);
        }

        public static void ShowDialogueWindow()
        {
            if (IsShown()) return;

            GameManager.GetPlayerScript().FreezeControls();

            _instance._dialogueMenuTransform.LeanMoveY(90, 0.5f)
                .setOnComplete(() =>
                {
                    _instance.shown = true;
                    onDialogueShown.Invoke();
                });
        }

        public static void HideDialogueWindow()
        {
            if (!IsShown()) return;

            _instance._dialogueMenuTransform.LeanMoveY(-180, 0.5f)
                .setOnComplete(() =>
                {
                    GameManager.GetPlayerScript().UnfreezeControls();
                    _instance.shown = false;
                    onDialogueHidden.Invoke();
                });
        }

        public static IEnumerator PrintLineToBox(string line)
        {
            onStartOfLine.Invoke();
            var builder = new StringBuilder();

            foreach (var c in line)
            {
                builder.Append(c);
                _instance.dialogueText.text = builder.ToString();
                yield return new WaitForSeconds(0.04f);
            }

            onEndOfLine.Invoke();
        }

        public void OnStartOfLine()
        {
            endOfLine = false;
        }

        public void OnEndOfLine()
        {
            endOfLine = true;
        }

        public static bool AtEndOfLine()
        {
            return _instance.endOfLine;
        }

        public static bool IsShown()
        {
            return _instance.shown;
        }

        public static void SetNamePlate(string text)
        {
            _instance.namePlate.text = text;
        }

        public static string GetNamePlate()
        {
            return _instance.namePlate.text;
        }

        public static void SetText(string text)
        {
            _instance.dialogueText.text = text;
        }

        public static string GetText()
        {
            return _instance.dialogueText.text;
        }
    }
}