using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shump
{
    [RequireComponent(typeof(PlayerInput))]
    public class YInputReader : MonoBehaviour
    {
        PlayerInput playerInput;
        InputAction moveAction;
        
        public Vector2 Move => moveAction.ReadValue<Vector2>();

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
        }
    }
}