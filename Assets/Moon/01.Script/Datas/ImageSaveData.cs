using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    [Serializable]
    public class ImageSaveData : ISerializationCallbackReceiver, IDisposable
    {
        private Dictionary<string, List<Texture2D>> _imageData = new Dictionary<string, List<Texture2D>>();
        
        [SerializeField] private List<StringPathPair> _imagePairs = new List<StringPathPair>();

        [Serializable]
        public struct StringPathPair
        {
            public string Key;
            public List<string> FileNames; 
        }

        public void OnBeforeSerialize()
        {
            _imagePairs.Clear();
            foreach (var kvp in _imageData)
            {
                List<string> savedFileNames = new List<string>();
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i] != null) 
                        savedFileNames.Add($"{kvp.Key}_{i}.png"); 
                }
                _imagePairs.Add(new StringPathPair { Key = kvp.Key, FileNames = savedFileNames });
            }
        }

        public void OnAfterDeserialize()
        {
            Dispose();
            
            foreach (var pair in _imagePairs)
            {
                _imageData[pair.Key] = new List<Texture2D>();
            }
        }

        public void Save(string filePath, string folderPath)
        {
            if (!Directory.Exists(folderPath)) 
                Directory.CreateDirectory(folderPath);

            foreach (var kvp in _imageData)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    Texture2D tex = kvp.Value[i];
                    if (tex == null) 
                        continue;

                    string fileName = $"{kvp.Key}_{i}.png";
                    string fullPath = Path.Combine(folderPath, fileName);
                    File.WriteAllBytes(fullPath, tex.EncodeToPNG());
                }
            }

            string jsonData = JsonUtility.ToJson(this, true);
            string encryptedData = RealKey.Encrypt(jsonData);
            File.WriteAllText(filePath, encryptedData);
        }

        public bool Load(string filePath, string folderPath)
        {
            if (!File.Exists(filePath)) 
                return false;

            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string jsonData = RealKey.Decrypt(encryptedData);

                if (!string.IsNullOrEmpty(jsonData))
                {
                    JsonUtility.FromJsonOverwrite(jsonData, this);

                    foreach (var pair in _imagePairs)
                    {
                        if (pair.FileNames == null) 
                             continue;

                        List<Texture2D> loadedTextures = new List<Texture2D>();

                        foreach (var fileName in pair.FileNames)
                        {
                            string fullPath = Path.Combine(folderPath, fileName);
                            if (File.Exists(fullPath))
                            {
                                byte[] bytes = File.ReadAllBytes(fullPath);
                                Texture2D texture = new Texture2D(2, 2);
                                texture.LoadImage(bytes);
                                loadedTextures.Add(texture);
                            }
                        }
                        _imageData[pair.Key] = loadedTextures;
                    }
                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }
        
        public bool TryGetValue(string key, out List<Texture2D> value) => _imageData.TryGetValue(key, out value);

        public void SaveData(string key, List<Texture2D> value)
        {
            if (_imageData.TryGetValue(key, out var oldList) && oldList != null)
            {
                foreach (var tex in oldList)
                {
                    if (tex != null) 
                        UnityEngine.Object.Destroy(tex);
                }
            }
            _imageData[key] = value;
        }

        public void Dispose()
        {
            foreach (var textureList in _imageData.Values)
            {
                if (textureList != null)
                {
                    foreach (var tex in textureList)
                    {
                        if (tex != null) 
                            UnityEngine.Object.Destroy(tex);
                    }
                }
            }
            _imageData.Clear();
        }
    }
}