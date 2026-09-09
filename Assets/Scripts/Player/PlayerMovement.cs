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
    [SerializeField] private Animator animator;

    //Criado para impedir movimento do jogador sem Chama.
    private bool canMove = true;
    FlameManager flameManager;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();
        //animator = GetComponentsInChildren<Animator>()[0];
        flameManager = GetComponent<FlameManager>();
    }

    private void OnEnable()
    {
        if (flameManager != null)
        {
            flameManager.OnFlameExtinguished += StopMovement;
            flameManager.OnFlameStateChanged += HandleFlameStateChanged;
        }
    }
    void OnDisable()
    {
        if (flameManager != null)
        {
            flameManager.OnFlameExtinguished -= StopMovement;
            flameManager.OnFlameStateChanged -= HandleFlameStateChanged;
        }
    }

    void HandleFlameStateChanged(FlameState state)
    {
        if (state == FlameState.Extinguished) StopMovement();
        else canMove = true;
    }

    void StopMovement() => canMove = false;

    private void Update()
    {
        if (playerDash != null && playerDash.IsDashing || !canMove)
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

        if(Debug && Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void StartAudioFootsteps()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    private void StopAudioFootsteps()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}