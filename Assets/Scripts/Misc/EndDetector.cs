using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDetector : MonoBehaviour {

    public GameObject Villagers;
    public GameObject NewVillagers;
    public ParticleSystem ps;

    private List<Cannibal> cannibals;
    private Corpse m_corpse;

    private bool end = false;
    private bool calledOnce = false;

    void Start()
    {
        cannibals = new List<Cannibal>();
        m_corpse = null;
    }

    void Update()
    {
        if(m_corpse)
        {
            if (cannibals.Count == 2) end = true;
        }

        if(end || Input.GetKeyDown(KeyCode.V))
        {
            if(!calledOnce)
            {
                Villagers.SetActive(false);
                NewVillagers.SetActive(true);
                float i = .5f;
                foreach (Transform t in NewVillagers.transform)
                {
                    Animator anim = t.GetComponent<Animator>();
                    if(anim)
                    {
                        StartCoroutine(PlayDelayedAnim(anim, Random.Range(i, i + .5f)));
                        i += Random.Range(.5f, 1f);
                    }
                }
                ps.Play();
                AkSoundEngine.PostEvent("village_chaudron", gameObject);
                GameManager manager = GameObject.FindObjectOfType<GameManager>();
                if(manager)
                    manager.setEndConditionState(true);
                calledOnce = true;
            }
        }
    }

    IEnumerator PlayDelayedAnim(Animator anim, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        anim.SetTrigger("Active");
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.name == "GlobalCollider")
        {
            Cannibal can = c.transform.parent.parent.GetComponent<Cannibal>();
            if (!cannibals.Contains(can))
                cannibals.Add(can);
        }
        else
        {
            Corpse corpse = c.GetComponent<Corpse>();
            if (corpse)
            {
                m_corpse = corpse;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.name == "GlobalCollider")
        {
            Cannibal can = c.transform.parent.parent.GetComponent<Cannibal>();
            if (cannibals.Contains(can))
                cannibals.Remove(can);
        }
        else
        {
            Corpse corpse = c.GetComponent<Corpse>();
            if (corpse)
            {
                m_corpse = null;
            }
        }
    }
}
