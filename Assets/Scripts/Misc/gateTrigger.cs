using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateTrigger : MonoBehaviour {

    public GameObject Villagers;
    public GameObject NewVillagers;
    public ParticleSystem ps;

    private List<Cannibal> cannibals;
    private Corpse m_corpse;

    bool end = false;
    bool calledOnce = false;

    // Use this for initialization
    void Start () {
        cannibals = new List<Cannibal>();
        m_corpse = null;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_corpse)
        {
            if (cannibals.Count == 2) end = true;
        }

        if (end || Input.GetKeyDown(KeyCode.V))
        {
            if (!calledOnce)
            {
                Villagers.SetActive(false);
                NewVillagers.SetActive(true);
                float i = .5f;
                foreach (Transform t in NewVillagers.transform)
                {
                    Animator anim = t.GetComponent<Animator>();
                    if (anim)
                    {
                        StartCoroutine(PlayDelayedAnim(anim, Random.Range(i, i + .5f)));
                        i += Random.Range(.5f, 1f);
                    }
                }
                ps.Play();
                AkSoundEngine.PostEvent("village_chaudron", gameObject);
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
        if (c.name == "GlobalCollider")
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
