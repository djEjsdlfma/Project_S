using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
            _path = string.IsNullOrEmpty(rootPath) ? Application.dataPath + "SaveData" : Path.Combine(rootPath, "SaveData");
            
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
                        string realData = Decrypt(jsonData, RealKey.GetKey());
                        Dictionary<string, int> data = DictionaryJsonConvert.FromJson<string, int>(realData);
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
                string realData = Encrypt(jsonData, RealKey.GetKey());
                File.WriteAllTextAsync($"{_path}/save_{slot}.json", realData);
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
                string realData = Encrypt(jsonData, RealKey.GetKey());
                File.WriteAllTextAsync($"{_path}/save_auto.json", realData);
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

#region Encryption

        private string Encrypt(string text, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

                byte[] resultBytes = new byte[aes.IV.Length + encryptedBytes.Length];
                
                Buffer.BlockCopy(aes.IV, 0, resultBytes, 0, aes.IV.Length);
                Buffer.BlockCopy(encryptedBytes, 0, resultBytes, aes.IV.Length, encryptedBytes.Length);

                return Convert.ToBase64String(resultBytes);
            }
        }

        private string Decrypt(string encryptedText,  byte[] key)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedText);

            if (fullCipher.Length < 16)
                throw new ArgumentException("유효하지 않은 암호문입니다.");

            byte[] iv = new byte[16];
            byte[] cipherText = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, 16, cipherText, 0, cipherText.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
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

    public static class RealKey
    {
        private const string DummyKey = "b2WshK2D3d25oAE45n9sQ==";

        private static readonly byte[] Key = 
        {
            20, 118, 52, 32, 90, 127, 11, 43, 99, 21, 106, 4, 61, 113, 120, 9
        };

        public static byte[] GetKey()
        {
            byte[] dummyBytes = Encoding.UTF8.GetBytes(DummyKey);
            byte[] realKey = new byte[Key.Length];

            for (int i = 0; i < Key.Length; i++)
            {
                realKey[i] = (byte)(Key[i] ^ dummyBytes[i % dummyBytes.Length]);
            }

            return realKey;
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