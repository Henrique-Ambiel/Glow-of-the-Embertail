using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Transform groundCheckComponent;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Camera objetoCamera;
    [SerializeField] private GameObject CharacterMesh;
    [SerializeField] private float velocidadeRotacao = 10.0f;
    [SerializeField] private float maxSpeed = 0.5f;
    float moveX;
    Animator animator;
    private CharacterController m_CharacterController;
    private PlayerDash m_PlayerDash;

    void Start()
    {
        //animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerDash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        if (!m_PlayerDash.IsDashing)
        {
            moveX = Input.GetAxis("Horizontal");

            Vector3 direcao = Quaternion.Euler(0, objetoCamera.transform.eulerAngles.y, 0) * new Vector3(moveX, 0, 0);
            Vector3.Normalize(direcao);
            

            if (direcao != Vector3.zero)
            {
                Quaternion rotacaoDesejada = Quaternion.LookRotation(direcao * 10.0f, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoDesejada, velocidadeRotacao * Time.deltaTime);
            }

            m_CharacterController.Move(direcao * maxSpeed * Time.deltaTime);
            CharacterMesh.transform.Rotate(Vector3.up, velocidadeRotacao * moveX * Time.deltaTime);

            /*animator.SetFloat("Speed", Math.Abs(playerRigidbody.velocity.x));
            animator.SetFloat("ySpeed", playerRigidbody.velocity.y);*/
        }
    }
}
