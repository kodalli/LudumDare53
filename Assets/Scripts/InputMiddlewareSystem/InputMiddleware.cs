using System;
using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public interface IInputMiddleware
    {
        public event Action OnJump;
        public event Action OnLeftClickAction;
        public event Action OnLeftClickReleaseAction;
        public event Action OnRightClickAction;
        InputState Process(InputState input);
        bool IsEnabled();
    }

    public abstract class InputMiddleware : MonoBehaviour, IInputMiddleware
    {
        public event Action OnJump;
        public abstract InputState Process(InputState input);
        public event Action OnLeftClickAction;
        public event Action OnLeftClickReleaseAction;
        public event Action OnRightClickAction;

        protected virtual void OnEnable()
        {
        }

        protected void BroadcastJump()
        {
            OnJump?.Invoke();
        }

        protected void BroadCastLeftClickPress()
        {
            OnLeftClickAction?.Invoke();
        }

        protected void BroadCastLeftClickRelease()
        {
            OnLeftClickReleaseAction?.Invoke();
        }

        protected void BroadCastRightClick()
        {
            OnRightClickAction?.Invoke();
        }

        public bool IsEnabled()
        {
            return enabled;
        }
    }
}