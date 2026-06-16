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
                var charController = _player.GetComponent<CharacterController>();
                if (charController != null) charController.enabled = false;
                
                _player.transform.position = teleportPoint.position;
    
                if (charController != null) 
                    charController.enabled = true;
                
                camera.PreviousStateIsValid = false;
                camera.OnTargetObjectWarped(_player.transform, teleportPoint.position - _player.transform.position);
                
                Vector3 targetCamPos = teleportPoint.position + new Vector3(0, 1.85f, -10f);
                camera.ForceCameraPosition(targetCamPos, Quaternion.identity);

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