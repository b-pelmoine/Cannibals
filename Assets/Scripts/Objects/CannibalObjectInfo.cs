using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameObjectType
{
    NONE,
    Bottle,
    Canette,
    Bone,
    ChampiBon,
    ChampiPoison,
    PlasticDuck,
    Cookies
}

[CreateAssetMenu(fileName = "CannibalObjectInfo", menuName ="Cannibal/CannibalObjectInfo")]
public class CannibalObjectInfo : ScriptableObject {

    public Sprite icon;
    public GameObjectType type;
}
