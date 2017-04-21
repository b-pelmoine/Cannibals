using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour, iAI {

    [SerializeField]
    IconDisplayer m_iconDisplayer;

    [SerializeField]
    Sprite knifeSprite;

    public bool IsVulnerable()
    {
        return true;
    }

    public void Kill()
    {

    }

    public void ShowKnifeIcon()
    {
        m_iconDisplayer.Show(knifeSprite);
    }

}
