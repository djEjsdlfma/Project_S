using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

public class Chatting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private SerializedDictionary<string, GameObject> ProfilPictures;

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

    public void SetProfil(string guestName)
    {
        foreach (GameObject picture in ProfilPictures.Values)
        {
            picture.SetActive(false);
        }

        ProfilPictures[guestName].SetActive(true);
    }
}
