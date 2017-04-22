using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnifeKillable {

    bool IsKnifeVulnerable();
    void KnifeKill();
    void ShowKnifeIcon();
}
