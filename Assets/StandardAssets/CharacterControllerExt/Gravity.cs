using UnityEngine;
using System.Collections;

namespace EquilibreGames
{
    public abstract class Gravity : MonoBehaviour
    {

        public bool isActive;
        public abstract Vector3 GetValue();

    }
}
