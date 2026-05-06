using UnityEngine;

public class Chatting : MonoBehaviour
{
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > 1f)
        {
            Destroy(gameObject);
        }
    }
}
