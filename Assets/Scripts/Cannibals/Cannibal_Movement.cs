using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EquilibreGames;

public class Cannibal_Movement : MonoBehaviour {

    [SerializeField]
    CharacterControllerExt m_characterControllerExt;

    public CharacterControllerExt CharacterControllerEx
    {
        get { return m_characterControllerExt; }
    }

    [Space(20)]
    [SerializeField]
    float m_maxRunSpeed = 1;

    public float MaxRunSpeed
    {
        get { return m_maxRunSpeed; }
    }

    public float m_currentMaxRunSpeed;

    void Awake()
    {
        m_currentMaxRunSpeed = m_maxRunSpeed;
    }

    /// <summary>
    /// Move the cannibal on x/z.
    /// </summary>
    /// <param name="input">input x/z between 0 and 1 </param>
    public void GroundMove(Vector2 input)
    {
        Vector3 input3D = new Vector3(input.x, 0, input.y);
        float length = input3D.magnitude;
        input3D = ExtendedMath.ProjectVectorOnPlane(m_characterControllerExt.GroundNormal, input3D);
        input3D = ExtendedMath.SetVectorLength(input3D, length);

        m_characterControllerExt.velocity.x = m_currentMaxRunSpeed * input3D.x;
        m_characterControllerExt.velocity.y = m_currentMaxRunSpeed * input3D.y;
        m_characterControllerExt.velocity.z = m_currentMaxRunSpeed * input3D.z;

        if(m_characterControllerExt.velocity.magnitude > m_currentMaxRunSpeed)
        {
            m_characterControllerExt.velocity = m_characterControllerExt.velocity .normalized* m_currentMaxRunSpeed;
        }
    }

    /// <summary>
    /// Stop the cannibal on x/z
    /// </summary>
    public void Stop()
    {
        m_characterControllerExt.velocity.x = 0;
        m_characterControllerExt.velocity.z = 0;
        m_characterControllerExt.acceleration.x = 0;
        m_characterControllerExt.acceleration.z = 0;
    }

    public void ResetMaxSpeed()
    {
        m_currentMaxRunSpeed = m_maxRunSpeed;
    }
}
