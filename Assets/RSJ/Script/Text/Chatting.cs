using TMPro;
using UnityEngine;

public class Chatting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.5f)
        {
            Destroy(gameObject);
        }
    }

    public void SetName(string guestName)
    {
        questName.SetText(guestName);
    }
}
