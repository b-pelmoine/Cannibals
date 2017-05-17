using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticle : MonoBehaviour {

    [SerializeField]
    ParticleSystem m_particleSystem;

    public void PlayParticule()
    {
        m_particleSystem.Play();
    }
}
