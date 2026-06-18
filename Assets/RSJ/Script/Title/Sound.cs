using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    [SerializeField] private Slider MAS;
    [SerializeField] private Slider BGM;
    [SerializeField] private Slider SFX;

    [SerializeField] private AudioMixer _mixer;

    public void Awake()
    {

    }

    public void ChangeMAS()
    {
        _mixer.SetFloat("MAS", MAS.value);
    }

    public void OnChangeToggle(bool value)
    {
        // value chagne part

    }


}