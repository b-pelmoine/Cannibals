using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAmbianceSound : MonoBehaviour {

    public AkGameObj akGameObj;

    public void DisposeTree(object in_cookie, AkCallbackType in_type, object in_info)
    {
        Debug.Log(gameObject);
    }
}
