using csiimnida.CSILib.SoundManager.RunTime;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    [SerializeField] private Slider MAS;
    [SerializeField] private Slider BGM;
    [SerializeField] private Slider SFX;

    [SerializeField] private AudioMixer _mixer;

    private float latestMasValue;
    private float latestBGMValue;
    private float latestSFXValue;

    private void Awake()
    {
        MAS.maxValue = 6f; MAS.minValue = -50f;
        BGM.maxValue = 6f; BGM.minValue = -50f;
        SFX.maxValue = 6f; SFX.minValue = -50f;

        if (_mixer.GetFloat("BGM", out float Bvalue))
            BGM.value = Bvalue;
        if (_mixer.GetFloat("SFX", out float Svalue))
            SFX.value = Svalue;
        if (_mixer.GetFloat("Master", out float Mvalue))
            MAS.value = Mvalue;
    }

    private void Start()
    {
        latestMasValue = MAS.value;
        latestBGMValue = BGM.value;
        latestSFXValue = SFX.value;
    }

    public void ChangeMAS(bool Off)
    {
        if(Off == false)
            latestMasValue = MAS.value;

        _mixer.SetFloat("Master", Off == true ? latestMasValue : MAS.minValue);
        MAS.value = Off == true ? latestMasValue : MAS.minValue;
    }

    public void ChangeBGM(bool Off)
    {
        if(Off == false)
            latestMasValue = BGM.value;

        _mixer.SetFloat("BGM", Off == true ? latestBGMValue : BGM.minValue);
        BGM.value = Off == true ? latestBGMValue : BGM.minValue;;
    }

    public void ChangeSFX(bool Off)
    {
        if(Off == false)
            latestSFXValue = SFX.value;

        _mixer.SetFloat("SFX", Off == true ? latestSFXValue : SFX.minValue);
        SFX.value = Off == true ? latestSFXValue : SFX.minValue;
    }

    public void ChangeMAS()
    {
        _mixer.SetFloat("Master", MAS.value);
    }

    public void ChangeBGM()
    {
        _mixer.SetFloat("BGM", BGM.value);
    }

    public void ChangeSFX()
    {
        _mixer.SetFloat("SFX", SFX.value);
    }

    public void OnChangeToggle(bool value)
    {
        // value chagne part

    }


}