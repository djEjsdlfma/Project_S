using System;
using System.Collections.Generic;
using Moon._01.Script.Datas;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    [Serializable]
    public struct Photo : IEquatable<Photo>
    {
        public Texture2D Image;
        public List<string> CamObjs;

        public Photo(Texture2D image, List<string> camObjs)
        {
            Image = image;
            CamObjs = camObjs;
        }

        public bool Equals(Photo other)
        {
            return Equals(Image, other.Image) && Equals(CamObjs, other.CamObjs);
        }

        public override bool Equals(object obj)
        {
            return obj is Photo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Image, CamObjs);
        }
    }
    public class PhotoStorage : MonoBehaviour
    {
        private static List<Photo> _photos = new List<Photo>();
        
        public int PhotoMany => _photos.Count;

        public int MaxPhoto { get; private set; } = 5;
        
        public void AddPhoto(Photo photo)
        {
            _photos.Add(photo);
        }
        
        public List<Photo> GetPhotos()
        {
            return new List<Photo>(_photos);
        }

        public void Destroyed()
        {
            foreach (var photo in _photos)
            {
                if(photo.Image)
                {
                    Destroy(photo.Image);
                }
                photo.CamObjs?.Clear();
            }
            _photos.Clear();
        }

        public void OnApplicationQuit()
        {
            Destroyed();
        }

        public bool CanPhoto()
        {
            return _photos.Count < MaxPhoto;
        }
    }
}