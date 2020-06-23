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

        private string _persistentPathToDialogueFile;
        private string _persistentPathToObjectsFile;

        private XElement _dialoguesXml;
        private XElement _objectsXml;
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

            _persistentPathToDialogueFile = GameManager.persistentDialogueFilePath;
            _dialoguesXml = XElement.Load(_persistentPathToDialogueFile);
            _persistentPathToObjectsFile = GameManager.persistentObjectsFilePath;
            _objectsXml = XElement.Load(_persistentPathToObjectsFile);
            if (_dialoguesXml != null) Debug.Log($"Loaded dialogues.xml from {_persistentPathToDialogueFile}");
            else throw new NullReferenceException("diaolgues.xml could not be found.");
            if (_objectsXml != null) Debug.Log($"Loaded storyobjects.xml from {_persistentPathToObjectsFile}");
            else throw new NullReferenceException("storyobjects.xml could not be found.");

            _gameManager.onGameLoaded.AddListener(OnGameLoaded);
            _gameManager.onNewGameStarted.AddListener(OnNewGameStarted);

            _saveSystem.onBeforeSave.AddListener(OnBeforeSave);
        }

        public void StartCorrectDialogue(string npcName)
        {
            var nextFlag = NextFlag(npcName);

            StartCoroutine(StartDialogue(npcName, nextFlag));
        }

        public void StartNextStoryStone()
        {
            StartCoroutine(StartDialogueStoryStone(_saveSystem.StoryStoneProgression));
            
            // Get the next stone next time.
            _saveSystem.StoryStoneProgression++;
        }

        private IEnumerator StartDialogue(string npcName, string flag)
        {
            _inDialogue = true;
            onDialogueStart.Invoke();

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
            onDialogueEnd.Invoke();
            //Debug.Log("Hide dialogue window.");
        }
        
        public IEnumerator StartDialogue(string npcName, int index)
        {
            throw new NotImplementedException("TODO");
        }

        public IEnumerator StartDialogue(string npcName, int index, string flag)
        {
            throw new NotImplementedException("TODO");
        }
        
        public IEnumerator StartDialogue(string name, List<string> lines)
        {
            _inDialogue = true;
            onDialogueStart.Invoke();

            var dialogue = new Dialogue(lines);

            //Debug.Log($"Dialogue found:\n {dialogue.ToString()}");

            DialogueMenu.SetNamePlate(name);
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
            onDialogueEnd.Invoke();
            //Debug.Log("Hide dialogue window.");
        }
        
        private IEnumerator StartDialogueStoryStone(int storyStoneindex)
        {
            _inDialogue = true;
            onDialogueStart.Invoke();

            Dialogue dialogue = BuildDialogueStoryStone(storyStoneindex);

            //Debug.Log($"Dialogue found:\n {dialogue.ToString()}");

            DialogueMenu.SetNamePlate("Ancient Stone");
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
            onDialogueEnd.Invoke();
            //Debug.Log("Hide dialogue window.");
        }

        private IEnumerator StartDialogueDiaryEntry(int diaryEntryindex)
        {
            throw new NotImplementedException("TODO");
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

            Debug.Log($"Current flag for {npcName}: {currentFlag}");

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
                      == index
                      && element.Attribute("flag")?.Value == flag
                select element;

            var lines =
                (from line in dialogues.Elements("line")
                    select line.Value).ToList();

            return new Dialogue(lines, index, flag);
        }

        private Dialogue BuildDialogueStoryStone(int index)
        {
            XElement stone =
                (from element in _objectsXml.Elements("stones").Elements("stone")
                    where int.Parse(element.Attribute("index")?.Value
                                    ?? throw new ArgumentException("Storystone doesn't have an index"))
                          == index
                          && int.Parse(element.Attribute("req")?.Value
                                       ?? "0")
                          <= _saveSystem.RunsCompleted
                    select element).First();
            
            var lines = new List<string>();

            if (stone != null && !stone.IsEmpty)
            {
                lines =
                    (from line in stone.Elements("line")
                        select line.Value).ToList();
            }
            else
            {
                lines.Add("There is is something written on this stone, but you can't decipher it.");
            }

            return new Dialogue(lines, index);
        }

        private Dialogue BuildDialogueDiaryEntry(int index)
        {
            throw new NotImplementedException("TODO");
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
        }

        private void OnGameLoaded()
        {
            _flags = _saveSystem.DialogueFlags;
        }

        #endregion
    }
}