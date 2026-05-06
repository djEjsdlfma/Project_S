using System;
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.System___Manager;
using UnityEngine;

namespace LSW._02._Code.Environment.InteractableObject
{
    [Serializable]
    public struct PieceData
    {
        public string pieceId;
        public Vector2 localPosition;
        public float localRotationZ;
    }

    [Serializable]
    public class PieceBlueprint
    {
        [Header("Puzzle Blueprint")]
        public List<PieceData> pieceData = new List<PieceData>();
        public GameObject resultPrefab;

        [Header("Snap Tolerance")]
        public float posTolerance = 0.1f;
        public float rotTolerance = 15f;
    }
    
    public class PuzzleManager : MonoBehaviour, ISystemManager
    {
        [SerializeField] private List<PieceBlueprint> blueprints = new List<PieceBlueprint>();
        
        private List<Piece> _activePieces = new List<Piece>();

        public void RegisterPiece(Piece piece) => _activePieces.Add(piece);
        public void UnregisterPiece(Piece piece) => _activePieces.Remove(piece);
        
        public void CheckPuzzleCompletion(string pieceId)
        {
            if (!GetBlueprint(pieceId, out var blueprint)) 
                return;
            
            var requiredIds = blueprint.pieceData.Select(d => d.pieceId).ToList();

            var matchingPieces = _activePieces
                .Where(p => requiredIds.Contains(p.PieceId) && p.IsInCorrectPlace)
                .ToList();

            if (matchingPieces.Count == requiredIds.Count)
            {
                AssemblePuzzle(blueprint, matchingPieces);
            }
        }

        private void AssemblePuzzle(PieceBlueprint blueprint, List<Piece> pieces)
        {
            if (pieces[0].GetVirtualCenter(out Vector2 centerPos, out float centerRot))
            {
                if (blueprint.resultPrefab != null)
                {
                    Instantiate(blueprint.resultPrefab, centerPos, Quaternion.Euler(0, 0, centerRot));
                }
                
                foreach (var p in pieces)
                {
                    UnregisterPiece(p);
                    Destroy(p.gameObject);
                }
                
                Debug.Log("퍼즐 완성! 조각 삭제 및 결과물 생성 완료.");
            }
        }
        
        public void Initialize(SystemManager systemManager) { }
        public void LoadScene(SceneType sceneType) { }

        public bool GetBlueprint(string id, out PieceBlueprint foundBlueprint)
        {
            int index = blueprints.FindIndex(b => b.pieceData.Any(data => data.pieceId == id));
            
            if (index >= 0)
            {
                foundBlueprint = blueprints[index];
                return true;
            }

            foundBlueprint = null;
            return false;
        }

        public bool GetPiece(string id, out PieceData foundPiece)
        {
            bool isFoundBp = GetBlueprint(id, out PieceBlueprint foundBlueprint);

            if (isFoundBp)
            {
                int index = foundBlueprint.pieceData.FindIndex(data => data.pieceId == id);
                if (index >= 0)
                {
                    foundPiece = foundBlueprint.pieceData[index];
                    return true;
                }
            }
            
            foundPiece = default;
            return false;
        }

        public void Reset() { }
    }
}