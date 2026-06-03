
using System;
using System.Collections.Generic;
using Moon._01.Script.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class PhotoContainer : MonoBehaviour
    {
        public int PhotoCount { get; private set; }
        
        private Image[] _photoImages;

        private void OnEnable()
        {
            if(_photoImages == null)
                _photoImages = GetComponentsInChildren<Image>();
            
            if (DataManager.Instance.TryGetValue(CurrentGuestManager.C[0], out List<Texture2D> photo))
            {
                PhotoCount = photo.Count;
                SetPhoto(photo.ToArray());
            }
        }

        public void SetPhoto(Texture2D[] photo)
        {
            if (_photoImages.Length <= 0) 
                return;
            
            for (int i = 0; i < _photoImages.Length; i++)
            {
                _photoImages[i].sprite = Sprite.Create(photo[i], new Rect(0, 0, photo[i].width, photo[i].height), new Vector2(0.5f, 0.5f));
            }
        }

        private void OnDestroy()
        {
            if(_photoImages == null)
                return;
            
            if (_photoImages.Length <= 0) 
                return;
            
            for (int i = _photoImages.Length - 1; i >= 0; i--)
            {
                Destroy(_photoImages[i].sprite);
            }
        }
    }
}