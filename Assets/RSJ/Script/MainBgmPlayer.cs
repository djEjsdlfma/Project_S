using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MainBgmPlayer : MonoBehaviour
{
    public static MainBgmPlayer Instance;

    [SerializeField] private AudioClip bgmClip;          // 기본 메인 브금
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool autoPlay = false;      // ▼ 끄면 수동 시작

    private AudioSource _source;
    private Coroutine _fade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // autoPlay일 때만 곡 넘겨받아 재생, 아니면 그냥 정리
            if (autoPlay) Instance.Play(bgmClip);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _source = gameObject.AddComponent<AudioSource>();
        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = volume;
        if (mixerGroup != null) _source.outputAudioMixerGroup = mixerGroup;

        if (autoPlay) Play(bgmClip); // ▼ 자동 재생은 토글이 켜져있을 때만
    }

    // 인스펙터에 끼운 기본 곡 재생 (버튼 OnClick에 바로 연결 가능)
    public void Play() => Play(bgmClip);

    // 원하는 곡 지정 재생
    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (_source.clip == clip && _source.isPlaying) return; // 같은 곡이면 무시 → 안 끊김

        if (_fade != null) StopCoroutine(_fade);
        _fade = StartCoroutine(Switch(clip));
    }

    public void Stop()
    {
        if (_fade != null) StopCoroutine(_fade);
        _fade = StartCoroutine(FadeOutStop());
    }

    private IEnumerator Switch(AudioClip clip)
    {
        if (_source.isPlaying && fadeDuration > 0f)
        {
            float t = _source.volume;
            while (t > 0f)
            {
                t -= Time.unscaledDeltaTime / fadeDuration * volume;
                _source.volume = Mathf.Max(t, 0f);
                yield return null;
            }
        }

        _source.clip = clip;
        _source.Play();

        if (fadeDuration > 0f)
        {
            float t = 0f;
            while (t < volume)
            {
                t += Time.unscaledDeltaTime / fadeDuration * volume;
                _source.volume = Mathf.Min(t, volume);
                yield return null;
            }
        }
        _source.volume = volume;
    }

    private IEnumerator FadeOutStop()
    {
        float start = _source.volume;
        while (_source.volume > 0f)
        {
            _source.volume -= start * Time.unscaledDeltaTime / Mathf.Max(fadeDuration, 0.0001f);
            yield return null;
        }
        _source.Stop();
        _source.volume = volume;
    }
}