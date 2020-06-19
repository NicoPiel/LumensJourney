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

        private XElement _dialoguesXML;
        private bool _inDialogue;
        private bool _onHold;

        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        private void Awake()
        {
            onDialogueStart = new UnityEvent();
            onDialogueEnd = new UnityEvent();
        }

        private void Start()
        {
            _dialoguesXML = XElement.Load(PathToDialogueFile);
        }

        public async Task StartDialogue(string npcName, string flag)
        {
            _inDialogue = true;
            _onHold = false;

            Dialogue dialogue = BuildDialogueWithFlag(npcName, flag);
            
            //Test
            DialogueMenu.SetNamePlate(npcName);
            //
            DialogueMenu.ShowDialogueWindow();

            while (dialogue.HasNextLine())
            {
                if (!_onHold)
                {
                    StartCoroutine(DialogueMenu.PrintLineToBox(dialogue.NextLine()));
                    _onHold = true;
                }

                await Task.Yield();
            }
            
            DialogueMenu.HideDialogueWindow();
        }
        
        public IEnumerator StartDialogue(string npcName, int index)
        {
            yield return null;
        }
        
        public IEnumerator StartDialogue(string npcName, int index, string flag)
        {
            yield return null;
        }

        public void Next()
        {
            _onHold = true;
        }

        private List<Dialogue> GetAllDialoguesFromNpc(string npcName)
        {
            var dialogueList = new List<Dialogue>();
            XElement root = _dialoguesXML;

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
            XElement root = _dialoguesXML;

            XElement dialogue =
                (from element in root.Elements(npcName)
                    where element.Attribute("flag")?.Value == flag
                    select element).First();

            var lines =
                (from line in dialogue.Elements()
                    select line.Value).ToList();

            return new Dialogue(lines);
        }

        private Dialogue BuildDialogueWithIndex(string npcName, int index)
        {
            XElement root = _dialoguesXML;

            var dialogues =
                from element in root.Elements(npcName)
                where int.Parse(element.Attribute("index")?.Value
                                ?? throw new ArgumentException($"Dialogue {element.ToString()} has no index attribute."))
                      == index
                select element;

            var lines =
                (from line in dialogues.Elements()
                    select line.Value).ToList();


            return new Dialogue(lines);
        }
    }
}