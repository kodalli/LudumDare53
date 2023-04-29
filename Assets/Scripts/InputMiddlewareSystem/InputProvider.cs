using System;
using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public interface IInputProvider
    {
        public event Action OnJump;
        public InputState GetState();
    }


    [CreateAssetMenu(fileName = "InputProvider", menuName = "InputProvider", order = 0)]
    public class InputProvider : ScriptableObject, IInputProvider
    {
        private InputMiddlewareProcessor middlewareProcessor;
        private InputState inputState;

        private void OnEnable()
        {
            inputState = new InputState();
        }

        public void Set(InputMiddlewareProcessor middlewareProcessor)
        {
            this.middlewareProcessor = middlewareProcessor;
        }

        public event Action OnJump;
        public event Action OnLeftClickPressedAction;
        public event Action OnLeftClickReleasedAction;
        public event Action OnRightClickAction;

        public void BroadcastJump()
        {
            if (inputState.CanJump)
            {
                OnJump?.Invoke();
            }
            else
            {
                Debug.Log("jump disabled");
            }
        }

        public void BroadcastLeftClickPress()
        {
            if (inputState.CanSelectTroops)
            {
                OnLeftClickPressedAction?.Invoke();
            }
            else
            {
                Debug.Log("troop selection disabled");
            }
        }
        public void BroadcastLeftClickReleasedPress()
        {
            if (inputState.CanSelectTroops)
            {
                OnLeftClickReleasedAction?.Invoke();
            }
            else
            {
                Debug.Log("troop selection disabled");
            }
        }

        public void BroadcastRightClick()
        {
            if (inputState.CanMoveTroops)
            {
                OnRightClickAction?.Invoke();
            }
            else
            {
                Debug.Log("troop movement disabled");
            }
        }

        public InputState GetState()
        {
            inputState = middlewareProcessor.Process(inputState.Reset());
            return inputState;
        }
    }
}