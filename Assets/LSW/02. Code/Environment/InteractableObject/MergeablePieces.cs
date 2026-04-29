using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LSW._02._Code.Environment.InteractableObject
{
    [Serializable]
    public struct PieceRequirement
    {
        public string pieceId;
        public Vector2 localPosition; // 합쳐졌을 때의 기준점 대비 상대 위치
        public float tolerance;      // 허용 오차 범위
    }

    public class MergeablePieces : MonoBehaviour
    {
        [SerializeField] private List<PieceRequirement> requirements = new List<PieceRequirement>();
        [SerializeField] private GameObject resultPrefab; // 합쳐졌을 때 생성될 오브젝트
        [SerializeField] private float checkRadius = 5f;  // 주변 조각을 찾을 범위

        public void CheckMerge(Piece triggeredPiece)
        {
            // triggeredPiece 주변에서 필요한 조각들이 있는지 확인
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius);
            List<Piece> foundPieces = new List<Piece>();

            foreach (var hit in colliders)
            {
                if (hit.TryGetComponent(out Piece piece))
                {
                    foundPieces.Add(piece);
                }
            }

            List<Piece> piecesToMerge = new List<Piece>();
            bool allRequirementsMet = true;

            foreach (var req in requirements)
            {
                Piece matchingPiece = foundPieces.FirstOrDefault(p => 
                    p.PieceId == req.pieceId && 
                    Vector2.Distance(p.transform.position - transform.position, req.localPosition) <= req.tolerance);

                if (matchingPiece != null)
                {
                    piecesToMerge.Add(matchingPiece);
                }
                else
                {
                    allRequirementsMet = false;
                    break;
                }
            }

            if (allRequirementsMet)
            {
                PerformMerge(piecesToMerge);
            }
        }

        private void PerformMerge(List<Piece> pieces)
        {
            foreach (var piece in pieces)
            {
                Destroy(piece.gameObject);
            }

            if (resultPrefab != null)
            {
                Instantiate(resultPrefab, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRadius);

            foreach (var req in requirements)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + (Vector3)req.localPosition, req.tolerance);
            }
        }
    }
}