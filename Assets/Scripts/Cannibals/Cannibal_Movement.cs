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
    float m_maxSpeed = 1;


    /// <summary>
    /// Move the cannibal on x/z.
    /// </summary>
    /// <param name="input">input x/z between 0 and 1 </param>
    public void Move(Vector2 input)
    {
        m_characterControllerExt.velocity.x = input.x;
        m_characterControllerExt.velocity.z = input.y;
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
