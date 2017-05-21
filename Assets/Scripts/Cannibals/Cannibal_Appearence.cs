using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Appearence : MonoBehaviour {

    [SerializeField]
    Cannibal m_cannibal;

    [SerializeField]
    float rotationTime = 2;

    public Transform m_appearenceTransform;

    [SerializeField]
    IconDisplayer iconDisplayer;

    [SerializeField]
    Sprite resurrectIcon;

    public Animator m_animator;

    void Update()
    {
        if (m_cannibal.m_cannibalMovement.CharacterControllerEx.velocity.magnitude != 0)
        {
            Vector3 orientationDirection = m_cannibal.m_cannibalMovement.CharacterControllerEx.velocity;
            orientationDirection.y = 0;
            Orientate(orientationDirection);
        }

        if(m_cannibal.m_cannibalSkill.m_corpse)
            m_animator.SetInteger("corpseCount", m_cannibal.m_cannibalSkill.m_corpse.cannibals.Count);
    }

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

    public void ShowResurrectIcon()
    {
        iconDisplayer.Show(resurrectIcon);
    }
}
