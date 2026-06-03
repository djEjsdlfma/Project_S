
using System.Collections.Generic;
using System.Linq;
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
        private List<PhotoSelector> _photoSelector;
        
        public List<PhotoSelector> SelectedPhotoSelector { get; private set; }
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
            
            _uploadPhotosImage = uploadPhotosButton.GetComponent<Image>();
        }

        private void SelectedPhoto(PhotoSelector photoSelector, bool isSelected)
        {
            if (isSelected && SelectedPhotoSelector.Count < maxSelectedPhoto 
                && !_photoSelector.Contains(photoSelector))
            {
                _photoSelector.Add(photoSelector);
                if (SelectedPhotoSelector.Count == maxSelectedPhoto)
                {
                    uploadPhotosButton.interactable = true;
                    _uploadPhotosImage.color = uploadPhotosBtnEnableColor;
                    CanChoose = false;
                }
            }
            else if(!isSelected && _photoSelector.Contains(photoSelector))
            {
                _photoSelector.Remove(photoSelector);
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
        }
    }
}