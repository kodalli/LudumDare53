using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

namespace TNS.InputMiddlewareSystem
{
    public class InputManager : InputMiddleware
    {
        private InputState inputState;

        private void Awake()
        {
            inputState = new InputState();
        }

        public override InputState Process(InputState input)
        {
            input.Copy(inputState);

            return input;
        }

        public void OnMove(InputValue value)
        {
            var movementDirection = value.Get<UnityEngine.Vector2>();

            inputState.MovementDirection = movementDirection;
        }

        public void OnCrouch(InputValue value)
        {
            inputState.IsCrouching = value.Get<float>() > 0.5f;
        }

        public void OnSpacebar(InputValue value)
        {
            BroadcastJump();
        }

        public void OnLeftClick(InputValue value)
        {
            if (value.Get<float>() > 0.5f) {
                BroadcastLeftClickPressed();
            }
            else {
                BroadcastLeftClickReleased();
            }
        }

        public void OnRightClick(InputValue value)
        {
            BroadcastRightClick();
        }

        public void OnMouse(InputValue value)
        {
            inputState.MouseDirection = value.Get<UnityEngine.Vector2>();
        }
    }
}