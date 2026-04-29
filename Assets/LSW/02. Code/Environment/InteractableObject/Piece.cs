using UnityEngine;
using System;

namespace LSW._02._Code.Environment.InteractableObject
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private string pieceId;
        public string PieceId => pieceId;

        private bool _isDragging = false;
        private Vector3 _offset;
        private Camera _mainCamera;

        public event Action<Piece> OnPlaced;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnMouseDown()
        {
            _offset = transform.position - GetMouseWorldPos();
            _isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (_isDragging)
            {
                transform.position = GetMouseWorldPos() + _offset;
            }
        }

        private void OnMouseUp()
        {
            _isDragging = false;
            OnPlaced?.Invoke(this);
            CheckForMergePoints();
        }

        private void CheckForMergePoints()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out MergeablePieces mergePoint))
                {
                    mergePoint.CheckMerge(this);
                }
            }
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -_mainCamera.transform.position.z;
            return _mainCamera.ScreenToWorldPoint(mousePos);
        }
    }
}