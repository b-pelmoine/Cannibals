using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour, IActivable {

    private bool On = false;
    public IconDisplayer icon;
    public bool Working = false;
    public float time = 30;
    private float timer = 0;
    private Animator animator;
    public delegate void Finish(GameObject can);
    public event Finish finish;
    public GameObject prefabCanette;
    public Transform positionCanette;
    public int production = 3;
    int produced = 0;
    bool finished = false;

    int hasCan = 0;
    List<GameObject> can = new List<GameObject>();

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
            if (finished && timer > 1.0f)
            {
                GameObject newCan = Instantiate(prefabCanette);
                newCan.transform.position = positionCanette.position;
                if (poisoned) newCan.GetComponent<Canette>().poisoned = true;
                //Generate canette 
                if (finish != null)
                    finish(newCan);
                produced++;
                if (produced == production)
                {
                    Working = false;
                    produced = 0;
                    animator.Play("Idle");
                    AkSoundEngine.PostEvent("machine_end_complete", gameObject);
                    finished = false;
                }
                else
                {
                    timer = 0;
                }
                can.Add(newCan);
                hasCan ++;
                //poisoned = true;
            }
            else if(timer > time)
            {
                finished = true;
                timer = 0;
            }
        }
    }

    public GameObject takeCan()
    {
        hasCan--;
        GameObject c = can[can.Count - 1];
        can.Remove(c);
        return c;
    }

    public bool CanReady()
    {
        return hasCan>0;
    }



    public void GenerateurSwitch(bool val)
    {
        if (val)
        {
            On = true;
            if (Working)
            {
                animator.Play("ToOn");
                AkSoundEngine.PostEvent("machine_work", gameObject);
            }
        }
        else
        {
            On = false;
            animator.Play("Idle");
            AkSoundEngine.PostEvent("machine_end", gameObject);
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
        if (On)
        {
            animator.Play("ToOn");
            AkSoundEngine.PostEvent("machine_work", gameObject);
        }
    }

    public void Fill()
    {
        AkSoundEngine.PostEvent("machine_gloups", gameObject);
    }

    public bool IsActivable(CannibalObject cannibalObject)
    {
        if (cannibalObject is Champignon)
            return true;

        return false;
    }

    public void Activate(CannibalObject cannibalObject)
    {
        if (cannibalObject is Champignon)
        {
            if (((Champignon)cannibalObject).type == Champignon.Type.Champoison)
                poisoned = true;

            ((Champignon)cannibalObject).gameObject.SetActive(false);
        }
    }

    public void ShowIcon()
    {
        icon.Show();
    }
}
