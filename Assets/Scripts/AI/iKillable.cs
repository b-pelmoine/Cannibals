using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iKillable {

    bool IsVulnerable();
    void Kill();
    void ShowKnifeIcon();
}
