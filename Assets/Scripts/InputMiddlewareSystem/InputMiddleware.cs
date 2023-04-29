using System;
using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public interface IInputMiddleware
    {
        public event Action OnJump;
        public event Action OnLeftClickPressedAction;
        public event Action OnRightClickAction;
        InputState Process(InputState input);
        bool IsEnabled();
    }

    public abstract class InputMiddleware : MonoBehaviour, IInputMiddleware
    {
        public event Action OnJump;
        public abstract InputState Process(InputState input);
        public event Action OnLeftClickPressedAction;
        public event Action OnLeftClickReleasedAction;
        public event Action OnRightClickAction;

        protected virtual void OnEnable()
        {
        }

        protected void BroadcastJump()
        {
            OnJump?.Invoke();
        }

        protected void BroadcastLeftClickPressed()
        {
            OnLeftClickPressedAction?.Invoke();
        }
        protected void BroadcastLeftClickReleased()
        {
            OnLeftClickPressedAction?.Invoke();
        }

        protected void BroadcastRightClick()
        {
            OnRightClickAction?.Invoke();
        }

        public bool IsEnabled()
        {
            return enabled;
        }
    }
}