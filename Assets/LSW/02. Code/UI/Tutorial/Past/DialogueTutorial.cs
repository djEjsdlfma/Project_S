using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;

namespace LSW._02._Code.UI.Tutorial.Past
{
    public class DialogueTutorial : TutorialUI
    {
        private BubbleManager _bubbleManager;

        public override bool ShowTutorial()
        {
            SystemManager systemManager = SystemManager.Instance;
            if (systemManager == null)
                return false;
            
            _bubbleManager = systemManager.GetSystemManager<BubbleManager>();
            if (_bubbleManager == null)
                return false;
            
            _bubbleManager.ChangeGuestDialogue(Guest.Tutorial);
            _bubbleManager.SpawnMessage();
            _bubbleManager.onSpawnMessage += EndSpawnMessage;
            return true;
        }

        private void EndSpawnMessage(LastDialogueData _)
        {
            _bubbleManager.onSpawnMessage -= EndSpawnMessage;
            TutorialEnd.Invoke();
        }
    }
}