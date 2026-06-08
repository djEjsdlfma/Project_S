using System;
using System.Collections.Generic;
using LSW._02._Code.Core;
using Moon._01.Script.Datas;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
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
    public class PhotoStorage : MonoBehaviour, ICore
    {
        private List<Photo> _photos = new List<Photo>();
        
        [SerializeField] private ScriptListFinderSO _photoListFinder;
        
        public int PhotoMany => _photos.Count;

        public int MaxPhoto { get; private set; } = 5;
        
        public void Initialize(CoreHandler coreHandler) { }
        
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
        
        public void LoadScene(SceneType sceneType)
        {
            if (IsPlatformerScene(sceneType))
            {
                Destroyed();
                _photoListFinder.AddTarget(this, true);
            }
        }

        private bool IsPlatformerScene(SceneType sceneType)
            => sceneType == SceneType.ChoiMyeongJinScene || sceneType == SceneType.DaEunJungScene
               || sceneType == SceneType.LeeJaeYoonScene || sceneType == SceneType.SeoAhYoonScene
               || sceneType == SceneType.YulParkScene;

        public void Reset() { }
    }
}