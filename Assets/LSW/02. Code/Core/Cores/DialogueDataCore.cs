using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LSW._02._Code.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace LSW._02._Code.Core.Cores
{
    public class DialogueDataCore : MonoBehaviour, ICore, IDataLoadManager
    {
        public List<SheetData> sheetData;
        
        private readonly Dictionary<string, Dictionary<string, DialogueData>> _allDialogues
            = new Dictionary<string, Dictionary<string, DialogueData>>();

        private List<string> _dialogueKeyList = new List<string>();
        private List<DialogueData> _dialogueDataList = new List<DialogueData>();
        
        private float _progress = 0f;
        public float Progress => _progress;
        
        private bool _isInitialized = false;
        public bool IsDone => _isInitialized;
        
        public event Action<string> OnLoadError;
        
        public void Initialize(CoreHandler coreHandler) { }
        
        public void LoadData()
        {
            StartCoroutine(InitializeAllSheets());
        }

        public void LoadScene(SceneType sceneType) { }

        private IEnumerator InitializeAllSheets()
        {
            _isInitialized = false;
            _progress = 0f;

            for (int i = 0; i < sheetData.Count; i++)
            {
                yield return StartCoroutine(DownloadCSV(sheetData[i]));
                
                _progress = (float)(i + 1) / sheetData.Count;
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
                string errorLog;
                long errorCode = webRequest.responseCode;
                
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ProtocolError:
                    {
                        errorLog = "Protocol Error: " + errorCode + $"\n{webRequest.error}";
                        break;
                    }                    
                    case UnityWebRequest.Result.DataProcessingError:
                    {
                        errorLog = "Data Processing Error: " + errorCode + $"\n{webRequest.error}";
                        break;
                    }
                    case UnityWebRequest.Result.ConnectionError:
                    {
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                            errorLog = "Please Check Internet Connection Statue.";
                        else
                            errorLog = "Connection Error: " + errorCode + $"\n{webRequest.error}";
                        break;
                    }
                    default:
                    {
                        errorLog = $"{webRequest.result.ToString()}: " + errorCode;
                        break;
                    }
                }
                OnLoadError?.Invoke(errorLog);
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

                if (values.Length < 9)
                    continue;

                string key = values[1].Trim();
                if (string.IsNullOrEmpty(key))
                    continue;

                DialogueData dialogueData = new DialogueData
                {
                    ID = int.Parse(values[0].Replace("\'", "").Trim()),
                    Day = int.Parse(values[2].Replace("\'", "").Trim()),
                    Seq = int.Parse(values[3].Replace("\'", "").Trim()),
                    Speaker = ParseFlags<SpeakerType>(values[4].Trim()),
                    Type = ParseFlags<DialogueType>(values[5].Trim()),
                    Branch = values[6].Trim(),
                    NextKey = values[7].Trim(),
                    Content = values[8].Trim(),
                    Sincerity = int.Parse(values[9].Trim())
                };
                
                VLog.LogStruct($"Dialogue Data of Key :{key}", dialogueData);
                currentSheetData.TryAdd(key, dialogueData);
                
                _dialogueKeyList.Add(key);
                _dialogueDataList.Add(dialogueData);
            }

            _allDialogues.TryAdd(sheetName, currentSheetData);
        }

        public bool GetDialogueDataByKey(string sheetName, string key, out DialogueData data)
        {
            data = default;
            
            if (!_isInitialized)
            {
                return false;
            }

            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                return sheet.TryGetValue(key, out data);
            }
            
            return false;
        }
        
        public bool GetKeyByDialogueData(string sheetName, DialogueData data, out string key)
        {
            key = string.Empty;
            if (!_isInitialized)
            {
                return false;
            }

            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                string foundKey = sheet.FirstOrDefault(d => d.Value.Equals(data)).Key;
                key = foundKey;
                return foundKey != string.Empty;
            }
            
            return false;
        }

        public bool GetAllDialogueData(string sheetName, out Dictionary<string, DialogueData> data)
        {
            data = null;
            if (!_isInitialized)
            {
                return false;
            }

            if (_allDialogues.TryGetValue(sheetName, out var sheet))
            {
                data = sheet;
                return true;
            }
            
            return false;
        }

        public bool GetDialogueKeyByIndex(int index, out string key)
        {
            key = string.Empty;
            
            if (!_isInitialized)
            {
                return false;
            }

            if (_dialogueKeyList.Count > index)
            {
                bool isNull = _dialogueKeyList[index] != null;
                key = isNull ?  string.Empty : _dialogueKeyList[index];
                return !isNull;
            }
            
            return false;
        }

        public bool GetFirstDialogueByDay(string sheetName, int day, out string key, out DialogueData data)
        {
            key = string.Empty;
            data = default;
            
            if (!_isInitialized || !_allDialogues.ContainsKey(sheetName))
            {
                return false;
            }
            
            var firstDialogueEntry = _allDialogues[sheetName]
                .Where(pair => pair.Value.Day == day)
                .OrderBy(pair => pair.Value.Seq)
                .FirstOrDefault();
            
            if (firstDialogueEntry.Value.Equals(default))
            {
                return false;
            }
            
            if (!GetKeyByDialogueData(sheetName, firstDialogueEntry.Value, out string foundKey))
            {
                return false;
            }

            key = foundKey;
            data = firstDialogueEntry.Value;
            return true;
        }

        // string -> enum 변환용
        private T ParseFlags<T>(string rawData) where T : struct, Enum
        {
            string cleanData = rawData.Replace("\"", "").Trim();
            if (string.IsNullOrEmpty(cleanData) || cleanData == "-") return default;
        
            T result = default;
            string[] splitData = cleanData.Contains(",") ? cleanData.Split(',') : cleanData.Split('/');
        
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

        public void Reset() { }
    }

    [Serializable]
    public class SheetData
    {
        public string sheetName;
        public string sheetUrl;
    }

    public struct DialogueData : IEquatable<DialogueData>
    {
        public int ID;
        public int Day;
        public int Seq;
        public SpeakerType Speaker;
        public DialogueType Type;
        public string Branch;
        public string NextKey;
        public string Content;
        public int Sincerity;

        public bool Equals(DialogueData other)
        {
            return ID == other.ID && Day == other.Day && 
                   Seq == other.Seq && Speaker == other.Speaker && 
                   Type == other.Type && Branch == other.Branch && 
                   NextKey == other.NextKey && Content == other.Content && Sincerity == other.Sincerity;
        }

        public override bool Equals(object obj)
        {
            return obj is DialogueData other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(ID);
            hash.Add(Day);
            hash.Add(Seq);
            hash.Add((int)Speaker);
            hash.Add((int)Type);
            hash.Add(Branch);
            hash.Add(NextKey);
            hash.Add(Content);
            hash.Add(Sincerity);

            return hash.ToHashCode();
        }
    }

    [Serializable]
    public enum SpeakerType
    {
        None = -1,
        Npc = 0,
        Player = 1
    }

    [Serializable]
    public enum DialogueType
    {
        None = -1,
        Normal = 0,
        Select = 1,
        Reaction = 2
    }
}