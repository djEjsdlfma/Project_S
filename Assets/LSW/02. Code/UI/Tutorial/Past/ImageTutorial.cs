
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI.Tutorial
{
    public class ImageTutorial : TutorialUI
    {
        [SerializeField] private Image tutorialImage;
        
        private bool _isShowing = false;

        public override bool ShowTutorial()
        {
            if (CanvaGroup == null) 
                return false;

            CanvaGroup.alpha = 1f;
            CanvaGroup.blocksRaycasts = true;
            CanvaGroup.interactable = true;
            
            _isShowing = true;
            return true;
        }

        public void SetImage(Sprite sprite)
        {
            if(tutorialImage != null)
                tutorialImage.sprite = sprite;
        }

        private void Update()
        {
            if (!_isShowing) 
                return;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TutorialEnd?.Invoke();
                ResetAndHideTutorial();
            }
        }

        public override void ResetAndHideTutorial()
        {
            base.ResetAndHideTutorial();
            _isShowing = false;
        }
    }
}