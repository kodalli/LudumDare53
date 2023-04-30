using UnityEngine;

public static class Utils
{
    public static void Flip(ref bool m_FacingRight, Transform transform, Vector3 m_Destination)
    {
        switch (m_FacingRight)
        {
            case true when transform.position.x > m_Destination.x:
                m_FacingRight = false;
                ScaleFlip(transform);
                break;
            case false when transform.position.x < m_Destination.x:
                m_FacingRight = true;
                ScaleFlip(transform);
                break;
        }
    }

    public static void ScaleFlip(Transform transform)
    {
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}