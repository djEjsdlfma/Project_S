using System.Collections;
using System.Collections.Generic;
using LSW._02._Code.System___Manager;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace LSW._02._Code.Environment.Trap
{
    public class BlackHandWallSystem : MonoBehaviour, ISystemManager
    {
        [SerializeField] private BlackHand blackHandPrefab;
        [SerializeField] private float handRadius = 0.5f;
        
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private float minDistance = 3f;
        [SerializeField] private float maxDistance = 7f;
        
        [SerializeField] private LayerMask surfaceLayer;

        [SerializeField] private ScriptFinderSO playerFinder;
        [SerializeField] private Transform respawnPoint;
        
        private Transform _playerTransform;
        private Coroutine _spawnCoroutine;
        
        private List<BlackHand> _spawnedHands = new List<BlackHand>();

        public void Initialize(SystemManager systemManager) 
        {
            if (_playerTransform == null && playerFinder != null)
            {
                _playerTransform = playerFinder.GetTransform();
            }

            StartSpawning();
        }

        public void Reset() 
        {
            StopSpawning();
            
            foreach (var hand in _spawnedHands)
            {
                if (hand != null) 
                    Destroy(hand);
            }
            _spawnedHands.Clear();
        }

        private void StartSpawning()
        {
            if (_spawnCoroutine != null) 
                StopCoroutine(_spawnCoroutine);
            
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        private void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                TrySpawnHandOnSurface();
            }
        }

        private void TrySpawnHandOnSurface()
        {
            if (_playerTransform == null) 
                return;
            
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(minDistance, maxDistance);
            
            Vector2 offset = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)) * randomDistance;
            Vector2 spawnPosition = (Vector2)_playerTransform.position + offset;
            
            Vector2 surfaceNormal = Vector2.up;

            SpawnHand(spawnPosition, surfaceNormal);
        }

        private void SpawnHand(Vector2 spawnPosition, Vector2 surfaceNormal)
        {
            BlackHand hand = Instantiate(blackHandPrefab, spawnPosition, Quaternion.identity);
            
            hand.transform.up = surfaceNormal;
            
            _spawnedHands.Add(hand);
        }

        public void DespawnHand(BlackHand hand)
        {
            _spawnedHands.Remove(hand);
            Destroy(hand.gameObject);
        }

        public void CatchPlayer(Player.Player player)
        {
            if (respawnPoint == null) 
                return;
            
            if (player != null)
            {
                player.Rig.linearVelocity = Vector2.zero;
            }
            
            player.transform.position = respawnPoint.position;
        }
    }
}