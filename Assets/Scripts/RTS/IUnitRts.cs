using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitRts
{
    public void ToggleSelection(bool status);
    public void MoveToPosition(Vector2 destination);
}
