using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LSW._02._Code.System___Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSW._02._Code.Core.Cores
{
    public class Transition : MonoBehaviour, ICore
    {
        [Header("Day Change Transition")]
        [SerializeField] private CanvasGroup dayChangeTransition;
        [SerializeField] private TextMeshProUGUI dayChangeText;
        
        [Header("Leave To Platform Transition")]
        [SerializeField] private CanvasGroup leaveToPlatformTransition;

        private GameStatueCore _gameStatueCore;
        private SceneType _currentSceneType;
        private SceneType _lastSceneType;
        
        public void Initialize(CoreHandler coreHandler)
        {
            _gameStatueCore = coreHandler.GetCore<GameStatueCore>();
        }

        public void TransitionScene(SceneType sceneType, TransitionType transitionType)
        {
            int buildIndex = (int)sceneType;
            if (buildIndex < 0)
                return;
            
            _lastSceneType = _currentSceneType;
            _currentSceneType = sceneType;
            TransitionScene(buildIndex, transitionType);
        }
        
        public void TransitionScene(int buildIndex, TransitionType transitionType)
        {
            CanvasGroup target;
            switch (transitionType)
            {
                case TransitionType.DayChange:
                    target = dayChangeTransition;
                    dayChangeText.SetText($"Day {_gameStatueCore.CurrentDay}");
                    break;
                case TransitionType.LeaveToPlatform:
                    target = leaveToPlatformTransition;
                    break;
                default:
                    target = null;
                    break;
            }

            if (target == null) return;

            target.DOKill();
    
            target.blocksRaycasts = true;
            target.DOFade(1, 0.5f).OnComplete(() =>
            {
                var op = SceneManager.LoadSceneAsync(buildIndex);
                if (op != null)
                {
                    op.allowSceneActivation = false;

                    StartCoroutine(WaitLoad(op, target));
                }
            });
        }
        
        private IEnumerator WaitLoad(AsyncOperation op, CanvasGroup target)
        {
            while (op.progress < 0.9f) yield return null;
            op.allowSceneActivation = true;
            
            yield return new WaitUntil(() => op.isDone);
            FadeOut(target);
        }
        
        private void FadeOut(CanvasGroup target)
        {
            target.DOFade(0, 0.95f).OnComplete(() => 
            {
                target.blocksRaycasts = false;
            });
            
            bool wasPlatformerScene = _lastSceneType == SceneType.ChoiMyeongJinScene || _lastSceneType == SceneType.DaEunJungScene || 
                                      _lastSceneType == SceneType.LeeJaeYoonScene || _lastSceneType == SceneType.SeoAhYoonScene ||
                                      _lastSceneType == SceneType.YulParkScene;
            
            if (wasPlatformerScene && _currentSceneType == SceneType.MainTabletScene)
            {
                BubbleManager bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
                if (bubbleManager != null)
                {
                    var dataToReplay = new Queue<LastDialogueData>(_gameStatueCore.SavedLastDialogueData);
                    _gameStatueCore.SavedLastDialogueData.Clear();
                    bubbleManager.ReplayList = dataToReplay.ToList();
                }
            }
        }

        public void LoadScene(SceneType sceneType) { }

        public void Reset() { }
    }
    
    public enum TransitionType
    {
        None = 0,
        DayChange = 1,
        LeaveToPlatform = 2
    }
}