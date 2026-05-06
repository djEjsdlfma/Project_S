using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.Networking;

namespace LSW._02._Code.CSV.Importer
{
    public class DialogueDataManager : MonoBehaviour, ISystemManager
    {
        public List<SheetData> sheetData;
        
        private readonly Dictionary<string, Dictionary<string, DialogueData>> _allDialogues 
            = new Dictionary<string, Dictionary<string, DialogueData>>();

        private bool _isInitialized = false;

        public void Initialize(SystemManager systemManager)
        {
            // DontDestroyOnLoad(gameObject);
            StartCoroutine(InitializeAllSheets());
        }

        public void LoadScene(SceneType sceneType)
        {
            // Transform systemManagerTrm = FindAnyObjectByType<SystemManager>().transform;
            // if (systemManagerTrm != null)
            // {
            //     transform.SetParent(systemManagerTrm);
            // }
        }

        private IEnumerator InitializeAllSheets()
        {
            _isInitialized = false;

            foreach (SheetData sheet in sheetData)
            {
                yield return StartCoroutine(DownloadCSV(sheet));
            }
            
            _isInitialized = true;
        }

        private IEnumerator DownloadCSV(SheetData sheet)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(sheet.sheetUrl);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string csvData = webRequest.downloadHandler.text;
                ParseCSV(sheet.sheetName, csvData);
                
                if (_allDialogues.TryGetValue(sheet.sheetName, out var currentSheetData))
                {
                    Debug.Log($"<color=green><b>[{sheet.sheetName}]</b></color> 로드 완료 (데이터: {currentSheetData.Count}개)");
                }
            }
            else
            {
                Debug.LogError($"Load Failed. ({sheet.sheetName}): {webRequest.error}");
            }
        }

        private void ParseCSV(string sheetName, string data)
        {
            string[] lines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            
            Dictionary<string, DialogueData> currentSheetData = new Dictionary<string, DialogueData>();
            
            for (int i = 2; i < lines.Length; i++)
            {
                string[] values = csvParser.Split(lines[i]);
                
                if (values.Length < 6) 
                    continue;

                string key = values[0].Trim();
                if (string.IsNullOrEmpty(key)) 
                    continue; 

                DialogueData dialogueData = new DialogueData
                {
                    expression = values[1].Trim(),
                    condition = values[2].Trim(),
                    dialogue = values[3].Trim(),
                    actionEvent = values[4].Trim(),
                    nextKey = values[5].Trim()
                };

                currentSheetData.TryAdd(key, dialogueData);
            }

            _allDialogues.TryAdd(sheetName, currentSheetData);
        }
        
        public bool GetDialogueData(string sheetName, string key, out DialogueData data)
        {
            if (!_isInitialized)
            {
                data = default;
                return false;
            }
            
            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                return sheet.TryGetValue(key, out data);
            }
            data = default;
            return false;
        }

        public bool GetAllDialogueData(string sheetName, out Dictionary<string, DialogueData> data)
        {
            if (!_isInitialized)
            {
                data = null;
                return false;
            }
            
            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                data = sheet;
                return true;
            }
            data = null;
            return false;
        }
        
        // private T ParseFlags<T>(string rawData) where T : struct, Enum
        // {
        //     string cleanData = rawData.Replace("\"", "").Trim();
        //     if (string.IsNullOrEmpty(cleanData) || cleanData == "-") return default;
        //
        //     T result = default;
        //     string[] splitData = cleanData.Contains(",") ? cleanData.Split(',') : cleanData.Split('/');
        //
        //     foreach (var s in splitData)
        //     {
        //         if (Enum.TryParse(s.Trim(), out T val))
        //         {
        //             int intVal = Convert.ToInt32(val);
        //             int intResult = Convert.ToInt32(result);
        //             result = (T)(object)(intResult | intVal);
        //         }
        //     }
        //     return result;
        // }

        public void Reset() { }
    }

    [Serializable]
    public class SheetData
    {
        public string sheetName;
        public string sheetUrl;
    }
    
    public struct DialogueData
    {
        public string expression;
        public string condition;
        public string dialogue;
        public string actionEvent;
        public string nextKey;
    }
}
