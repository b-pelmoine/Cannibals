using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicKillable : MonoBehaviour, iKillable {

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
