
using System;
using Moon._01.Script.Cameras;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class PhotoSelector : MonoBehaviour
    {
        [Header("Image")]
        [SerializeField] private Image photoImage;
        
        [Header("Select")]
        [SerializeField] private Button selectBtn;
        [SerializeField] private Image markImage;
        [SerializeField] private Color selectColor;
        [SerializeField] private Color unselectColor;

        private bool _isSelected = false;
        
        public event Action<PhotoSelector, bool> OnSelected;
        
        private void Start()
        {
            markImage.gameObject.SetActive(false);
        }

        public void SetImage(Photo photo)
        {
            Texture2D texture = photo.Image;
            
            photoImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            selectBtn.onClick.AddListener(Selected);
        }

        private void Selected()
        {
            _isSelected = !_isSelected;
            markImage.gameObject.SetActive(_isSelected);
            photoImage.color = _isSelected ? selectColor : unselectColor;
            OnSelected?.Invoke(this, _isSelected);
        }
        
        public Texture2D GetTexture()
            => photoImage.sprite.texture;

        private void OnDestroy()
        {
            selectBtn.onClick.RemoveAllListeners();
            Destroy(photoImage.sprite);
        }
    }
}