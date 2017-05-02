using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Category("System Events")]
[EventReceiver("OnTriggerEnter", "OnTriggerExit")]
public class CheckTriggerExt<T> : ConditionTask<Collider> 
{
    public TriggerTypes checkType = TriggerTypes.TriggerEnter;
    public LayerMask layerMask;

    public bool specifiedTagOnly;
    [TagField]
    public string objectTag = "Untagged";
    [BlackboardOnly]
    public BBParameter<List<T>> savedList;

    private bool stay;
    List<Collider> colliders = new List<Collider>();

    List<T> stayList = new List<T>();
    List<Collider> stayCollider = new List<Collider>();

    protected override string info
    {
        get { return checkType.ToString() + (specifiedTagOnly ? (" '" + objectTag + "' tag") : ""); }
    }

    protected override bool OnCheck()
    {
        if (checkType == TriggerTypes.TriggerStay)
        {
            colliders.Clear();
            savedList.value.Clear();

            foreach (Collider c in stayCollider)
                colliders.Add(c);

            foreach (T t in stayList)
                savedList.value.Add(t);

            for (int i = colliders.Count - 1; i >= 0; i--)
            {
                if (!colliders[i].enabled)
                {
                    colliders.RemoveAt(i);
                    savedList.value.RemoveAt(i);
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
                if(!savedList.value.Exists(x => { return x.Equals(script); }))
                {
                    savedList.value.Add(script);
                    colliders.Add(other);

                    stayList.Add(script);
                    stayCollider.Add(other);
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
                colliders.Remove(other);
                savedList.value.Remove(script);

                stayList.Remove(script);
                stayCollider.Remove(other);

                if (checkType == TriggerTypes.TriggerExit)
                {
                    YieldReturn(true);
                }
            }
        }
    }
}
