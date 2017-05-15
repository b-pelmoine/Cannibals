using UnityEngine;
using System.Collections;


namespace EquilibreGames
{
    public class StandardGravity : Gravity
    {
        public Vector3 force;

        public override Vector3 GetValue()
        {
            return force;
        }
    }
}
