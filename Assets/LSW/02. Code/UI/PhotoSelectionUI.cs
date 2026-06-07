
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.System___Manager;
using Moon._01.Script.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class PhotoSelectionUI : MonoBehaviour
    {
        [SerializeField] private int maxSelectedPhoto = 3;
        [SerializeField] private Button uploadPhotosButton;
        [SerializeField] private Color uploadPhotosBtnEnableColor;
        [SerializeField] private Color uploadPhotosBtnDisableColor;
        
        private Image _uploadPhotosImage;
        private List<PhotoSelector> _photoSelector = new List<PhotoSelector>();

        private List<PhotoSelector> _selectedPhotoSelector = new List<PhotoSelector>();
        public bool CanChoose { get; private set; } = true;

        private void Start()
        {
            _photoSelector = GetComponentsInChildren<PhotoSelector>().ToList();
            _photoSelector.ForEach(ps =>
            {
                ps.SetImage(null);
                ps.gameObject.SetActive(false);
            });
            if (DataManager.Instance.TryGetValue(CurrentGuestManager.C[0], out List<Texture2D> photo))
            {
                bool photoCountMore = photo.Count > _photoSelector.Count;
                for (int i = 0; i < (photoCountMore ? _photoSelector.Count : photo.Count); i++)
                {
                    _photoSelector[i].gameObject.SetActive(true);
                    _photoSelector[i].SetImage(photo[i]);
                    _photoSelector[i].OnSelected += SelectedPhoto;
                }
            }
            
            uploadPhotosButton.onClick.AddListener(UploadPhotos);
            _uploadPhotosImage = uploadPhotosButton.GetComponent<Image>();
            
            uploadPhotosButton.interactable = false;
            _uploadPhotosImage.color = uploadPhotosBtnDisableColor;
        }

        private void UploadPhotos()
        {
            BubbleManager bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
            if (bubbleManager != null)
            {
                bubbleManager.SpawnPhotoMessage();
            }
        }

        private void SelectedPhoto(PhotoSelector photoSelector, bool isSelected)
        {
            if (isSelected && _selectedPhotoSelector.Count < maxSelectedPhoto 
                && !_selectedPhotoSelector.Contains(photoSelector))
            {
                _selectedPhotoSelector.Add(photoSelector);
                if (_selectedPhotoSelector.Count == maxSelectedPhoto)
                {
                    uploadPhotosButton.interactable = true;
                    _uploadPhotosImage.color = uploadPhotosBtnEnableColor;
                    CanChoose = false;
                }
            }
            else if(!isSelected && _selectedPhotoSelector.Contains(photoSelector))
            {
                _selectedPhotoSelector.Remove(photoSelector);
                uploadPhotosButton.interactable = false;
                _uploadPhotosImage.color = uploadPhotosBtnDisableColor;
                CanChoose = true;
            }
        }

        private void OnDestroy()
        {
            foreach (var photoSelector in _photoSelector)
            {
                photoSelector.OnSelected -= SelectedPhoto;
            }
            uploadPhotosButton.onClick.RemoveListener(UploadPhotos);
        }
    }
}