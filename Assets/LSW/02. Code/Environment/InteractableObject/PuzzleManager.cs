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