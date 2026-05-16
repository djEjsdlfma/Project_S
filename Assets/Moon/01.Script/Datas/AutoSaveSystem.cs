using System;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    public class AutoSaveSystem : MonoBehaviour
    {
        [SerializeField] private float autoSaveTime = 1;
        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= autoSaveTime)
            {
                DataManager.Instance.AutoSavedToCurrent();
                _timer = 0f;
            }
        }
    }
}