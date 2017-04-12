using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Appearence : MonoBehaviour {

    [SerializeField]
    Cannibal m_cannibal;

    [SerializeField]
    float rotationTime = 2;

    public Transform m_appearenceTransform;

    float rotationVelocity;
    public void Orientate(Vector3 direction)
    {
        float baseAngle = Vector3.Angle(m_appearenceTransform.forward, direction);

        float newAngle = Mathf.SmoothDampAngle(baseAngle * Mathf.Deg2Rad, 0, ref rotationVelocity, rotationTime) * Mathf.Rad2Deg;
        float deltaAngle = newAngle - baseAngle;

        Vector3 cross = Vector3.Cross(direction, m_appearenceTransform.forward);

        if (Vector3.Dot(cross, m_appearenceTransform.up) < 0)
            deltaAngle *= -1; 

        m_appearenceTransform.Rotate(Vector3.up, deltaAngle);
    }
}
