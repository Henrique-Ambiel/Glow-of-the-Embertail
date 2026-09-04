using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera objetoCamera;
    [SerializeField] private float velocidadeRotacao = 10f;
    [SerializeField] private float maxSpeed = 5f;

    private float moveX;
    private CharacterController characterController;
    private PlayerDash playerDash;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();
    }

    private void Update()
    {
        if (playerDash != null && playerDash.IsDashing)
        {
            moveX = Input.GetAxis("Horizontal");

            Vector3 direcao = Quaternion.Euler(0, objetoCamera.transform.eulerAngles.y, 0) * new Vector3(moveX, 0, 0);
            Vector3.Normalize(direcao);
            

            if (direcao != Vector3.zero)
            {
                Quaternion rotacaoDesejada = Quaternion.LookRotation(direcao * 10.0f, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoDesejada, velocidadeRotacao * Time.deltaTime);
            }

        Vector3 direcao =
            Quaternion.Euler(
                0f,
                objetoCamera.transform.eulerAngles.y,
                0f
            )
            * new Vector3(moveX, 0f, 0f);

        if (direcao.sqrMagnitude > 0.0001f)
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