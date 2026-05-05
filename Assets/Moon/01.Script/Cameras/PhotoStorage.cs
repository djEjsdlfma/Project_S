using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public struct Photo : IEquatable<Photo>
    {
        public Texture2D Image;
        public List<CamObject> CamObjs;

        public Photo(Texture2D image, List<CamObject> camObjs)
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
        private List<Photo> _photos = new List<Photo>();
        
        public int PhotoMany => _photos.Count;

        public void Reset()
        {
            _photos.Clear();
        }
        
        public void AddPhoto(Photo photo)
        {
            _photos.Add(photo);
        }
        
        public List<Photo> GetPhotos()
        {
            return _photos;
        }

        public void Destroyed()
        {
            foreach (var photo in _photos)
            {
                if(photo.Image)
                {
                    Destroy(photo.Image);
                }
            }
        }
    }
}