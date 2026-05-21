using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class ArrowSeeTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform arrow;
        [SerializeField] private Transform cam;
        
        private void FixedUpdate()
        {
            Vector2 playerToTarget = target.position - cam.position;
            float angle = Mathf.Atan2(playerToTarget.y, playerToTarget.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0f, 0f, angle);
            
        }
    }
}