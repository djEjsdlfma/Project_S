using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    [Serializable]
    public class DynamicSaveData : ISerializationCallbackReceiver
    {
        private Dictionary<string, int> _intData = new Dictionary<string, int>();
        private Dictionary<string, float> _floatData = new Dictionary<string, float>();
        private Dictionary<string, string> _stringData = new Dictionary<string, string>();
        private Dictionary<string, bool> _boolData = new Dictionary<string, bool>();

        [SerializeField] private List<StringIntPair> _intPairs = new List<StringIntPair>();
        [SerializeField] private List<StringFloatPair> _floatPairs = new List<StringFloatPair>();
        [SerializeField] private List<StringStringPair> _stringPairs = new List<StringStringPair>();
        [SerializeField] private List<StringBoolPair> _boolPairs = new List<StringBoolPair>();

        public void OnBeforeSerialize()
        {
            _intPairs.Clear();
            foreach (var kvp in _intData)
            {
                _intPairs.Add(new StringIntPair { Key = kvp.Key, Value = kvp.Value });
            }

            _floatPairs.Clear();
            foreach (var kvp in _floatData)
            {
                _floatPairs.Add(new StringFloatPair { Key = kvp.Key, Value = kvp.Value });
            }

            _stringPairs.Clear();
            foreach (var kvp in _stringData)
            {
                _stringPairs.Add(new StringStringPair { Key = kvp.Key, Value = kvp.Value });
            }

            _boolPairs.Clear();
            foreach (var kvp in _boolData)
            {
                _boolPairs.Add(new StringBoolPair { Key = kvp.Key, Value = kvp.Value });
            }
        }

        public void OnAfterDeserialize()
        {
            _intData.Clear();
            foreach (var pair in _intPairs)
            {
                _intData[pair.Key] = pair.Value;
            }

            _floatData.Clear();
            foreach (var pair in _floatPairs)
            {
                _floatData[pair.Key] = pair.Value;
            }

            _stringData.Clear();
            foreach (var pair in _stringPairs)
            {
                _stringData[pair.Key] = pair.Value;
            }

            _boolData.Clear();
            foreach (var pair in _boolPairs)
            {
                _boolData[pair.Key] = pair.Value;
            }
        }

        [Serializable]
        public struct StringIntPair
        {
            public string Key;
            public int Value;
        }

        [Serializable]
        public struct StringFloatPair
        {
            public string Key;
            public float Value;
        }

        [Serializable]
        public struct StringStringPair
        {
            public string Key;
            public string Value;
        }

        [Serializable]
        public struct StringBoolPair
        {
            public string Key;
            public bool Value;
        }

        public bool TryGetValue(string key, out int value) => _intData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out float value) => _floatData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out string value) => _stringData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out bool value) => _boolData.TryGetValue(key, out value);

        public void SaveData(string key, int value) => _intData[key] = value;
        public void SaveData(string key, float value) => _floatData[key] = value;
        public void SaveData(string key, string value) => _stringData[key] = value;
        public void SaveData(string key, bool value) => _boolData[key] = value;
    }
    
    [Serializable]
    public class ImageSaveData : ISerializationCallbackReceiver , IDisposable
    {
        private Dictionary<string, List<Texture2D>> _imageData = new Dictionary<string, List<Texture2D>>();
        
        [SerializeField] private List<StringImagePair> _imagePairs = new List<StringImagePair>();
        
        public void OnBeforeSerialize()
        {
            _imagePairs.Clear();
            foreach (var kvp in _imageData)
            {
                _imagePairs.Add(new StringImagePair { Key = kvp.Key, Value = kvp.Value.ConvertAll(texture => texture.EncodeToPNG()) });
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (var textureList in _imageData.Values)
            {
                if (textureList != null)
                {
                    foreach (var tex in textureList)
                    {
                        if (tex != null)
                        {
                            UnityEngine.Object.Destroy(tex);
                        }
                    }
                }
            }
            
            _imageData.Clear();
            
            foreach (var pair in _imagePairs)
            {
                _imageData[pair.Key] = pair.Value.ConvertAll(bytes =>
                {
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(bytes);
                    return texture;
                });
            }
        }
        
        [Serializable]
        public struct StringImagePair
        {
            public string Key;
            public List<byte[]> Value;
        }

        public bool TryGetValue(string key, out List<Texture2D> value) => _imageData.TryGetValue(key, out value);

        public void SaveData(string key, List<Texture2D> value)
        {
            if (_imageData.ContainsKey(key))
            {
                if (_imageData[key] != null)
                {
                    foreach (var tex in _imageData[key])
                    {
                        if (tex != null)
                        {
                            UnityEngine.Object.Destroy(tex);
                        }
                    }
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
                        {
                            UnityEngine.Object.Destroy(tex);
                        }
                    }
                }
            }
            _imageData.Clear();
        }
    }
}