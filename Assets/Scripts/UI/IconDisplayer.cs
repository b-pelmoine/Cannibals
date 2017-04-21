using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconDisplayer : MonoBehaviour {


    [SerializeField]
    float m_displayTime = 0.2f;

    [SerializeField]
    Image m_image;

    float m_timer = -1;

    void Awake()
    {
        m_image.enabled = false;
    }

	// Update is called once per frame
	void Update () {
		if(m_timer != -1 && Time.time > m_timer)
        {
            m_image.enabled = false;
            m_timer = -1;
        }
	}

    public void Show(Sprite sprite)
    {
        m_image.enabled = true;
        m_image.sprite = sprite;
        m_timer = Time.time + m_displayTime;
    }
}
