     using Unity.Mathematics;
using UnityEngine;

namespace Moon._01.Script.Sprites
{
    public class Blur : MonoBehaviour
    {
        private static readonly int BlurAmount = Shader.PropertyToID("_BlurSize");

        
        private SpriteRenderer _sR;

        private MaterialPropertyBlock _mpb;

        private void Start()
        {
            _mpb = new MaterialPropertyBlock();
            _sR = GetComponent<SpriteRenderer>();
        }

/*
        [Range(0f, 1f), SerializeField] private float paddingScale = 0.1f;

        private Texture2D _paddedTexture;
        private Sprite _paddedSprite;
        private void OnDestroy()
        {
            if (_paddedSprite != null) Destroy(_paddedSprite);
            if (_paddedTexture != null) Destroy(_paddedTexture);
        }

        private Texture2D AddPadding(Texture2D original, int padX, int padY)
        {
            int newW = original.width + padX * 2;
            int newH = original.height + padY * 2;

            Texture2D result = new Texture2D(newW, newH, TextureFormat.RGBA32, false);

            result.wrapMode = TextureWrapMode.Clamp;
            result.SetPixels(new Color[newW * newH]);
            result.SetPixels(padX, padY, original.width, original.height, original.GetPixels());
            result.Apply();

            return result;
        }
*/

        public void SetBlur(float value)
        {
            _sR.GetPropertyBlock(_mpb);
            _mpb.SetFloat(BlurAmount, value);
            _sR.SetPropertyBlock(_mpb);
        }
    }
}