using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Assets.MenuManager.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem.Scripts
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager _instance;

        private const string PathToDialogueFile = "Assets/Scripts/DialogueSystem/Data/dialogues.xml";

        private XElement _dialoguesXml;
        private bool _inDialogue;

        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        private void Awake()
        {
            onDialogueStart = new UnityEvent();
            onDialogueEnd = new UnityEvent();
        }

        private void Start()
        {
            _dialoguesXml = XElement.Load(PathToDialogueFile);
        }

        public IEnumerator StartDialogue(string npcName, string flag)
        {
            _inDialogue = true;

            Dialogue dialogue = BuildDialogueWithFlag(npcName, flag);

            //Debug.Log($"Dialogue found:\n {dialogue.ToString()}");

            DialogueMenu.SetNamePlate(npcName);
            DialogueMenu.ShowDialogueWindow();

            Debug.Log($"Show dialogue for {npcName}");

            while (dialogue.HasNextLine())
            {
                var newLine = dialogue.NextLine();
                
                StartCoroutine(DialogueMenu.PrintLineToBox(newLine));
                Debug.Log($"Showing line: {newLine}");

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                yield return new WaitForEndOfFrame();
            }

            DialogueMenu.HideDialogueWindow();
            Debug.Log("Hide dialogue window.");
        }

        public IEnumerator StartDialogue(string npcName, int index)
        {
            yield return null;
        }

        public IEnumerator StartDialogue(string npcName, int index, string flag)
        {
            yield return null;
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

        #endregion
    }
}