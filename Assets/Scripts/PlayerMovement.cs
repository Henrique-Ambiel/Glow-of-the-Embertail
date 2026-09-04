using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera objetoCamera;
    [SerializeField] private float velocidadeRotacao = 10f;
    [SerializeField] private float maxSpeed = 5f;

    public bool Debug = true;

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

        if(Debug && Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}