using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommand : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            if (GameObject.FindObjectOfType<AI.Mamie>() != null) {
                GameObject.FindObjectOfType<AI.Mamie>().DieByDebug();
            }
        }
    }
}
