using System;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    public class DataDeleteBtn : MonoBehaviour
    {
        [SerializeField] private GameObject realDeleteCheckPanel;
        private bool _isDelete = false;
        
        
        public void CheckDelete()
        {
            realDeleteCheckPanel.SetActive(true);
        }
        
        public void RealDelete()
        {
            DataManager.Instance.AutoDeleteToCurrent();
            _isDelete = true;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_isDelete)
            {
                Application.Quit();
            }
        }
    }
}