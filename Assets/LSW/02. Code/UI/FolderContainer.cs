using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LSW._02._Code.UI
{
    public class FolderContainer : MonoBehaviour
    {
        [SerializeField] private List<FolderData> folderData;

        private void Awake()
        {
            foreach (var folder in folderData)
            {
                folder.folderBtn.onClick.AddListener(() => OpenFolder(folder));
            }
        }

        private void OpenFolder(FolderData folder)
        {
            // folderData.ForEach(f => f.imageContainer.gameObject.SetActive(Equals(folder, f)));
        }

        public void CloseAllFolder()
        {
            // folderData.ForEach(f => f.imageContainer.gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            CloseAllFolder();
        }

        private void OnDestroy()
        {
            foreach (var folder in folderData)
            {
                folder.folderBtn.onClick.RemoveAllListeners();
            }
        }
    }

    [Serializable]
    public struct FolderData
    {
        public Button folderBtn;
        // public ImageContainer imageContainer;
    }
}