using System;
using System.Collections.Generic;
using LSW._02._Code.Core.Cores;
using UnityEngine;
namespace LSW._02._Code.So
{
    [CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Data/Dialogue Database")]
    public class DialogueDatabaseSo : ScriptableObject
    {
        public List<DialogueSheet> sheets = new();
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
        [TextArea]
        public string content;

        public int sincerity;
    }

    [Serializable]
    public enum SpeakerType
    {
        None = 0,
        NPC = 1,
        PLAYER = 2
    }

    [Serializable]
    public enum DialogueType
    {
        None = 0,
        Normal = 1,
        Select = 2,
        Reaction = 4
    }
}