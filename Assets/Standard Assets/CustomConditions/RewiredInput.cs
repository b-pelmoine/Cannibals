using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RewiredInput : MonoBehaviour {


    [Space(20)]
    public string playerInputID = "Cannibal";
    public byte id = 0;


    public Player m_playerInput { get; private set;}

    void Awake()
    {
        m_playerInput = ReInput.players.GetPlayer(playerInputID + id);
    }

}
