using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public class InputMiddlewareProcessor : MonoBehaviour
    {
        private const string processor = "InputMiddlewareProcessor";

        [SerializeField] private InputProvider provider;
        private IInputMiddleware[] middleware;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            var target = Instantiate(Resources.Load(processor)) as GameObject;
            if (target == null)
                throw new ApplicationException();
            target.name = processor;

            DontDestroyOnLoad(target);
        }

        private void Awake()
        {
            middleware = GetComponents<IInputMiddleware>();

            provider.Set(this);
            foreach (var middleware in middleware)
            {
                middleware.OnJump += provider.BroadcastJump;
                middleware.OnLeftClickReleasedAction += provider.BroadcastLeftClickReleasedPress;
                middleware.OnLeftClickPressedAction += provider.BroadcastLeftClickPress;
                middleware.OnRightClickAction += provider.BroadcastRightClick;
            }
        }

        public InputState Process(InputState input)
        {
            foreach (var t in middleware)
            {
                if (t.IsEnabled())
                {
                    input = t.Process(input);
                }
            }

            return input;
        }
    }
}