using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour, IActivable {

    private bool On = false;
    public bool Working = false;
    public float time = 30;
    private float timer = 0;
    private Animator animator;
    public delegate void Finish(GameObject can);
    public event Finish finish;
    public GameObject prefabCanette;
    public Transform positionCanette;

    public bool poisoned = false;

	void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (On && Working)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                GameObject newCan = Instantiate(prefabCanette);
                newCan.transform.position = positionCanette.position;
                if (poisoned) newCan.GetComponent<Canette>().poisoned = true;
                //Generate canette 
                if (finish != null)
                    finish(newCan);
                animator.Play("Click");
                Working = false;
                poisoned = false;
            }
        }
    }



    public void GenerateurSwitch(bool val)
    {
        if (val)
        {
            On = true;
            if(Working)
                animator.Play("On");
        }
        else
        {
            On = false;
            animator.Play("Click");
        }
    }

    public bool IsOn()
    {
        return On;
    }


    public void Launch()
    {
        timer = 0;
        Working = true;
        if(On)
            animator.Play("On");
    }

    public bool IsActivable(Cannibal cannibal)
    {
        CannibalObject obj = cannibal.m_cannibalSkill.m_cannibalObject;
        if (obj != null && obj.GetComponent<Champignon>() != null)
            return true;
        return false;
    }

    public void Activate(Cannibal cannibal)
    {
        CannibalObject obj = cannibal.m_cannibalSkill.m_cannibalObject;
        if (obj != null) {
            Champignon champ = obj.GetComponent<Champignon>();
            if (champ != null)
            {
                if (champ.type == Champignon.Type.Champoison)
                    poisoned = true;
                cannibal.LooseCannibalObject();
                champ.gameObject.SetActive(false);
            }
        }
    }

    public void ShowIcon()
    {
        //A faire
    }
}
