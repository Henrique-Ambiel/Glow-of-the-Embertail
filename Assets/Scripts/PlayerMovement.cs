using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera objetoCamera;
    [SerializeField] private float velocidadeRotacao = 10f;
    [SerializeField] private float maxSpeed = 5f;

    private float moveX;
    private CharacterController characterController;
    private PlayerDash playerDash;
    private Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();
        animator = GetComponentsInChildren<Animator>()[0];
    }

    private void Update()
    {
        if (playerDash != null && playerDash.IsDashing)
        {
            return;
        }

        moveX = Input.GetAxis("Horizontal");

        Vector3 direcao =
            Quaternion.Euler(
                0f,
                objetoCamera.transform.eulerAngles.y,
                0f
            )
            * new Vector3(moveX, 0f, 0f);

        float movimento = direcao.sqrMagnitude;
        animator.SetFloat("Movimento",  movimento);

        if (movimento > 0.0001f)
        {
            direcao.Normalize();

            Quaternion rotacaoDesejada =
                Quaternion.LookRotation(
                    direcao,
                    Vector3.up
                );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacaoDesejada,
                velocidadeRotacao * Time.deltaTime
            );
        }

        characterController.Move(
            direcao * maxSpeed * Time.deltaTime
        );
    }
}