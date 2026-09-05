using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum Quantization { Immediate, Beat, Compass }

    public int testIntensity;

    [Serializable]
    public class MusicLayer
    {
        public AudioClip clip;
        public AudioMixerGroup group;
        public string volumeParameter;
        [Range(-80f, 20f)] public float activeVolumeDb = 0f;

        [NonSerialized] public AudioSource source;
        [NonSerialized] public Coroutine fade;
    }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private MusicLayer[] layers;

    [Header("Tempo musical")]
    [SerializeField, Min(1f)] private float bpm = 120f;
    [SerializeField, Min(1)] private int beatsForCompass = 4;
    [SerializeField] private Quantization quantization = Quantization.Compass;

    [Header("Transicao")]
    [SerializeField, Min(0f)] private float fadeInBeats = 2f;
    [SerializeField, Range(-80f, 0f)] private float muteVolumeDb = -80f;

    [Header("Inicio")]
    [SerializeField, Min(0f)] private double initialDelay = 0.3;
    [SerializeField, Min(0)] private int initialIntensity = 1;

    private double initialDsp;

    private double beatForSeconds => 60.0 / bpm;

    private void Awake()
    {
        initialIntensity = Mathf.Clamp(initialIntensity, 0, layers.Length);
        initialDsp = AudioSettings.dspTime + initialDelay;

        for (int i = 0; i < layers.Length; i++)
        {
            MusicLayer c = layers[i];

            c.source = gameObject.AddComponent<AudioSource>();
            c.source.clip = c.clip;
            c.source.loop = true;
            c.source.playOnAwake = false;
            c.source.outputAudioMixerGroup = c.group;

            if (c.clip != null)
                c.source.PlayScheduled(initialDsp);
        }

        StartCoroutine(InitialIntensityApply());
    }

    private IEnumerator InitialIntensityApply()
    {
        yield return null; 
        
        for (int i = 0; i < layers.Length; i++)
        {
            MusicLayer c = layers[i];
            float destiny = i < initialIntensity ? c.activeVolumeDb : muteVolumeDb;
            mixer.SetFloat(c.volumeParameter, destiny);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetIntensity(testIntensity);
        }
    }

    public void SetIntensity(int level)
    {
        level = Mathf.Clamp(level, 0, layers.Length);
        double quando = QuantizationNextTime();

        for (int i = 0; i < layers.Length; i++)
            ChangeMusicLayer(layers[i], i < level, quando);
    }

    private void ChangeMusicLayer(MusicLayer c, bool active, double changeTime)
    {
        if (c.fade != null)
            StopCoroutine(c.fade);

        c.fade = StartCoroutine(Fade(c, active ? c.activeVolumeDb : muteVolumeDb, changeTime));
    }

    private IEnumerator Fade(MusicLayer c, float destinyDb, double changeTime)
    {
        while (AudioSettings.dspTime < changeTime)
            yield return null;

        mixer.GetFloat(c.volumeParameter, out float initialDb);

        double duration = fadeInBeats * beatForSeconds;
        double fim = changeTime + duration;

        if (duration <= 0.0001)
        {
            mixer.SetFloat(c.volumeParameter, destinyDb);
            c.fade = null;
            yield break;
        }

        while (AudioSettings.dspTime < fim)
        {
            float t = (float)((AudioSettings.dspTime - changeTime) / duration);
            t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            mixer.SetFloat(c.volumeParameter, Mathf.Lerp(initialDb, destinyDb, t));
            yield return null;
        }

        mixer.SetFloat(c.volumeParameter, destinyDb);
        c.fade = null;
    }

    private double QuantizationNextTime()
    {
        double now = AudioSettings.dspTime;

        if (now < initialDsp)
            return initialDsp;

        if (quantization == Quantization.Immediate)
            return now;

        double unit = quantization == Quantization.Beat
            ? beatForSeconds
            : beatForSeconds * beatsForCompass;

        double elapsed = now - initialDsp;
        return initialDsp + (Math.Floor(elapsed / unit) + 1) * unit;
    }
}