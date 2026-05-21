using TMPro;
using UnityEngine;

public class Chatting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > 1f)
        {
            Destroy(gameObject);
        }
    }

    public void SetName(string guestName)
    {
        questName.SetText(guestName);
    }
}
