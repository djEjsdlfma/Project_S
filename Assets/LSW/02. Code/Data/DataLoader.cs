using System.Collections.Generic;
using DG.Tweening;
using LSW._02._Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LSW._02._Code.Data
{
    public class DataLoader : MonoBehaviour
    {
        [Header("Load UI")]
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private float duration = 0.5f;
        
        [Header("Scene")]
        [SerializeField] private SceneType loadSceneType;
        
        private List<IDataLoadManager> _managers;
        private bool _isLoadStarted = false;
        private Tweener _sliderTween;
        private float _lastTargetProgress = -1f;

        private void Start()
        {
            _managers = CoreHandler.Instance.GetDataLoadManagers();
        
            if (_managers == null || _managers.Count == 0)
            {
                loadingSlider.value = 1f;
                CompleteLoading();
                return;
            }

            loadingSlider.value = 0f;

            foreach (var manager in _managers)
            {
                manager.OnLoadError += HandleLoadError;
                manager.LoadData();
            }
            _isLoadStarted = true;
        }

        private void Update()
        {
            if (!_isLoadStarted || _managers == null) 
                return;

            float totalProgress = 0f;
            foreach (var manager in _managers)
            {
                totalProgress += manager.Progress;
            }
            
            float targetProgress = totalProgress / _managers.Count;
            
            if (!Mathf.Approximately(_lastTargetProgress, targetProgress))
            {
                _lastTargetProgress = targetProgress;
                
                if (_sliderTween != null && _sliderTween.IsActive())
                    _sliderTween.Kill();
                
                if (loadingSlider != null)
                {
                    _sliderTween = loadingSlider.DOValue(targetProgress, duration)
                        .SetEase(Ease.OutQuad);
                }
            }

            if (targetProgress >= 0.999f && _isLoadStarted)
            {
                _isLoadStarted = false;

                if (_sliderTween != null && _sliderTween.IsActive())
                {
                    _sliderTween.OnComplete(() => DelayedSceneLoad());
                }
                else
                {
                    DelayedSceneLoad();
                }
            }
        }

        private void HandleLoadError(string errorMsg)
        {
            Debug.LogError($"[DataLoad Error] {errorMsg}");
        }

        private void CompleteLoading()
        {
            _sliderTween?.Kill();
            DelayedSceneLoad();
        }

        private void DelayedSceneLoad()
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                SceneManager.LoadScene((int)loadSceneType); 
            }).SetUpdate(true);
        }

        private void OnDestroy()
        {
            _sliderTween?.Kill();
            DOTween.Kill(loadingSlider);

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