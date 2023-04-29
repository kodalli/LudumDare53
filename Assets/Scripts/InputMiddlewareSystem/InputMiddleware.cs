using System;
using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public interface IInputMiddleware
    {
        public event Action OnJump;
        InputState Process(InputState input);
        bool IsEnabled();
    }

    public abstract class InputMiddleware : MonoBehaviour, IInputMiddleware
    {
        public event Action OnJump;
        public abstract InputState Process(InputState input);

        protected virtual void OnEnable()
        {
        }

        protected void BroadcastJump()
        {
            OnJump?.Invoke();
        }

        public bool IsEnabled()
        {
            return enabled;
        }
    }
}