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
        
        public DynamicSaveData CurrentData { get; private set; } = new DynamicSaveData();

        public ImageSaveData CurrentImage { get; private set; } = new ImageSaveData();

        public int CurrentSaveSlot { get; private set; } = 0;
        
        private DynamicSaveData _autoSaved = new DynamicSaveData();
        
        private ImageSaveData _autoSavedImg = new ImageSaveData();

        private List<DynamicSaveData> _allData = new List<DynamicSaveData>();
        
        private List<ImageSaveData> _allImageData = new List<ImageSaveData>();
        
        private List<bool> _slotDataExist = new List<bool>();
        
        private List<bool> _slotImgDataExist = new List<bool>();

        private bool _autoDataExist;

        private bool _autoImgDataExist;
        
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

        private void OnDestroy()
        {
            foreach (var imageSaveData in _allImageData)
            {
                imageSaveData.Dispose();
            }
            
            _autoSavedImg.Dispose();
        }

        private void Init()
        {
            string rootPath = Directory.GetParent(Application.dataPath)?.FullName;
            _path = string.IsNullOrEmpty(rootPath) ? Application.dataPath + "/SaveData" : Path.Combine(rootPath, "SaveData");
            
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
                        DynamicSaveData data = null;
                        bool isDataValid = false;

                        try
                        {
                            string realData = Decrypt(jsonData, RealKey.GetKey());

                            if (!string.IsNullOrEmpty(realData))
                            {
                                data = JsonUtility.FromJson<DynamicSaveData>(realData);
                                isDataValid = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(
                                $"[DataManager] Slot {i} save data is corrupted / Decryption failed : {e.Message}");
                        }

                        if (isDataValid && data != null)
                        {
                            _allData.Add(data);
                            _slotDataExist.Add(true);
                        }
                        else
                        {
                            _slotDataExist.Add(false);
                            _allData.Add(new DynamicSaveData());
                        }
                    }
                    else
                    {
                        _slotDataExist.Add(false);
                        _allData.Add(new DynamicSaveData());
                    }

                    filePath = $"{_path}/saveImg_{i}.json";
                    if (File.Exists(filePath))
                    {
                        string jsonData = File.ReadAllText(filePath);
                        ImageSaveData data = null;
                        bool isDataValid = false;

                        try
                        {
                            string realData = Decrypt(jsonData, RealKey.GetKey());

                            if (!string.IsNullOrEmpty(realData))
                            {
                                data = JsonUtility.FromJson<ImageSaveData>(realData);
                                isDataValid = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(
                                $"[DataManager] Slot {i} image save data is corrupted / Decryption failed : {e.Message}");
                        }

                        if (isDataValid && data != null)
                        {
                            _allImageData.Add(data);
                            _slotImgDataExist.Add(true);
                        }
                        else
                        {
                            _allImageData.Add(new ImageSaveData());
                            _slotImgDataExist.Add(false);
                        }
                    }
                    else
                    {
                        _allImageData.Add(new ImageSaveData());
                        _slotImgDataExist.Add(false);
                    }
                }

                string autoFilePath = $"{_path}/save_auto.json";
                if (File.Exists(autoFilePath))
                {
                    string jsonData = File.ReadAllText(autoFilePath);
                    string realData = Decrypt(jsonData, RealKey.GetKey());
                    _autoSaved = JsonUtility.FromJson<DynamicSaveData>(realData);
                    _autoDataExist = true;
                }
                else
                {
                    _autoDataExist = false;
                }
                
                autoFilePath = $"{_path}/saveImg_auto.json";
                if ( File.Exists(autoFilePath))
                {
                    string jsonData = File.ReadAllText(autoFilePath);
                    string realData = Decrypt(jsonData, RealKey.GetKey());
                    _autoSavedImg = JsonUtility.FromJson<ImageSaveData>(realData);
                    _autoImgDataExist = true;
                }
                else
                {
                    _autoImgDataExist = false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] Initial load failed: {e.Message}");
            }
        }
        
#endregion

#region Exist

        public bool SlotDataExist(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot)
            {
                Debug.LogError($"[DataManager] Invalid save slot requested: {slot}");
                return false;
            }
            return _slotDataExist[slot];
        }
                
        public bool AutoDataExist()
        {
            return _autoDataExist;
        }

        public bool ImgDataExist(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot)
            {
                Debug.LogError($"[DataManager] Invalid save slot requested: {slot}");
                return false;
            }
            return _slotImgDataExist[slot];
        }

        public bool AutoImgDataExist()
        {
            return _autoImgDataExist;
        }

        #endregion

#region Save
        public void SlotSave(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot)
            {
                return;
            }
            
            try
            {
                string jsonData = JsonUtility.ToJson(CurrentData, true);
                string realData = Encrypt(jsonData, RealKey.GetKey());
                File.WriteAllText($"{_path}/save_{slot}.json", realData);
                        
                _allData[slot] = JsonUtility.FromJson<DynamicSaveData>(jsonData);
                _slotDataExist[slot] = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] Save failed: {e.Message}");
            }
        }

        public void AutoSavedToCurrent()
        {
            SlotSave(CurrentSaveSlot);
        }

        public void AutoSave()
        {
            try
            {
                string jsonData = JsonUtility.ToJson(CurrentData, true);
                string realData = Encrypt(jsonData, RealKey.GetKey());
                File.WriteAllText($"{_path}/save_auto.json", realData);
                        
                _autoSaved = JsonUtility.FromJson<DynamicSaveData>(jsonData);
                _autoDataExist = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] Save failed: {e.Message}");
            }
        }

        public void SaveImg(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot)
            {
                return;
            }
            
            try
            {
                string jsonData = JsonUtility.ToJson(CurrentImage, true);
                string realData = Encrypt(jsonData, RealKey.GetKey());
                File.WriteAllText($"{_path}/saveImg_{slot}.json", realData);
                        
                _allImageData[slot] = JsonUtility.FromJson<ImageSaveData>(jsonData);
                _slotImgDataExist[slot] = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] Image save failed: {e.Message}");
            }
        }
        
        public void AutoImgSave()
        {
            try
            {
                string jsonData = JsonUtility.ToJson(CurrentImage, true);
                string realData = Encrypt(jsonData, RealKey.GetKey());
                File.WriteAllText($"{_path}/saveImg_auto.json", realData);
                        
                _autoSavedImg = JsonUtility.FromJson<ImageSaveData>(jsonData);
                _autoImgDataExist = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataManager] Image save failed: {e.Message}");
            }
        }
        
        public void AutoImgSavedToCurrent()
        {
            SaveImg(CurrentSaveSlot);
        }

#endregion

#region Load

        public DynamicSaveData LoadSlot(int slot)
        {
            if(slot < 0 || slot >= MaxSaveSlot)
            {
                Debug.LogError($"[DataManager] Invalid save slot requested: {slot}");
                return null;
            }
                    
            string clonedJson = JsonUtility.ToJson(_allData[slot]);
            CurrentData = JsonUtility.FromJson<DynamicSaveData>(clonedJson);
            CurrentSaveSlot = slot;
            return CurrentData;
        }
        
        public DynamicSaveData LoadAutoSave()
        {
            string clonedJson = JsonUtility.ToJson(_autoSaved);
            CurrentData = JsonUtility.FromJson<DynamicSaveData>(clonedJson);
            return CurrentData;
        }
                
        public List<DynamicSaveData> GetAllData()
        {
            List<DynamicSaveData> list = new List<DynamicSaveData>();
            foreach (var data in _allData)
            {
                string clonedJson = JsonUtility.ToJson(data);
                list.Add(JsonUtility.FromJson<DynamicSaveData>(clonedJson));
            }
            return list;
        }

        public ImageSaveData LoadImg(int slot)
        {
            if(slot < 0 || slot >= MaxSaveSlot)
            {
                Debug.LogError($"[DataManager] Invalid save slot requested: {slot}");
                return null;
            }
                    
            string clonedJson = JsonUtility.ToJson(_allImageData[slot]);
            CurrentImage = JsonUtility.FromJson<ImageSaveData>(clonedJson);
            return CurrentImage;
        }

        public ImageSaveData LoadAutoImgSave()
        {
            string clonedJson = JsonUtility.ToJson(_autoSavedImg);
            CurrentImage = JsonUtility.FromJson<ImageSaveData>(clonedJson);
            return CurrentImage;
        }

        public List<ImageSaveData> GetAllImgData()
        {
            List<ImageSaveData> list = new List<ImageSaveData>();
            foreach (var data in _allImageData)
            {
                string clonedJson = JsonUtility.ToJson(data);
                list.Add(JsonUtility.FromJson<ImageSaveData>(clonedJson));
            }

            return list;
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

        private string Decrypt(string encryptedText, byte[] key)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedText);

            if (fullCipher == null)
            {
                throw new ArgumentNullException(nameof(fullCipher), "Cipher data cannot be null.");
            }

            if (fullCipher.Length < 16)
            {
                throw new ArgumentException("Invalid cipher data length.", nameof(fullCipher));
            }

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

#region GetValue

        public bool TryGetValue(string key, out int value) => CurrentData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out float value) => CurrentData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out string value) => CurrentData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out bool value) => CurrentData.TryGetValue(key, out value);
        
        public bool TryGetValue(string key, out List<Texture2D> value) => CurrentImage.TryGetValue(key, out value);
        

#endregion

#region SaveData

        public void SaveData(string key, int value) => CurrentData.SaveData(key, value);
        public void SaveData(string key, float value) => CurrentData.SaveData(key, value);
        public void SaveData(string key, string value) => CurrentData.SaveData(key, value);
        public void SaveData(string key, bool value) => CurrentData.SaveData(key, value);
        
        public void SaveData(string key, List<Texture2D> value) => CurrentImage.SaveData(key, value);

#endregion

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
}