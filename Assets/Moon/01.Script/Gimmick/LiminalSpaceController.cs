using Unity.Cinemachine;
using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class LiminalSpaceController : MonoBehaviour
    {
        [Header("Loop Settings")] 
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField]private int requiredMoveTimes = 10;

        [Header("References")] 
        [SerializeField] private Transform player;
        [SerializeField] private Rigidbody2D playerRb;

        private int _currentMoveTimes = 0;
        private bool _isLooping = true;
        private bool _isEffectActive = true;

        private Vector3 _warpVector;
        private CinemachineBrain _brain;
        private CinemachineCamera _cam;
        private bool _isBrainNotNull;
        private bool _isCamNotNull;

        private void Awake()
        {
            if (Camera.main != null)
            {
                _brain = Camera.main.GetComponent<CinemachineBrain>();
            }
        }

        private void Start()
        {
            _cam = _brain.ActiveVirtualCamera as CinemachineCamera;
            _warpVector = new Vector3(startPoint.position.x - endPoint.position.x, 0, 0);
            _isCamNotNull = _cam != null;
            _isBrainNotNull = _brain != null;
        }

        private void FixedUpdate()
        {
            if (!_isLooping || !_isEffectActive) return;

            if (playerRb.position.x >= endPoint.position.x)
            {
                if (_currentMoveTimes <= requiredMoveTimes)
                {
                    RigidbodyInterpolation2D originalInterpolation = playerRb.interpolation;
                    playerRb.interpolation = RigidbodyInterpolation2D.None;

                    playerRb.position = new Vector2(playerRb.position.x + _warpVector.x, playerRb.position.y);
                    player.position = new Vector3(playerRb.position.x, player.position.y, player.position.z);
                    
                    Physics2D.SyncTransforms();


                    if (_isBrainNotNull && _brain.ActiveVirtualCamera != null)
                    {
                        if (_isCamNotNull)
                        {
                            _cam.OnTargetObjectWarped(player, _warpVector);
                        }
                    }

                    playerRb.interpolation = originalInterpolation;

                    _currentMoveTimes++;
                }
                else
                {
                    _isEffectActive = false;
                    _isLooping = false;
                }
            }
        }
    }
}