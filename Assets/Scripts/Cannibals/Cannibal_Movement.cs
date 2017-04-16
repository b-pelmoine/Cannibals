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


    /// <summary>
    /// Move the cannibal on x/z.
    /// </summary>
    /// <param name="input">input x/z between 0 and 1 </param>
    public void GroundMove(Vector2 input)
    {
        Vector3 input3D = new Vector3(input.x, 0, input.y);
        input3D = ExtendedMath.ProjectVectorOnPlane(m_characterControllerExt.GroundNormal, input3D);
        input3D.Normalize();

        m_characterControllerExt.velocity.x = m_maxRunSpeed* input3D.x;
        m_characterControllerExt.velocity.z = m_maxRunSpeed* input3D.z;

        if(m_characterControllerExt.velocity.magnitude > m_maxRunSpeed)
        {
            m_characterControllerExt.velocity = m_characterControllerExt.velocity .normalized* m_maxRunSpeed;
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
}
