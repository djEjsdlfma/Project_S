using System;
using System.Collections.Generic;
using System.Linq;
using Moon._01.Script.Datas;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Moon._01.Script.Cameras
{
    [Serializable]
    public struct Answer
    {
        public string names;
        public int score;
        public bool isCanFakeOrReal;
        public bool isReal;
    }
    public class SubmitPhoto : MonoBehaviour
    {
        [SerializeField] private ScriptListFinderSO camerasFinder;
        [SerializeField] private GameObject submitPanel;
        [SerializeField] private List<Polaroid> polaroids;
        [SerializeField] private List<Answer> answers;
        private List<Photo> _photos = new List<Photo>();

        private List<bool> _answerIsScored = new List<bool>();
        
        private Dictionary<Photo, bool> _photoSelection = new Dictionary<Photo, bool>();

        private const int AnswerCount = 3;
        
        private int _selectCount = 0;
        
        [ContextMenu("StartSubmit")]
        public void StartSubmit()
        {
            _photos.Clear();
            _photos = camerasFinder.GetTarget<PhotoStorage>().GetPhotos();
            
            /*if(_photos.Count < AnswerCount)
                return;*/
            
            _answerIsScored.Clear();

            for (int j = 0; j < answers.Count; j++)
            {
                _answerIsScored.Add(false);
            }
            
            _photoSelection.Clear();
            submitPanel.SetActive(true);

            foreach (var polaroid in polaroids)
            {
                polaroid.Init();
            }
            
            int i = 0;
            foreach (var photo in _photos)
            {
               _photoSelection.Add(photo, false);
                //사진이나 보여주기
                //사진과 연관된 버튼 활성화
               SetPhotoUI(photo, i++);
            }

            _selectCount = 0;

        }

        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame && !submitPanel.activeSelf)
            {
                StartSubmit();
            }
        }

        public void Cancel()
        {
            _selectCount = 0;
            _photoSelection.Clear();
            foreach (var polaroid in polaroids)
            {
                polaroid.Init();
            }
            submitPanel.SetActive(false);
        }
        
        public void EndSubmit()
        {
            if (_selectCount < AnswerCount) return;

            int score = 0;
            int fakeOrRealScore = 0;

            // 1. 선택된 사진들의 모든 객체(CamObjs)를 하나의 리스트로 모음 (LINQ 활용)
            var selectedObjects = _photos
                .Where(p => _photoSelection[p])
                .SelectMany(p => p.CamObjs)
                .ToList();
            
            var selectedTexture = _photos
                .Where(p => _photoSelection[p])
                .Select(p => p.Image)
                .ToList();

            // 2. 정답 리스트를 순회하며 점수 계산
            for (int i = 0; i < answers.Count; i++)
            {
                // 이미 채점된 정답은 패스 (보호 구문)
                if (_answerIsScored[i]) continue;

                var answer = answers[i];

                // 선택된 사진 객체 중 정답과 이름이 일치하는 것이 하나라도 있는지 검사
                bool isAnswerFound = selectedObjects.Any(obj => obj == answer.names);

                if (isAnswerFound)
                {
                    // 점수 부여 로직
                    if (answer.isCanFakeOrReal)
                    {
                        if (answer.isReal) 
                            fakeOrRealScore = answer.score;
                        else if (fakeOrRealScore == 0) 
                            fakeOrRealScore = answer.score;
                    }
                    else
                    {
                        score += answer.score;
                    }

                    _answerIsScored[i] = true;
                }
            }
            score += fakeOrRealScore;
            camerasFinder.GetTarget<ResultPhoto>().Result(score, selectedTexture);
        }

        private void OnDestroy()
        {
            EndAndDelete();
        }

        public void EndAndDelete()
        {
            //camerasFinder.GetTarget<PhotoStorage>().Destroyed();
            foreach (var polaroid in polaroids)
            {
                polaroid.Init();
            }
        }

        private void SetPhotoUI(Photo photo, int i)
        {
            polaroids[i].gameObject.SetActive(true);
            
            Sprite sprite = Sprite.Create(photo.Image, new Rect(0, 0, photo.Image.width, photo.Image.height), new Vector2(0.5f, 0.5f));

            polaroids[i].SetImage(sprite);
            
            polaroids[i].Btn.onClick.AddListener(() =>
            {
                SelectPhoto(i);
            });
        }

        private void SelectPhoto(int i)
        {
            if (_photoSelection[_photos[i]])
            {
                _photoSelection[_photos[i]] = false;
                _selectCount--;
                polaroids[i].Selected(false);
                return;
            }
            if (AnswerCount > _selectCount)
            {
                _photoSelection[_photos[i]] = true;
                _selectCount++;
                polaroids[i].Selected(true);
            }
        }
    }
}