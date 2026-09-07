using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(FlameManager))]
public sealed class PlayerScaleController : MonoBehaviour
{
    [Header("Escala mínima")]
    [SerializeField, Tooltip("Escala com 0% de chama")]
    private Vector3 minScale = new Vector3(0.6f, 0.6f, 0.6f);

    [Header("Escala máxima")]
    [SerializeField, Tooltip("Escala com 100% de chama")]
    private Vector3 maxScale = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("Suavização da Escala")]
    [SerializeField, Min(1f), Tooltip("Velocidade da transição de tamanho")]
    private float lerpSpeed = 5f;

    [SerializeField] private Transform visualModel;

    private FlameManager flameManager;

    void Awake()
    {
        flameManager = GetComponent<FlameManager>();

        //Se não arrastar o modelo, usa o próprio Transform do objeto
        if (visualModel == null) visualModel = transform;
    }

    void Update()
    {
        UpdatePlayerScale();    
    }

    private void UpdatePlayerScale()
    {
        if (flameManager == null || flameManager.MaxFlame <= 0f) return;

        float flamePercentage = flameManager.CurrentFlame / flameManager.MaxFlame;

        Vector3 targetScale = Vector3.Lerp(minScale, maxScale, flamePercentage);

        visualModel.localScale = Vector3.Lerp(visualModel.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }
}
