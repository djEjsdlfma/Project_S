using System;
using DG.Tweening;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using Unity.Cinemachine;
using UnityEngine;

namespace LSW._02._Code.Environment.Trap
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private ScriptFinderSO playerFinder;
        [SerializeField] private CinemachineCamera camera;
        [SerializeField] private Transform teleportPoint;
        [SerializeField] private CanvasGroup teleportFadeCanvas;

        private Player.Player _player;
        private TweenCallback _teleportTween;
        
        private void Start()
        {
            _player = playerFinder.GetTarget<Player.Player>();
    
            _teleportTween = () =>
            {
                _player.transform.position = teleportPoint.position;
                
                camera.PreviousStateIsValid = false;
                camera.OnTargetObjectWarped(_player.transform, teleportPoint.position - _player.transform.position);

                EndFade();
            };
            
        }

        public void TeleportTo()
        {
            teleportFadeCanvas.DOFade(1f, 0.75f).OnComplete(
                _teleportTween);
        }

        private void EndFade()
        {
            teleportFadeCanvas.DOFade(0f, 1f);
            camera.PreviousStateIsValid = true;
        }
    }
}