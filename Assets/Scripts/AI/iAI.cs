using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iAI {

    bool IsVulnerable();
    void Kill();
    void ShowKnifeIcon();
}
