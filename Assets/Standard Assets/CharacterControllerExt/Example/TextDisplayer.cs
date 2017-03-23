using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EquilibreGames{
    public class TextDisplayer : MonoBehaviour
    {

        [SerializeField]
        string info;

        [SerializeField]
        Vector2 offset;

        void OnDrawGizmos()
        {
            Handles.Label(this.transform.position + (Vector3)offset, info);
        }
    }
}
