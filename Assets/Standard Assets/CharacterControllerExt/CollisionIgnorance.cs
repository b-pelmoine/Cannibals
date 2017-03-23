using UnityEngine;
using System.Collections;


namespace EquilibreGames
{
    public class CollisionIgnorance : MonoBehaviour
    {


        [System.Serializable]
        public struct CollisionIgnoranceInfo
        {
            public LayerMask consideredLayers;
            public string[] name;
        }

        public CollisionIgnoranceInfo[] ignoredCollisions;


        public bool Ignore(GameObject obj, string name)
        {
            foreach (CollisionIgnoranceInfo i in ignoredCollisions)
            {
                if ((((1 << obj.layer) & i.consideredLayers) != 0))
                {
                    foreach (string j in i.name)
                        if (j == name)
                            return true;
                }
            }

            return false;
        }

    }
}
