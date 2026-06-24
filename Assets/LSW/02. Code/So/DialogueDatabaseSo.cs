using System;
using System.Collections.Generic;
using System.IO;
using LSW._02._Code.Core.Cores;
using UnityEngine;
namespace LSW._02._Code.So
{
    [CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Data/Dialogue Database")]
    public class DialogueDatabaseSo : ScriptableObject
    {
        public List<DialogueSheet> sheets = new();

        public void SaveToBinary(string filePath)
        {
            using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
            writer.Write(sheets.Count);
            foreach (var sheet in sheets)
            {
                writer.Write(sheet.sheetName);
                writer.Write((int)sheet.guestType);
                writer.Write(sheet.dialogues.Count);
                foreach (var entry in sheet.dialogues)
                {
                    writer.Write(entry.key);
                    writer.Write(entry.id);
                    writer.Write(entry.day);
                    writer.Write(entry.seq);
                    writer.Write((int)entry.speaker);
                    writer.Write((int)entry.type);
                    writer.Write(entry.branch);
                    writer.Write(entry.nextKey);
                    writer.Write(entry.content);
                    writer.Write(entry.sincerity);
                }
            }
        }
    }

    [Serializable]
    public class DialogueSheet
    {
        public string sheetName;
        public Guest guestType;
        public List<DialogueEntry> dialogues = new();
    }

    [Serializable]
    public struct DialogueEntry
    {
        public string key;
        public int id;
        public int day;
        public int seq;
        public SpeakerType speaker;
        public DialogueType type;
        public string branch;
        public string nextKey;
        [TextArea] public string content;
        public int sincerity;
    }

    public enum SpeakerType { None = 0, NPC = 1, PLAYER = 2 }
    public enum DialogueType { None = 0, Normal = 1, Select = 2, Reaction = 4 }
    
}