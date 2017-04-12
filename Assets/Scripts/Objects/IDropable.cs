using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDropable
{

    void Throw(float force, Vector3 normalizedDirection);

}

