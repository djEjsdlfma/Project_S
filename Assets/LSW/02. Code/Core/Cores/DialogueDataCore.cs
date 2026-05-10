using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.So;
using UnityEngine;

namespace LSW._02._Code.Core.Cores
{
    public class DialogueDataCore : MonoBehaviour, ICore
    {
        [SerializeField] private DialogueDatabaseSo database;

        private readonly Dictionary<string, Dictionary<string, DialogueEntry>>
            _allDialogues = new();

        private bool _initialized;

        public void Initialize(CoreHandler coreHandler)
        {
            _allDialogues.Clear();

            foreach (DialogueSheet sheet in database.sheets)
            {
                Dictionary<string, DialogueEntry> map = new();

                foreach (DialogueEntry entry in sheet.dialogues)
                {
                    if (!map.ContainsKey(entry.key))
                    {
                        map.Add(entry.key, entry);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Duplicate Key Detected : {entry.key} " +
                            $"(Sheet : {sheet.sheetName})"
                        );
                    }
                }

                _allDialogues.Add(sheet.sheetName, map);
            }

            _initialized = true;

            Debug.Log("<color=green>Dialogue Database Initialized</color>");
            foreach (var pair in _allDialogues)
            {
                Debug.Log($"Dictionary Key : [{pair.Key}]");
            }
        }

        public bool GetDialogueDataByKey(string sheetName, string currentKey, out DialogueEntry entry)
        {
            entry = null;

            if (!_initialized)
                return false;

            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                return sheet.TryGetValue(currentKey, out entry);
            }

            return false;
        }

        public bool GetAllDialogueEntry(string sheetName, out Dictionary<string, DialogueEntry> entry)
        {
            entry = null;

            if (!_initialized)
                return false;

            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                entry = sheet;
                return true;
            }

            return false;
        }

        public bool GetFirstDialogueByDay(string sheetName, int day, out DialogueEntry entry)
        {
            entry = null;

            if (!_initialized)
                return false;

            if (!_allDialogues.TryGetValue(sheetName, out var sheet))
                return false;

            entry = sheet.Values
                .Where(x => x.day == day)
                .OrderBy(x => x.seq)
                .FirstOrDefault();

            return entry != null;
        }

        public bool GetSheetNameByGuest(Guest guest, out string sheetName)
        {
            sheetName = string.Empty;

            if (!_initialized)
                return false;

            DialogueSheet foundSheet = database.sheets.Find(data => data.guestType == guest);
            if (foundSheet != null)
            {
                sheetName = foundSheet.sheetName;
                return true;
            }

            return false;
        }

        public void LoadScene(SceneType sceneType) { }

        public void Reset() { }
    }
}