using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem.Scripts
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager _instance;

        private const string PathToDialogueFile = "Assets/Scripts/DialogueSystem/Data/dialogues.xml";

        private XElement dialoguesXML;

        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        private void Awake()
        {
            onDialogueStart = new UnityEvent();
            onDialogueEnd = new UnityEvent();
        }

        private void Start()
        {
            dialoguesXML = XElement.Load(PathToDialogueFile);
        }

        private List<Dialogue> GetAllDialoguesFromNpc(string npcName)
        {
            var dialogueList = new List<Dialogue>();
            XElement root = dialoguesXML;

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
            XElement root = dialoguesXML;

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
            XElement root = dialoguesXML;

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