using System;
using LSW._02._Code.System___Manager;
using Moon._01.Script.Mouses;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class SpawnAttention : MonoBehaviour
    {
        [SerializeField] private GameObject attentionPrefab;
        [SerializeField] private float spawnWaitTime = 3f;
        [SerializeField] private float playerMinDistance = 5f;
        [SerializeField] private float cameraMinDistance = 2f;
        [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 5f);
        [SerializeField] private ScriptFinderSO playerFinder;
        
        private float _timer;
        private Camera _main;

        private MouseManager _mouse;
        
        private void Awake()
        {
            _main = Camera.main;
            playerFinder.GetTransform();
            _mouse = SystemManager.Instance.GetSystemManager<MouseManager>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnWaitTime)
            {
                SpawnAttentionObject();
                _timer = 0f;
            }
        }

        private void SpawnAttentionObject()
        {
            Vector2 spawnAreaCenter = transform.position;
            Vector2 randomPos = spawnAreaCenter + new Vector2(
                UnityEngine.Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
                UnityEngine.Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f)
            );

            Vector2 playerPos = playerFinder.GetTransform().position;
            Vector2 cameraPos = _main.ScreenToWorldPoint(_mouse.ExactScreenPos);

            Vector2 toPlayer = randomPos - playerPos;
            if (toPlayer.sqrMagnitude < playerMinDistance * playerMinDistance)
            {
                if (toPlayer == Vector2.zero) toPlayer = Vector2.up;

                randomPos = playerPos + toPlayer.normalized * playerMinDistance;
            }

            Vector2 toCamera = randomPos - cameraPos;
            if (toCamera.sqrMagnitude < cameraMinDistance * cameraMinDistance)
            {
                if (toCamera == Vector2.zero) toCamera = Vector2.up;

                randomPos = cameraPos + toCamera.normalized * cameraMinDistance;
            }

            float minX = spawnAreaCenter.x - spawnAreaSize.x * 0.5f;
            float maxX = spawnAreaCenter.x + spawnAreaSize.x * 0.5f;
            float minY = spawnAreaCenter.y - spawnAreaSize.y * 0.5f;
            float maxY = spawnAreaCenter.y + spawnAreaSize.y * 0.5f;

            Vector2 finalSpawnPos = new Vector2(
                Mathf.Clamp(randomPos.x, minX, maxX),
                Mathf.Clamp(randomPos.y, minY, maxY)
            );

            Instantiate(attentionPrefab, finalSpawnPos, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        }
    }
}