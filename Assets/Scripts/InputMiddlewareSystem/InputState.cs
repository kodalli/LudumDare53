using UnityEngine;

namespace TNS.InputMiddlewareSystem
{
    public class InputState
    {
        public Vector2 MovementDirection;
        public Vector2 MouseDirection;
        public bool IsCrouching;
        public bool CanJump = true;
        public bool CanSelectTroops = true;
        public bool CanMoveTroops = true;

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
            MouseDirection = Vector2.zero;
            IsCrouching = false;
            CanJump = true;
            CanSelectTroops = true;
            CanMoveTroops = true;

            return this;
        }
    }
}