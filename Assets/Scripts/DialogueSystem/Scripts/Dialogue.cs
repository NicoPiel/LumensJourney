using System;
using System.Collections.Generic;
using System.Text;

namespace DialogueSystem.Scripts
{
    public class Dialogue
    {
        public List<string> DialogueLines { get; }
        public int Index { get; private set; } = int.MaxValue;
        public string Flag { get; private set; }
        
        private int LineIndex { get; set; }

        public Dialogue(List<string> lines)
        {
            DialogueLines = lines;
            LineIndex = 0;
        }
        
        public Dialogue(List<string> lines, string flag)
        {
            DialogueLines = lines;
            LineIndex = 0;
            Flag = flag;
        }
        
        public Dialogue(List<string> lines, int index)
        {
            DialogueLines = lines;
            LineIndex = 0;
            Index = index;
        }
        
        public Dialogue(List<string> lines, int index, string flag)
        {
            DialogueLines = lines;
            LineIndex = 0;
            Index = index;
            Flag = flag;
        }

        public string NextLine ()
        {
            if (!HasNextLine()) throw new IndexOutOfRangeException("Tried to access dialogue lines out of range.");
            
            var result = DialogueLines[LineIndex];
            LineIndex++;
                
            return result;
        }

        public bool HasNextLine()
        {
            return DialogueLines.Count != LineIndex;
        }

        public bool HasIndex()
        {
            return Index < int.MaxValue;
        }

        public bool HasFlag()
        {
            return Flag != null;
        }

        /// <summary>
        /// Returns the dialogue nicely formatted.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var line in DialogueLines)
            {
                builder.Append(line);
                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}
