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
        public event Action OnLeftClickAction;
        public event Action OnLeftClickReleaseAction;
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
                OnLeftClickAction?.Invoke();
            }
            else
            {
                Debug.Log("troop selection disabled");
            }
        }

        public void BroadcastLeftClickRelease()
        {
            if (inputState.CanSelectTroops)
            {
                OnLeftClickReleaseAction?.Invoke();
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