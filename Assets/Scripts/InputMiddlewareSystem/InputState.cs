using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public class InputState
    {
        public Vector2 MovementDirection;
        public bool IsCrouching;
        public bool CanJump;

        public void Copy(InputState copy)
        {
            MovementDirection = copy.MovementDirection;
            IsCrouching = copy.IsCrouching;
            CanJump = copy.CanJump;
        }

        public InputState Reset()
        {
            MovementDirection = Vector2.zero;
            IsCrouching = false;
            CanJump = true;

            return this;
        }
    }
}