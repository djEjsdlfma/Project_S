using System.Collections.Generic;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public struct Photo
    {
        public Texture2D Image;
        public List<CamObject> CamObjs;

        public Photo(Texture2D image, List<CamObject> camObjs)
        {
            Image = image;
            CamObjs = camObjs;
        }
    }
    public class PhotoStorage : MonoBehaviour
    {
        private List<Photo> _photos = new List<Photo>();
    }
}