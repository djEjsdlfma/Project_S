using System;
using UnityEngine;

namespace Moon._01.Script.Mouses
{
    public class MouseVisible : MonoBehaviour
    {
        [SerializeField] private bool visible;

        private void Awake()
        {            
            Cursor.visible = visible;
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
        }
    }
}
