using TMPro;
using UnityEngine;

namespace LSW._02._Code.UI.Tutorial
{
    public class NormalTextTutorial : TutorialUI
    {
        [SerializeField] private TextMeshProUGUI tutorialText;

        private bool _isShowing = false;

        public override bool ShowTutorial()
        {
            if (CanvaGroup == null) 
                return false;

            CanvaGroup.alpha = 1f;
            
            _isShowing = true;
            return true;
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
        
        public void SetText(string text)
        {
            if (tutorialText != null)
            {
                tutorialText.SetText(text);
            }
        }
    }
}