using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentRegister : MonoBehaviour {

	void Start()
    {
        AIAgentManager.registerAIAgent(gameObject);
    }
}
