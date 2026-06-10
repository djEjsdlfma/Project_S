using System;
using LSW._02._Code.Player;
using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class JumpAndSlowed : MonoBehaviour
    {
        [SerializeField] private float speedMin = 0.25f;
        [SerializeField] private float speedDownMultiply = 0.75f;
        [SerializeField] private float gravityMax = 3f;
        [SerializeField] private float gravityMultiply = 1.25f;

        public bool Used { get; set; } = false;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!Used && other.gameObject.CompareTag("Player"))
            {
                var player = other.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    Used = true;
                    float trapSpeed = player.GetTrapSpeedMultiplier();
                    player.SetTrapSpeedToMin(trapSpeed * speedDownMultiply, speedMin);

                    float jumpSpeed = player.GetTrapGravityMultiplier();
                    player.SetGravityScaleToMax(jumpSpeed * gravityMultiply, gravityMax);
                }
            }
        }
    }
}