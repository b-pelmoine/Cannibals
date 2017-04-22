using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicKillable : MonoBehaviour, IKnifeKillable {

    [SerializeField]
    IconDisplayer m_iconDisplayer;

    [SerializeField]
    Sprite knifeSprite;

    public bool IsKnifeVulnerable()
    {
        return true;
    }

    public void KnifeKill()
    {

    }

    public void ShowKnifeIcon()
    {
        m_iconDisplayer.Show(knifeSprite);
    }

}
