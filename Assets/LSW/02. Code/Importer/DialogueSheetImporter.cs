using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.So;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LSW._02._Code.Importer
{
    public class DialogueSheetImporter : MonoBehaviour
    {
        public string saveFileName = "DialogueDatabase.bin";
        [SerializeField] private Slider importProgress;
        
        [Serializable]
        public class SheetInfo
        {
            public string guestName;
            public Guest guest;
            public string sheetUrl;
        }

        public List<SheetInfo> sheets = new();
        public bool IsImporting { get; private set; }
        
        public async Task<DialogueDatabaseSo> ImportDialogueSheets()
        {
            IsImporting = true;
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
                
                if (importProgress != null)
                {
                    importProgress.value = i + 1;
                }
            }

            await Task.Delay(500); 

            SaveDatabase(database);
            IsImporting = false;
    
            return database;
        }

        private void SaveDatabase(DialogueDatabaseSo database)
        {
            string path = Path.Combine(Application.persistentDataPath, saveFileName);
            database.SaveToBinary(path);
        }

        private DialogueSheet ParseCSV(string sheetName, Guest guestType, string csv)
        {
            DialogueSheet sheet = new DialogueSheet { sheetName = sheetName, guestType = guestType };
            string[] lines = csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            for (int i = 2; i < lines.Length; i++)
            {
                string[] values = csvParser.Split(lines[i]);
                if (values.Length < 10) continue;
                sheet.dialogues.Add(new DialogueEntry {
                    key = values[1].Trim(),
                    id = ParseInt(values[0]),
                    day = ParseInt(values[2]),
                    seq = ParseInt(values[3]),
                    speaker = (SpeakerType)ParseFlags<SpeakerType>(values[4]),
                    type = (DialogueType)ParseFlags<DialogueType>(values[5]),
                    branch = Clean(values[6]),
                    nextKey = Clean(values[7]),
                    content = Clean(values[8]),
                    sincerity = ParseInt(values[9])
                });
            }
            return sheet;
        }

        private string Clean(string value) => value.Replace("\"", "").Replace("\'", "").Replace("$", ",").Trim().Replace("#", "\"");
        private int ParseInt(string value) { int.TryParse(Clean(value), out int result); return result; }
        private int ParseFlags<T>(string rawData) where T : struct, Enum
        {
            string cleanData = Clean(rawData);
            if (string.IsNullOrEmpty(cleanData) || cleanData == "-") return 0;
            int result = 0;
            foreach (var s in cleanData.Split(new[] { ',', '/' }))
                if (Enum.TryParse(s.Trim(), out T val)) result |= Convert.ToInt32(val);
            return result;
        }
    }
}