
using LSW._02._Code.System___Manager;
using UnityEngine;

namespace LSW._02._Code.Environment.InteractableObject
{
    public class Piece : MonoBehaviour, ICopyable
    {
        [SerializeField] private string pieceId;
        public bool IsCoping { get; set; }
        public string PieceId => pieceId;
        
        public bool IsInCorrectPlace { get; private set; }
        
        private PuzzleManager _puzzleManager;

        private void Start()
        {
            _puzzleManager = SystemManager.Instance.GetSystemManager<PuzzleManager>();
        }

        private void Update()
        {
            CheckOverlapStatus();
        }

        private void CheckOverlapStatus()
        {
            if (IsCoping) 
                return;
            
            if (!GetVirtualCenter(out Vector2 myCenter, out float myRot)) 
                return;
            
            if (!_puzzleManager.GetBlueprint(pieceId, out var blueprint)) 
                return;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blueprint.posTolerance * 2f);
            
            bool foundMatch = false;
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out Piece otherPiece) && otherPiece != this)
                {
                    if (!otherPiece.GetVirtualCenter(out Vector2 otherCenter, out float otherRot))
                        continue;
                    
                    float dist = Vector2.Distance(myCenter, otherCenter);
                    float rotDiff = Mathf.Abs(Mathf.DeltaAngle(myRot, otherRot));
                    
                    if (dist <= blueprint.posTolerance && rotDiff <= blueprint.rotTolerance)
                    {
                        foundMatch = true;
                        break; 
                    }
                }
            }
            
            IsInCorrectPlace = foundMatch;
        }
        
        private bool GetVirtualCenter(out Vector2 centerPos, out float centerRot)
        {
            if (_puzzleManager.GetPiece(pieceId, out var data))
            {
                centerRot = transform.eulerAngles.z - data.localRotationZ;
                Vector2 rotatedOffset = Quaternion.Euler(0, 0, centerRot) * data.localPosition;
                centerPos = (Vector2)transform.position - rotatedOffset;
                return true;
            }
            centerPos = Vector2.zero;
            centerRot = 0f;
            return false;
        }

        public void Copy() => IsCoping = true;
        public void Paste() => IsCoping = false;
    }
}