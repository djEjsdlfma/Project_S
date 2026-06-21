
using System.Collections.Generic;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using Moon._01.Script.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class PhotoContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI noPhotoText;
        [SerializeField] private List<Image> photoImages; 
        
        public int PhotoCount { get; private set; }
        private GameStatueCore _gameStatueCore;

        private void OnEnable()
        {
            if(_gameStatueCore == null)
                _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            ClearGeneratedSprites();
        }

        public void SetPhoto(Guest guest)
        {
            ClearGeneratedSprites();

            string guestKey = CurrentGuestManager.C[(int)guest - 1];
            
            if (DataManager.Instance.TryGetValue(guestKey, out List<Texture2D> photoList) && photoList.Count > 0)
            {
                noPhotoText.gameObject.SetActive(false);
                PhotoCount = Mathf.Min(photoList.Count, photoImages.Count);

                for (int i = 0; i < photoImages.Count; i++)
                {
                    if (i < photoList.Count)
                    {
                        photoImages[i].gameObject.SetActive(true);
                        
                        photoImages[i].sprite = Sprite.Create(
                            photoList[i], 
                            new Rect(0, 0, photoList[i].width, photoList[i].height),
                            new Vector2(0.5f, 0.5f), 
                            100.0f
                        );
                    }
                    else
                    {
                        photoImages[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                noPhotoText.gameObject.SetActive(true);
                PhotoCount = 0;

                for (int i = 0; i < photoImages.Count; i++)
                {
                    if (photoImages[i] != null)
                    {
                        photoImages[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnDisable()
        {
            ClearGeneratedSprites();
            noPhotoText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            ClearGeneratedSprites();
        }
        
        private void ClearGeneratedSprites()
        {
            if (photoImages == null) return;

            foreach (var img in photoImages)
            {
                if (img != null && img.sprite != null)
                {
                    Destroy(img.sprite);
                    img.sprite = null;
                }
            }
        }
    }
}