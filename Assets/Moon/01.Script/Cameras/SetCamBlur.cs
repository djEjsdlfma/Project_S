using System;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class SetCamBlur : MonoBehaviour
    {
        private static readonly int BlurSize = Shader.PropertyToID("_BlurSize");
        [Range(0,0.05f),SerializeField] private float blurActiveSet = 0.025f;
        [SerializeField] private Material blurMat;

        private bool _blurActive = false;

        public bool BlurActive => _blurActive;

        private void Awake()
        {
            if (blurMat != null)
            {
                blurMat.SetFloat(BlurSize, 0f);
                _blurActive = false;
            }
        }
        
        public void ActiveBlur(bool active)
        {
            if (blurMat == null) return;
            
            if (active && !_blurActive)
            {
                blurMat.SetFloat(BlurSize, blurActiveSet);
                _blurActive = true;
            }
            else if (!active && _blurActive)
            {
                blurMat.SetFloat(BlurSize, 0f);
                _blurActive = false;
            }
        }
        
        private void OnDestroy()
        {
            if (blurMat != null)
            {
                blurMat.SetFloat(BlurSize, 0f);
                _blurActive = false;
            }
        }
    }
}