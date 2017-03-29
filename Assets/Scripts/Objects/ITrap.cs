using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrap  {

    IWire Wire { get; }

    void Pose(Vector3 position);

}
