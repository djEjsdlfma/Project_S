using LSW._02._Code.Player;
using Unity.VisualScripting;
using UnityEngine;

public class DebuffPlatform : MonoBehaviour
{
    [SerializeField] private float debuffSpeedValue;
    [SerializeField] private float debuffGravityValue;

    private bool debuffEnabled;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Player player) && debuffEnabled == false)
        {
            debuffEnabled = true;
            player.AddSpeedModifier(-debuffSpeedValue);

            if (player.gameObject.GetComponent<Rigidbody2D>().gravityScale > 2.5f)
            player.gameObject.GetComponent<Rigidbody2D>().gravityScale += debuffGravityValue;
        }
    }
}
