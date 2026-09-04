using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Vector2 moveDirection;

    public static InputManager Instance;
    
    private PlayerInput playerInput;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }
    
    public static Vector2 GetMoveDirection()
    {
        return Instance.moveDirection;
    }
}
