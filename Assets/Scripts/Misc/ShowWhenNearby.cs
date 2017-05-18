using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWhenNearby : MonoBehaviour
{
    public Corpse cScript;
    Image m_Image;
    public Transform corpseTransform;

    public Transform pOne;
    public Transform pTwo;

    // Use this for initialization
    void Start()
    {
        m_Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        bool show = false;
        if (Vector3.Distance(pOne.position, corpseTransform.position) < 2 || Vector3.Distance(pTwo.position, corpseTransform.position) < 2)
            show = true;
        if (cScript.cannibals.Count > 0) show = false;
        m_Image.enabled = show;
    }
}
