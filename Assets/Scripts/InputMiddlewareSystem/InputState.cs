using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public class InputState
    {
        public Vector2 MovementDirection;
        public bool IsCrouching;
        public bool CanJump;
        public Vector2 MouseDirection;
        public bool CanSelectTroops;
        public bool CanMoveTroops;

        public void Copy(InputState copy)
        {
            MovementDirection = copy.MovementDirection;
            IsCrouching = copy.IsCrouching;
            CanJump = copy.CanJump;
            MouseDirection = copy.MouseDirection;
            CanSelectTroops = copy.CanSelectTroops;
            CanMoveTroops = copy.CanMoveTroops;
        }

        public InputState Reset()
        {
            MovementDirection = Vector2.zero;
            IsCrouching = false;
            CanJump = true;
            MouseDirection = Vector2.zero;
            CanSelectTroops = false;
            CanMoveTroops = false;

            return this;
        }
    }
}