using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.Data
{
    public class DataLoader : MonoBehaviour
    {
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private float lerpSpeed = 5f;
        
        private List<IDataLoadManager> _managers;
        private bool _isLoadStarted = false;
        
        private void Start()
        {
            _managers = SystemManager.Instance.GetDataLoadManagers();
        
            if (_managers == null || _managers.Count == 0) return;

            foreach (var manager in _managers)
            {
                manager.OnLoadError += HandleLoadError;
                manager.LoadData();
            }
            _isLoadStarted = true;
        }

        private void Update()
        {
            if (!_isLoadStarted || _managers == null) return;

            float totalProgress = 0f;
            foreach (var manager in _managers)
            {
                totalProgress += manager.Progress;
            }
            
            float averageProgress = totalProgress / _managers.Count;
            
            loadingSlider.value = Mathf.Lerp(loadingSlider.value, averageProgress, Time.deltaTime * lerpSpeed);
            
            if (averageProgress >= 1f && loadingSlider.value >= 0.995f)
            {
                loadingSlider.value = 1f;
                CompleteLoading();
            }
        }
        
        private void HandleLoadError(string errorMsg)
        {
            Debug.LogError($"[DataLoad Error] {errorMsg}");
        }

        private void CompleteLoading()
        {
            _isLoadStarted = false;
            
            foreach (var manager in _managers)
            {
                manager.OnLoadError -= HandleLoadError;
            }
        }

        private void OnDestroy()
        {
            if (_managers != null)
            {
                foreach (var manager in _managers)
                {
                    manager.OnLoadError -= HandleLoadError;
                }
            }
        }
    }
}