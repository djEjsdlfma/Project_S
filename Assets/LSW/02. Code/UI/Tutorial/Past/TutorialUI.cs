
using UnityEngine;
using UnityEngine.Events;

namespace LSW._02._Code.UI.Tutorial
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class TutorialUI : MonoBehaviour
    {
        public UnityEvent TutorialEnd { get; set; } = new UnityEvent();

        protected CanvasGroup CanvaGroup { get; set; }

        private void Awake()
        {
            CanvaGroup = GetComponent<CanvasGroup>();
            ResetAndHideTutorial();
        }

        public virtual bool ShowTutorial() { return false; }

        public virtual void ResetAndHideTutorial()
        {
            if (CanvaGroup == null) 
                return;
            
            CanvaGroup.alpha = 0f;
            CanvaGroup.blocksRaycasts = false;
            CanvaGroup.interactable = false;
        }
    }
}