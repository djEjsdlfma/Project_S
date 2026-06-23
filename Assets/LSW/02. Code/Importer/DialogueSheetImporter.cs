using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.So;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LSW._02._Code.Importer
{
    public class DialogueSheetImporter : MonoBehaviour
    {
        [SerializeField] private Slider importProgress;
        
        private DialogueDataCore _dialogueDataCore;
        
        [Serializable]
        public class SheetInfo
        {
            public string guestName;
            public Guest guest;
            public string sheetUrl;
        }

        [Header("Google Sheet CSV")]
        public List<SheetInfo> sheets = new();

        [Header("Save Path")]
        public string savePath = "Assets/02.Data/DialogueDatabase.asset";
        
        public UnityEvent<DialogueDatabaseSo> onImportComplete;

        private async void Start()
        {
            try
            {
                _dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
            
                var database = await ImportDialogueSheets(); 
                
                importProgress.gameObject.SetActive(false);
                _dialogueDataCore.SetDatabase(database);
                onImportComplete?.Invoke(database);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public async Task<DialogueDatabaseSo> ImportDialogueSheets()
        {
            if (importProgress != null)
            {
                importProgress.minValue = 0;
                importProgress.maxValue = sheets.Count;
                importProgress.value = 0;
            }

            DialogueDatabaseSo database = ScriptableObject.CreateInstance<DialogueDatabaseSo>();

            for (int i = 0; i < sheets.Count; i++)
            {
                var sheet = sheets[i];
                using UnityWebRequest request = UnityWebRequest.Get(sheet.sheetUrl);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    float progress = (i + request.downloadProgress) / sheets.Count;
                    if (importProgress != null) importProgress.value = progress * sheets.Count;
    
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[Dialogue Import] 실패 : {sheet.guestName}\n{request.error}");
                }
                else
                {
                    string csv = request.downloadHandler.text;
                    DialogueSheet dialogueSheet = ParseCSV(sheet.guestName, sheet.guest, csv);
                    database.sheets.Add(dialogueSheet);
                    Debug.Log($"[Dialogue Import] 성공 : {sheet.guestName}");
                }

                // 2. 슬라이더 값 갱신 (현재 진행된 시트 개수)
                if (importProgress != null)
                {
                    importProgress.value = i + 1;
                }
            }

            SaveDatabase(database);
            return database;
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
            return value.Replace("\"", "").Replace("\'", "").Replace("$", ",").Trim().Replace("#", "\"");
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
}