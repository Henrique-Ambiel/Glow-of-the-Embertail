using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Candle : MonoBehaviour
{
    [Header("Candle Settings")]
    [SerializeField, Min(1.5f)] private float rechargeRate;
    //[SerializeField] private bool isOneTimeUse = true;

    [Header("VFX & Audio")]
    [SerializeField] private GameObject flameVisualEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundType soundType;

    private bool hasBeenLit;

    [Header("Event Reference")]
    [SerializeField] private SoundEventsData soundEvents; 
    
    //Events
    public event Action OnCandleLit;

    void Start()
    {
        if (flameVisualEffect != null)
        {
            //Garante que a chama comece apagada
            flameVisualEffect.SetActive(hasBeenLit);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<FlameManager>(out var flameManager))
        {
            flameManager.AddFlame(rechargeRate * Time.deltaTime);

            //Avisa uma única vez que a vela foi acesa
            if (!hasBeenLit)
            {
                hasBeenLit = true;
                LightCandle(); //Ativa os efeitos sonoros e visuais
                OnCandleLit?.Invoke(); //Notifica outros scripts
            }
        }
    }

    void LightCandle()
    {
        if (flameVisualEffect != null) flameVisualEffect.SetActive(true);

        soundEvents.PlaySFX(soundType);
        
        if (audioSource != null) audioSource.Play();

        Debug.Log($"Vela {gameObject.name} foi acesa pela primeira vez!");
    }
}
