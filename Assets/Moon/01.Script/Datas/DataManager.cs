using System.Collections.Generic;
using System.IO;
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
        private string _imgPath;

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
            _imgPath = Application.persistentDataPath + "/SaveData/Images";
            
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            LoadData();
        }

        private void LoadData()
        {
            for (int i = 0; i < MaxSaveSlot; i++)
            {
                DynamicSaveData data = new DynamicSaveData();
                bool dataExists = data.Load($"{_path}/save_{i}.json");
                _allData.Add(data);
                _slotDataExist.Add(dataExists);

                ImageSaveData imgData = new ImageSaveData();
                bool imgExists = imgData.Load($"{_path}/saveImg_{i}.json", $"{_imgPath}/saveImgFolder_{i}");
                _allImageData.Add(imgData);
                _slotImgDataExist.Add(imgExists);
            }

            _autoDataExist = _autoSaved.Load($"{_path}/save_auto.json");
            _autoImgDataExist = _autoSavedImg.Load($"{_path}/saveImg_auto.json", $"{_imgPath}/saveImgFolder_auto");
        }
        
        #endregion

        #region Exist

        public bool SlotDataExist(int slot) => (slot >= 0 && slot < MaxSaveSlot) && _slotDataExist[slot];
        public bool AutoDataExist() => _autoDataExist;
        public bool ImgDataExist(int slot) => (slot >= 0 && slot < MaxSaveSlot) && _slotImgDataExist[slot];
        public bool AutoImgDataExist() => _autoImgDataExist;

        #endregion

        #region Save

        public void SlotSave(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot) return;
            CurrentData.Save($"{_path}/save_{slot}.json");
            _allData[slot].Load($"{_path}/save_{slot}.json");
            _slotDataExist[slot] = true;
        }

        public void AutoSavedToCurrent() => SlotSave(CurrentSaveSlot);

        public void AutoSave()
        {
            CurrentData.Save($"{_path}/save_auto.json");
            _autoSaved.Load($"{_path}/save_auto.json");
            _autoDataExist = true;
        }

        public void SaveImg(int slot)
        {
            if (slot < 0 || slot >= MaxSaveSlot) return;
            CurrentImage.Save($"{_path}/saveImg_{slot}.json", $"{_imgPath}/saveImgFolder_{slot}");
            _allImageData[slot].Load($"{_path}/saveImg_{slot}.json", $"{_imgPath}/saveImgFolder_{slot}");
            _slotImgDataExist[slot] = true;
        }
        
        public void AutoImgSave()
        {
            CurrentImage.Save($"{_path}/saveImg_auto.json", $"{_imgPath}/saveImgFolder_auto");
            _autoSavedImg.Load($"{_path}/saveImg_auto.json", $"{_imgPath}/saveImgFolder_auto");
            _autoImgDataExist = true;
        }
        
        public void AutoImgSavedToCurrent() => SaveImg(CurrentSaveSlot);

        #endregion

        #region Load

        public DynamicSaveData LoadSlot(int slot)
        {
            if(slot < 0 || slot >= MaxSaveSlot) return null;
            CurrentData.Load($"{_path}/save_{slot}.json");
            CurrentSaveSlot = slot;
            return CurrentData;
        }
        
        public DynamicSaveData LoadAutoSave()
        {
            CurrentData.Load($"{_path}/save_auto.json");
            return CurrentData;
        }
                
        public List<DynamicSaveData> GetAllData()
        {
            List<DynamicSaveData> list = new List<DynamicSaveData>();
            for (int i = 0; i < _allData.Count; i++)
            {
                DynamicSaveData clone = new DynamicSaveData();
                clone.Load($"{_path}/save_{i}.json");
                list.Add(clone);
            }
            return list;
        }

        public ImageSaveData LoadImg(int slot)
        {
            if(slot < 0 || slot >= MaxSaveSlot) return null;
            CurrentImage.Load($"{_path}/saveImg_{slot}.json", $"{_imgPath}/saveImgFolder_{slot}");
            return CurrentImage;
        }

        public ImageSaveData LoadAutoImgSave()
        {
            CurrentImage.Load($"{_path}/saveImg_auto.json", $"{_imgPath}/saveImgFolder_auto");
            return CurrentImage;
        }

        public List<ImageSaveData> GetAllImgData()
        {
            List<ImageSaveData> list = new List<ImageSaveData>();
            for (int i = 0; i < _allImageData.Count; i++)
            {
                ImageSaveData clone = new ImageSaveData();
                clone.Load($"{_path}/saveImg_{i}.json", $"{_imgPath}/saveImgFolder_{i}");
                list.Add(clone);
            }
            return list;
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
}