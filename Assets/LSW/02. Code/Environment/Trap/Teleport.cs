using System;
using DG.Tweening;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace LSW._02._Code.Environment.Trap
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private ScriptFinderSO playerFinder;
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
                EndFade();
            };
            
        }

        public void TeleportTo()
        {
            teleportFadeCanvas.DOFade(1f, 0.5f).OnComplete(
                _teleportTween);
        }

        private void EndFade()
        {
            teleportFadeCanvas.DOFade(0f, 0.5f);
        }
    }
}