using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Assets.MenuManager.Scripts;
using Assets.SaveSystem;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem.Scripts
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager _instance;

        private Dictionary<string, Dictionary<string, bool>> _flags;
        
        private string _persistentPathToFile;

        private XElement _dialoguesXml;
        private bool _inDialogue;

        private GameManager _gameManager;
        private SaveSystem _saveSystem;

        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        private void Awake()
        {
            onDialogueStart = new UnityEvent();
            onDialogueEnd = new UnityEvent();
        }

        private void Start()
        {
            _gameManager = GameManager.GetGameManager();
            _saveSystem = GameManager.GetSaveSystem();
            
            _persistentPathToFile = GameManager.persistentDialogueFilePath;

            _dialoguesXml = XElement.Load(_persistentPathToFile);

            _gameManager.onGameLoaded.AddListener(OnGameLoaded);
            _gameManager.onNewGameStarted.AddListener(OnNewGameStarted);

            _saveSystem.onBeforeSave.AddListener(OnBeforeSave);
        }

        public void StartCorrectDialogue(string npcName)
        {
            var nextFlag = NextFlag(npcName);

            StartCoroutine(StartDialogue(npcName, nextFlag));
        }

        private IEnumerator StartDialogue(string npcName, string flag)
        {
            _inDialogue = true;

            Dialogue dialogue = BuildDialogueWithFlag(npcName, flag);

            //Debug.Log($"Dialogue found:\n {dialogue.ToString()}");

            DialogueMenu.SetNamePlate(npcName);
            DialogueMenu.ShowDialogueWindow();

            //Debug.Log($"Show dialogue for {npcName}");

            while (dialogue.HasNextLine())
            {
                var newLine = dialogue.NextLine();

                StartCoroutine(DialogueMenu.PrintLineToBox(newLine));
                //Debug.Log($"Showing line: {newLine}");

                yield return new WaitUntil(DialogueMenu.AtEndOfLine);
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                yield return new WaitForEndOfFrame();
            }

            DialogueMenu.HideDialogueWindow();
            //Debug.Log("Hide dialogue window.");
        }

        public IEnumerator StartDialogue(string npcName, int index)
        {
            yield return null;
        }

        public IEnumerator StartDialogue(string npcName, int index, string flag)
        {
            yield return null;
        }

        private string NextFlag(string npcName)
        {
            var currentFlag =
                (from flags in _flags[npcName]
                    where flags.Value
                    select flags.Key).First();

            var newFlag =
                (from dialogue in _dialoguesXml.Elements(npcName).Elements()
                    where dialogue.Attribute("flag")?.Value == currentFlag
                    select dialogue.Attribute("newFlag")?.Value).First();

            //Debug.Log($"Current flag for {npcName}: {currentFlag}");

            if (currentFlag != "default")
            {
                _flags[npcName][currentFlag] = false;
                
                if (newFlag != null)
                {
                    if (_flags[npcName].ContainsKey(newFlag)) _flags[npcName][newFlag] = true;
                    else _flags[npcName]["default"] = true;
                    
                    //Debug.Log($"New flag for {npcName}: {newFlag}");
                }
                else _flags[npcName]["default"] = true;
            }

            SaveFlags();

            return currentFlag;
        }

        public void SetFlag(string npcName, string flag, bool value)
        {
            if (_flags[npcName].ContainsKey(flag)) _flags[npcName][flag] = value;
            SaveFlags();
        }

        private bool IsFlagSet(string npcName, string flag)
        {
            return _flags[npcName].ContainsKey(flag) && _flags[npcName][flag];
        }

        private void SaveFlags()
        {
            _saveSystem.SaveFlags(_flags);
            _saveSystem.CreateSave();
        }

        #region Build dialogues

        private List<Dialogue> GetAllDialoguesFromNpc(string npcName)
        {
            var dialogueList = new List<Dialogue>();
            XElement root = _dialoguesXml;

            var dialogues =
                from element in root.Elements(npcName)
                select element;

            foreach (XElement dialogue in dialogues)
            {
                var lines =
                    (from line in dialogue.Elements()
                        select line.Value).ToList();

                dialogueList.Add(new Dialogue(lines));
            }

            return dialogueList;
        }

        private Dialogue BuildDialogueWithFlag(string npcName, string flag)
        {
            //Debug.Log($"Retrieving dialogue for {npcName} with flag: {flag}");

            XElement root = _dialoguesXml;

            XElement dialogue =
                (from element in root.Elements(npcName).Elements()
                    where element.Attribute("flag")?.Value == flag
                    select element).First();

            var lines =
                (from line in dialogue.Elements("line")
                    select line.Value).ToList();

            return new Dialogue(lines, flag);
        }

        private Dialogue BuildDialogueWithIndex(string npcName, int index)
        {
            XElement root = _dialoguesXml;

            var dialogues =
                from element in root.Elements(npcName).Elements()
                where int.Parse(element.Attribute("index")?.Value
                                ?? throw new ArgumentException($"Dialogue {element.ToString()} has no index attribute."))
                      == index
                select element;

            var lines =
                (from line in dialogues.Elements("line")
                    select line.Value).ToList();


            return new Dialogue(lines, index);
        }

        private Dialogue BuildDialogueWithIndexAndFlag(string npcName, int index, string flag)
        {
            XElement root = _dialoguesXml;

            var dialogues =
                from element in root.Elements(npcName).Elements()
                where int.Parse(element.Attribute("index")?.Value
                                ?? throw new ArgumentException($"Dialogue {element.ToString()} has no index attribute."))
                    == index && element.Attribute("flag")?.Value == flag
                select element;

            var lines =
                (from line in dialogues.Elements("line")
                    select line.Value).ToList();


            return new Dialogue(lines, index, flag);
        }

        private Dialogue GetDefaultDialog(string npcName)
        {
            return BuildDialogueWithFlag(npcName, "default");
        }

        #endregion

        #region Event subscriptions

        private void OnBeforeSave()
        {
            _saveSystem.SaveFlags(_flags);
        }

        private void OnNewGameStarted()
        {
            _flags = new Dictionary<string, Dictionary<string, bool>>();

            var npcs =
                from npc in _dialoguesXml.Elements()
                select npc;

            foreach (XElement npc in npcs)
            {
                var flagDict = new Dictionary<string, bool>();

                var flags =
                    from dialogue in npc.Elements("dialogue")
                    select dialogue.Attribute("flag")?.Value.ToString();

                foreach (var flag in flags)
                {
                    if (flag == "first" || flag == "default") flagDict.Add(flag, true);
                    else flagDict.Add(flag, false);
                }

                _flags.Add(npc.Name.ToString(), flagDict);
            }

            _saveSystem.SaveFlags(_flags);
            _saveSystem.CreateSave();
        }

        private void OnGameLoaded()
        {
            _flags = _saveSystem.DialogueFlags;
        }

        #endregion
    }
}