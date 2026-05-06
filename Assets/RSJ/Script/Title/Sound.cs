using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    [SerializeField] private Slider MAS;
    [SerializeField] private Slider BGM;
    [SerializeField] private Slider SFX;

    [SerializeField] private AudioMixer _mixer;
}
