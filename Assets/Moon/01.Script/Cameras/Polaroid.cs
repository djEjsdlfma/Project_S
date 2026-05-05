using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Moon._01.Script.Cameras
{
    public class Polaroid : MonoBehaviour
    {
        [field: SerializeField] public Button Btn { get; private set; }
        [SerializeField] private Image img;
        [SerializeField] private Outline selected;

        public void Init()
        {
            if (img.sprite)
            {
                Destroy(img.sprite);
            }
            Btn.onClick.RemoveAllListeners();
            selected.enabled = false;
            img.sprite = null;
            img.color = Color.black;
            gameObject.SetActive(false);
        }

        public void SetImage(Sprite sprite)
        {
            img.sprite = sprite;
            img.color = Color.white;
        }

        public void Selected(bool select)
        {
            selected.enabled = select;
        }
    }
}