using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    [DefaultExecutionOrder(-75)]
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; private set; }

        [field: SerializeField] public int MaxSaveSlot { get; private set; } = 3;
        
        private Dictionary<string, int> _currentData = new Dictionary<string, int>();
        
        private Dictionary<string, int> _autoSaved = new Dictionary<string, int>();

        private List<Dictionary<string, int>> _allData = new List<Dictionary<string, int>>();
        
        private List<bool> _slotDataExist = new List<bool>();

        private bool _autoDataExist;
        
        private string _path;
        
#region Init

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            string rootPath = Directory.GetParent(Application.dataPath)?.FullName;
            _path = rootPath != null ? Path.Combine(rootPath, "SaveDatas") : Application.dataPath;
            
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                for (int i = 0; i < MaxSaveSlot; i++)
                {
                    string filePath = $"{_path}/save_{i}.json";
                    if (File.Exists(filePath))
                    {
                        string jsonData = File.ReadAllText(filePath);
                        Dictionary<string, int> data = DictionaryJsonConvert.FromJson<string, int>(jsonData);
                        _allData.Add(data);
                        _slotDataExist.Add(true);
                    }
                    else
                    {
                        _slotDataExist.Add(false);
                        _allData.Add(new Dictionary<string, int>());
                    }
                }

                string autoFilePath = $"{_path}/save_auto.json";
                if (File.Exists(autoFilePath))
                {
                    string jsonData = File.ReadAllText(autoFilePath);
                    _autoSaved = DictionaryJsonConvert.FromJson<string, int>(jsonData);
                    _autoDataExist = true;
                }
                else
                {
                    _autoDataExist = false;
                }
            }
            catch (Exception e)
            {
                DevLog.LogError($"init load failed : {e.Message}");
            }
        }

#endregion

#region Exist

        public bool SlotDataExist(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot)
            {
                DevLog.LogError("invalid save slot");
                return false;
            }
            return _slotDataExist[slot];
        }
        
        public bool AutoDataExist()
        {
            return _autoDataExist;
        }

#endregion

#region Save

        public void SaveData(string key, int value)
        {
            _currentData[key] = value;
        }
                
        public void SlotSave(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot)
            {
                return;
            }
            
            try
            {
                string jsonData = DictionaryJsonConvert.ToJson(_currentData, true);
                File.WriteAllTextAsync($"{_path}/save_{slot}.json", jsonData);
                _allData[slot] = new Dictionary<string, int>(_currentData);
                _slotDataExist[slot] = true;
            }
            catch (Exception e)
            {
                DevLog.LogError($"save failed: {e.Message}");
            }
        }

        public void AutoSave()
        {
            try
            {
                _autoSaved = new Dictionary<string, int>(_currentData);
                string jsonData = DictionaryJsonConvert.ToJson(_autoSaved, true);
                File.WriteAllTextAsync($"{_path}/save_auto.json", jsonData);
                _autoDataExist = true;
            }
            catch (Exception e)
            {
                DevLog.LogError($"save failed: {e.Message}");
            }
        }

#endregion

#region Load

        public bool TryGetValue(string key, out int value)
        {
            return _currentData.TryGetValue(key, out value);
        }

        public Dictionary<string, int> LoadSlot(int slot)
        {
            if(slot < 0 || slot >= MaxSaveSlot)
            {
                DevLog.LogError("invalid save slot");
                return null;
            }
            _currentData = new Dictionary<string, int>(_allData[slot]);
            return new Dictionary<string, int>(_allData[slot]);
        }
        
        public Dictionary<string, int> LoadAutoSave()
        {
            _currentData = new Dictionary<string, int>(_autoSaved);
            return new Dictionary<string, int>(_autoSaved);
        }
        
        public List<Dictionary<string, int>> GetAllData()
        {
            var list = new List<Dictionary<string, int>>();
            foreach (var data in _allData)
            {
                list.Add(new Dictionary<string, int>(data));
            }
            return new (list);
        }

#endregion
    }

    public static class DictionaryJsonConvert
    {
        public static string ToJson<TKey, TValue>(Dictionary<TKey, TValue> jsonDicData, bool pretty = false)
        {
            List<DataDictionary<TKey, TValue>> dataList = new List<DataDictionary<TKey, TValue>>();
            DataDictionary<TKey, TValue> dictionaryData;
            foreach (TKey key in jsonDicData.Keys)
            {
                dictionaryData = new DataDictionary<TKey, TValue>();
                dictionaryData.Key = key;
                dictionaryData.Value = jsonDicData[key];
                dataList.Add(dictionaryData);
            }
            JsonDataArray<TKey, TValue> arrayJson = new JsonDataArray<TKey, TValue>();
            arrayJson.data = dataList;

            return JsonUtility.ToJson(arrayJson, pretty);
        }
        
        public static Dictionary<TKey, TValue> FromJson<TKey, TValue>(string jsonData)
        {
            JsonDataArray<TKey, TValue> dataList = JsonUtility.FromJson<JsonDataArray<TKey, TValue>>(jsonData);
            
            Dictionary<TKey, TValue> returnDictionary = new Dictionary<TKey, TValue>();
            
            foreach (var dictionaryData in dataList.data)
            {
                returnDictionary[dictionaryData.Key] = dictionaryData.Value;
            }
            
            return returnDictionary;
        }
    }
    
    [Serializable]
    public class DataDictionary<TKey,TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    [Serializable]
    public class JsonDataArray<TKey, TValue>
    {
        public List<DataDictionary<TKey, TValue>> data;
    }
}