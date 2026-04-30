
using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using UnityEngine;

namespace LSW._02._Code.Environment.InteractableObject
{
    public class Piece : MonoBehaviour, ICopyable
    {
        [SerializeField] private string pieceId;
        public bool IsCoping { get; set; }
        public string PieceId => pieceId;
        
        private PuzzleManager _puzzleManager;

        private void Start()
        {
            _puzzleManager = SystemManager.Instance.GetSystemManager<PuzzleManager>();
        }

        private void Update()
        {
            CheckForSnapping();
        }

        private void CheckForSnapping()
        {
            if(IsCoping)
                return;
            
            if (!GetVirtualCenter(out Vector2 myCenter, out float myRot)) 
                return;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);
            
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out Piece otherPiece) && otherPiece != this)
                {
                    if (transform.parent != null && transform.parent == otherPiece.transform.parent)
                        continue;
                    
                    if (!CanMerge(this, otherPiece))
                        continue;

                    if (!otherPiece.GetVirtualCenter(out Vector2 otherCenter, out float otherRot))
                        continue;
                    
                    float dist = Vector2.Distance(myCenter, otherCenter);
                    float rotDiff = Mathf.Abs(Mathf.DeltaAngle(myRot, otherRot));

                    bool isFoundBlueprint = _puzzleManager.GetBlueprint(pieceId, out var blueprint);
                    
                    if(!isFoundBlueprint)
                        continue;
                    
                    if (dist <= blueprint.posTolerance && rotDiff <= blueprint.rotTolerance)
                    {
                        SnapTo(otherPiece, myCenter, myRot);
                        break; 
                    }
                }
            }
        }

        private bool CanMerge(Piece a, Piece b)
        {
            List<string> idsA = new List<string>();
            if (a.transform.parent != null)
            {
                foreach (Transform child in a.transform.parent)
                    if (child.TryGetComponent(out Piece p)) idsA.Add(p.PieceId);
            }
            else idsA.Add(a.PieceId);
            
            List<string> idsB = new List<string>();
            if (b.transform.parent != null)
            {
                foreach (Transform child in b.transform.parent)
                    if (child.TryGetComponent(out Piece p)) idsB.Add(p.PieceId);
            }
            else idsB.Add(b.PieceId);
            
            foreach (string id in idsA)
            {
                if (idsB.Contains(id)) return false;
            }
            
            return true;
        }
        
        private void SnapTo(Piece other, Vector2 centerPos, float centerRot)
        {
            Transform groupTransform;
            
            if (transform.parent == null && other.transform.parent == null)
            {
                GameObject groupObj = new GameObject("PuzzleGroup")
                {
                    transform =
                    {
                        position = centerPos,
                        rotation = Quaternion.Euler(0, 0, centerRot)
                    }
                };

                groupTransform = groupObj.transform;
                transform.SetParent(groupTransform, true);
                other.transform.SetParent(groupTransform, true);
            }
            else if (other.transform.parent != null && transform.parent == null)
            {
                groupTransform = other.transform.parent;
                transform.SetParent(groupTransform, true);
            }
            else if (transform.parent != null && other.transform.parent == null)
            {
                groupTransform = transform.parent;
                other.transform.SetParent(groupTransform, true);
            }
            else
            {
                groupTransform = other.transform.parent;
                Transform myOldGroup = transform.parent;
                
                while (myOldGroup.childCount > 0)
                {
                    myOldGroup.GetChild(0).SetParent(groupTransform, true);
                }
                Destroy(myOldGroup.gameObject);
            }

            foreach (Transform child in groupTransform)
            {
                if (child.TryGetComponent(out Piece p))
                {
                    if (_puzzleManager.GetPiece(p.PieceId, out var data))
                    {
                        child.localPosition = data.localPosition;
                        child.localRotation = Quaternion.Euler(0, 0, data.localRotationZ);
                    }
                }
            }
            
            _puzzleManager.CheckCompletion(groupTransform);
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

        public void Copy()
        {
            IsCoping = true;
        }

        public void Paste()
        {
            IsCoping = false;
        }
    }
}