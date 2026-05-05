using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSW._02._Code.Scriptable_Object
{
    [CreateAssetMenu(fileName = "DialogueSo", menuName = "SO/DialogueSo", order = 0)]
    public class DialogueSo : ScriptableObject
    {
        public List<Dialogue> dialogues = new List<Dialogue>();

        public bool GetDialogueData(string findKey, out DialogueData data)
        {
            if (dialogues == null || dialogues.Count <= 0)
            {
                Debug.LogWarning("Dialogue was not set");
                data = new DialogueData();
                return false;
            }
            
            int dialogueIndex = dialogues.FindIndex(dialogue => dialogue.key == findKey);
            if (dialogueIndex < 0)
            {
                Debug.LogWarning("No dialogue of key : " + findKey);
                data = new DialogueData();
                return false;
            }

            data = dialogues[dialogueIndex].data;
            return true;
        }

        public void AddDialogueData(string key, DialogueData data)
        {
            dialogues.Add(new Dialogue(key, data));
        }
    }

    [Serializable]
    public class Dialogue
    {
        public string key;
        public DialogueData data;

        public Dialogue(string key, DialogueData data)
        {
            this.key = key;
            this.data = data;
        }
    }
    
    [Serializable]
    public struct DialogueData
    {
        public string expression;
        public string condition;
        public string dialogue;
        public string actionEvent;
        public string nextKey;
    }
}