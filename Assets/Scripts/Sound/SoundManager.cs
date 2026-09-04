using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum Quantizacao { Imediata, Beat, Compasso }

    public int testIntensity;

    [Serializable]
    public class Camada
    {
        public AudioClip clip;
        public AudioMixerGroup grupo;
        public string parametroVolume;
        [Range(-80f, 20f)] public float volumeAtivoDb = 0f;

        [NonSerialized] public AudioSource source;
        [NonSerialized] public Coroutine fade;
    }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Camada[] camadas;

    [Header("Tempo musical")]
    [SerializeField, Min(1f)] private float bpm = 120f;
    [SerializeField, Min(1)] private int beatsPorCompasso = 4;
    [SerializeField] private Quantizacao quantizacao = Quantizacao.Compasso;

    [Header("Transição")]
    [SerializeField, Min(0f)] private float fadeEmBeats = 2f;
    [SerializeField, Range(-80f, 0f)] private float volumeMudoDb = -80f;

    [Header("Início")]
    [SerializeField, Min(0f)] private double atrasoInicial = 0.3;
    [SerializeField, Min(0)] private int intensidadeInicial = 1;

    private double inicioDsp;

    private double SegundosPorBeat => 60.0 / bpm;

    private void Awake()
    {
        intensidadeInicial = Mathf.Clamp(intensidadeInicial, 0, camadas.Length);
        inicioDsp = AudioSettings.dspTime + atrasoInicial;

        for (int i = 0; i < camadas.Length; i++)
        {
            Camada c = camadas[i];

            c.source = gameObject.AddComponent<AudioSource>();
            c.source.clip = c.clip;
            c.source.loop = true;
            c.source.playOnAwake = false;
            c.source.outputAudioMixerGroup = c.grupo;

            mixer.SetFloat(c.parametroVolume,
                i < intensidadeInicial ? c.volumeAtivoDb : volumeMudoDb);

            if (c.clip != null)
                c.source.PlayScheduled(inicioDsp);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DefinirIntensidade(testIntensity);
        }
    }

    public void DefinirIntensidade(int nivel)
    {
        nivel = Mathf.Clamp(nivel, 0, camadas.Length);
        double quando = ProximoTempoQuantizado();

        for (int i = 0; i < camadas.Length; i++)
            AlterarCamada(camadas[i], i < nivel, quando);
    }

    private void AlterarCamada(Camada c, bool ativa, double quando)
    {
        if (c.fade != null)
            StopCoroutine(c.fade);

        c.fade = StartCoroutine(Fade(c, ativa ? c.volumeAtivoDb : volumeMudoDb, quando));
    }

    private IEnumerator Fade(Camada c, float destinoDb, double quando)
    {
        while (AudioSettings.dspTime < quando)
            yield return null;

        mixer.GetFloat(c.parametroVolume, out float inicioDb);

        double duracao = fadeEmBeats * SegundosPorBeat;
        double fim = quando + duracao;

        if (duracao <= 0.0001)
        {
            mixer.SetFloat(c.parametroVolume, destinoDb);
            c.fade = null;
            yield break;
        }

        while (AudioSettings.dspTime < fim)
        {
            float t = (float)((AudioSettings.dspTime - quando) / duracao);
            t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            mixer.SetFloat(c.parametroVolume, Mathf.Lerp(inicioDb, destinoDb, t));
            yield return null;
        }

        mixer.SetFloat(c.parametroVolume, destinoDb);
        c.fade = null;
    }

    private double ProximoTempoQuantizado()
    {
        double agora = AudioSettings.dspTime;

        if (agora < inicioDsp)
            return inicioDsp;

        if (quantizacao == Quantizacao.Imediata)
            return agora;

        double unidade = quantizacao == Quantizacao.Beat
            ? SegundosPorBeat
            : SegundosPorBeat * beatsPorCompasso;

        double decorrido = agora - inicioDsp;
        return inicioDsp + (Math.Floor(decorrido / unidade) + 1) * unidade;
    }

}
