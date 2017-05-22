using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour {

    private bool On = false;
    public bool Working = false;
    public float time = 30;
    private float timer = 0;
    private Animator animator;
    public delegate void Finish(GameObject can);
    public event Finish finish;

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
                //Generate canette 
                if (finish != null)
                    finish(null);
                animator.Play("Click");
                Working = false;
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


}
