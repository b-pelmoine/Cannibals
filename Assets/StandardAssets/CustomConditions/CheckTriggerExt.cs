using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Category("System Events")]
[EventReceiver("OnTriggerEnter", "OnTriggerExit")]
public class CheckTriggerExt<T> : ConditionTask<Collider> where T : class
{
    class Info
    {
        public T script;
        public List<Collider> colliders = new List<Collider>();
    }

    public TriggerTypes checkType = TriggerTypes.TriggerEnter;
    public LayerMask layerMask;

    public bool specifiedTagOnly;
    [TagField]
    public string objectTag = "Untagged";
    [BlackboardOnly]
    public BBParameter<List<T>> savedList;

    private bool stay;

    List<Info> infos = new List<Info>();

    protected override string info
    {
        get { return checkType.ToString() + (specifiedTagOnly ? (" '" + objectTag + "' tag") : ""); }
    }

    protected override bool OnCheck()
    {
        if (checkType == TriggerTypes.TriggerStay)
        {
            savedList.value.Clear();

            foreach(Info i in infos)
            {
                savedList.value.Add(i.script);
            }

            for (int i = infos.Count -1 ; i >= 0; i--)
            {
                for (int j = infos[i].colliders.Count - 1; j >= 0; j--)
                {
                    if (!infos[i].colliders[j].enabled)
                    {
                        infos[i].colliders.RemoveAt(j);

                        if (infos[i].colliders.Count == 0)
                        {
                            savedList.value.Remove(infos[i].script);
                            infos.RemoveAt(i);
                        }
                    }
                }
            }

            return savedList.value.Count > 0;
        }

        return false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if ((((1 << other.gameObject.layer) & layerMask) != 0) && (!specifiedTagOnly || other.gameObject.tag == objectTag))
        {
            stay = true;
            T script = other.GetComponentInParent<T>();

            if (script != null)
            {
                Info info = infos.Find(x => { return x.script == script; });

                if (info != null)
                    info.colliders.Add(other);
                else
                {
                    info = new Info();
                    info.script = script;
                    info.colliders.Add(other);
                    infos.Add(info);
                }

                if (checkType == TriggerTypes.TriggerEnter || checkType == TriggerTypes.TriggerStay)
                {
                    YieldReturn(true);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if ((((1 << other.gameObject.layer) & layerMask) != 0) && (!specifiedTagOnly || other.gameObject.tag == objectTag))
        {
            stay = false;

            T script = other.GetComponentInParent<T>();

            if (script != null)
            {
                Info info = infos.Find(x => { return x.script.Equals(script); });

                if (info != null)
                {
                    info.colliders.Remove(other);
                    if (info.colliders.Count == 0)
                        infos.Remove(info);
                }

                if (checkType == TriggerTypes.TriggerExit)
                {
                    YieldReturn(true);
                }
            }
        }
    }
}
