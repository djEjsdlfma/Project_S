
using LSW._02._Code.Environment.Takable;
using LSW._02._Code.System___Manager;
using UnityEngine;

namespace LSW._02._Code.Environment.Trap
{
    public class BlackHand : MonoBehaviour, ITakable
    {
        [SerializeField] private string playerTag = "Player";
        
        private BlackHandWallSystem _blackHandWallSystem;
        private bool _isTaken = false;
        
        private void Awake()
        {
            _blackHandWallSystem = SystemManager.Instance.GetSystemManager<BlackHandWallSystem>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(_isTaken) 
                return;
            
            if (other.gameObject.CompareTag(playerTag) && other.gameObject.TryGetComponent(out Player.Player player))
            {
                _blackHandWallSystem.CatchPlayer(player);
            }
        }

        public void Take()
        {
            _isTaken = true;
            _blackHandWallSystem.DespawnHand(this);
        }

        public bool IsDisableCapture()
        {
            return false;
        }

        public bool CanBeTaken()
        {
            return !_isTaken;
        }
    }
}