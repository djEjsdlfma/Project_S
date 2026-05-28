using System;
using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using LSW._02._Code.UI.Tutorial;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSW._02._Code.Core.Cores
{
    public class TutorialCore : MonoBehaviour, ICore
    {
        [Header("Tutorial Data")]
        [SerializeField] private List<TutorialData> tutorialList;
        
        [Header("Tutorial UI")]
        [SerializeField] private Canvas tutorialCanvas;
        [SerializeField] private NormalTextTutorial normalTextTutorialUI;
        [SerializeField] private DialogueTutorial dialogueTutorialUI;
        [SerializeField] private ImageTutorial imageTutorialUI;
        
        private Queue<TutorialData> _tutorialQueue;
        private TutorialData _currentTutorial;
        private TutorialUI _currentTutorialUI;
        private Selectable[] _allSelectables;
        
        private CoreHandler _coreHandler;
        private bool _gotAllSelectable;
        
        public void Initialize(CoreHandler coreHandler)
        {
            _coreHandler = coreHandler;
            
            _tutorialQueue = new Queue<TutorialData>(tutorialList);
        }

        public void LoadScene(SceneType sceneType)
        {
            _allSelectables = null;
            _gotAllSelectable = false;
        }
        
        public void ShowTutorial()
        {
            if (!_gotAllSelectable)
            {
                GetAllSelectable();
                DisableAllInteraction();
            }
                
            if (_tutorialQueue.Count <= 0)
            {
                EndTutorial();
                return;
            }
            
            _currentTutorial = _tutorialQueue.Dequeue();
            _currentTutorialUI = GetTutorialUI(_currentTutorial.tutorialType);

            if (_currentTutorialUI == null)
            {
                Debug.LogError("TutorialUI is null");
                return;
            }
            
            _currentTutorialUI.TutorialEnd.AddListener(CurrentTutorialEnd);

            if (!_currentTutorialUI.ShowTutorial())
            {
                CurrentTutorialEnd();
                Debug.Log("Failed to show tutorial");
            }
        }

        private void GetAllSelectable()
        {
            _allSelectables = FindObjectsByType<Selectable>();
            _gotAllSelectable = true;
        }
        
        private void DisableAllInteraction()
        {
            foreach (Selectable selectable in _allSelectables)
            {
                selectable.interactable = false;
            }
            
            SystemManager systemManager = SystemManager.Instance;
            if (systemManager != null)
            {
                Title title = systemManager.GetSystemManager<Title>();
                if (title != null)
                    title.canEnterPassword = false;
            }
        }

        private TutorialUI GetTutorialUI(TutorialType tutorialType)
        {
            switch (tutorialType)
            {
                case TutorialType.NormalText:
                {
                    NormalTextTutorial normalTextTutorial = Instantiate(normalTextTutorialUI, tutorialCanvas.transform);
                    normalTextTutorial.SetText(_currentTutorial.tutorialText);
                    return normalTextTutorial;
                }
                case TutorialType.Dialogue:
                {
                    DialogueTutorial dialogueTutorial = Instantiate(dialogueTutorialUI, tutorialCanvas.transform);
                    return dialogueTutorial;
                }
                case TutorialType.Image:
                {
                    ImageTutorial imageTutorial = Instantiate(imageTutorialUI, tutorialCanvas.transform);
                    imageTutorial.SetImage(_currentTutorial.tutorialImage);
                    return imageTutorial;
                }
                default:
                    return null;
            }
        }

        public void CurrentTutorialEnd()
        {
            if (_currentTutorialUI == null) 
                return;
            
            _currentTutorialUI.TutorialEnd.RemoveListener(CurrentTutorialEnd);
            _currentTutorialUI.ResetAndHideTutorial();
            Destroy(_currentTutorialUI.gameObject);
            _currentTutorialUI = null;
            
            _currentTutorial.onTutorialEnd?.Invoke();
        }

        public void EndTutorial()
        {
            _coreHandler.ResetCoreAllData();
        }

        public void Reset()
        {
            
        }
    }
    
    [Serializable]
    public struct TutorialData
    {
        public TutorialType tutorialType;
        public UnityEvent onTutorialEnd;
        
        public string tutorialText;
        
        public Sprite tutorialImage;
    }
    
    public enum TutorialType
    {
        None = -1,
        NormalText = 0,
        Dialogue = 1,
        Image = 2
    }
}