using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.So;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace LSW._02._Code.Importer
{
#if UNITY_EDITOR
    public class DialogueSheetImporter : MonoBehaviour
    {
        [Serializable]
        public class SheetInfo
        {
            public string sheetName;
            public Guest guest;
            public string sheetUrl;
        }

        [Header("Google Sheet CSV")]
        public List<SheetInfo> sheets = new();

        [Header("Save Path")]
        public string savePath = "Assets/02.Data/DialogueDatabase.asset";

        [ContextMenu("Import Dialogue Sheets")]
        public async void ImportDialogueSheets()
        {
            DialogueDatabaseSo database = ScriptableObject.CreateInstance<DialogueDatabaseSo>();

            foreach (var sheet in sheets)
            {
                using UnityWebRequest request = UnityWebRequest.Get(sheet.sheetUrl);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await System.Threading.Tasks.Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[Dialogue Import] 실패 : {sheet.sheetName}\n{request.error}");
                    continue;
                }

                string csv = request.downloadHandler.text;

                DialogueSheet dialogueSheet = ParseCSV(sheet.sheetName, sheet.guest, csv);

                database.sheets.Add(dialogueSheet);

                Debug.Log($"[Dialogue Import] 성공 : {sheet.sheetName}");
            }

            SaveDatabase(database);

            Debug.Log("<color=green>Dialogue Import Complete</color>");
        }

        private DialogueSheet ParseCSV(string sheetName, Guest guestType, string csv)
        {
            DialogueSheet sheet = new DialogueSheet();
            sheet.sheetName = sheetName;
            sheet.guestType = guestType;

            string[] lines = csv.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.RemoveEmptyEntries
            );

            Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            for (int i = 2; i < lines.Length; i++)
            {
                string[] values = csvParser.Split(lines[i]);

                if (values.Length < 10)
                    continue;

                string key = values[1].Trim();

                if (string.IsNullOrEmpty(key))
                    continue;

                DialogueEntry entry = new DialogueEntry
                {
                    key = key,

                    id = ParseInt(values[0]),
                    day = ParseInt(values[2]),
                    seq = ParseInt(values[3]),

                    speaker = ParseFlags<SpeakerType>(values[4]),
                    type = ParseFlags<DialogueType>(values[5]),

                    branch = Clean(values[6]),
                    nextKey = Clean(values[7]),
                    content = Clean(values[8]),

                    sincerity = ParseInt(values[9])
                };

                sheet.dialogues.Add(entry);
            }

            return sheet;
        }

        private void SaveDatabase(DialogueDatabaseSo database)
        {
            string folder = Path.GetDirectoryName(savePath);
            
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            DialogueDatabaseSo existing = AssetDatabase.LoadAssetAtPath<DialogueDatabaseSo>(savePath);

            if (existing != null)
            {
                AssetDatabase.DeleteAsset(savePath);
            }

            AssetDatabase.CreateAsset(database, savePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string Clean(string value)
        {
            return value.Replace("\"", "").Replace("\'", "").Trim();
        }

        private int ParseInt(string value)
        {
            int.TryParse(Clean(value), out int result);
            return result;
        }

        private T ParseFlags<T>(string rawData) where T : struct, Enum
        {
            string cleanData = Clean(rawData);

            if (string.IsNullOrEmpty(cleanData) || cleanData == "-")
                return default;

            T result = default;

            string[] splitData =
                cleanData.Contains(",")
                    ? cleanData.Split(',')
                    : cleanData.Split('/');

            foreach (var s in splitData)
            {
                if (Enum.TryParse(s.Trim(), out T val))
                {
                    int intVal = Convert.ToInt32(val);
                    int intResult = Convert.ToInt32(result);

                    result = (T)(object)(intResult | intVal);
                }
            }

            return result;
        }
    }
#endif
}