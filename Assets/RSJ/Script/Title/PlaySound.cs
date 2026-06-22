using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private string BGMKey;

    private void Awake()
    {
        SoundManager.Instance.PlaySound(BGMKey);
    }
}
