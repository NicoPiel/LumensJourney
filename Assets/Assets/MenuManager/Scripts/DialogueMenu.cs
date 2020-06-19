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

        public UnityEvent onDialogueShown;
        public UnityEvent onDialogueHidden;

        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TMP_Text namePlate;
        [SerializeField] private TMP_Text dialogueText;

        public bool shown;

        private GameObject _dialogueMenu;
        private Transform _dialogueMenuTransform;

        private void Awake()
        {
            _instance = this;
            
            onDialogueShown = new UnityEvent();
        }

        private void Start()
        {
            namePlate.text = string.Empty;
            dialogueText.text = string.Empty;

            _dialogueMenu = this.gameObject;
            _dialogueMenuTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (IsShown() && Input.GetMouseButtonDown(0)) HideDialogueWindow();
        }

        public static void ShowDialogueWindow()
        {
            if (IsShown()) return;
            
            GameManager.GetPlayerScript().FreezeControls();

            _instance._dialogueMenuTransform.LeanMoveY(90, 0.5f)
                .setOnComplete(() =>
                {
                    _instance.shown = true;
                    _instance.onDialogueShown.Invoke();
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
                    _instance.onDialogueHidden.Invoke();
                });
        }

        public static IEnumerator PrintLineToBox(string line)
        {
            var builder = new StringBuilder();
            
            foreach (var c in line)
            {
                builder.Append(c);
                _instance.dialogueText.text = builder.ToString();
                yield return new WaitForSeconds(0.1f);
            }
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