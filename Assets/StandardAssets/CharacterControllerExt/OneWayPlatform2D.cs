using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EquilibreGames
{
    public class OneWayPlatform2D : MonoBehaviour
    {
        [SerializeField]
        new Collider2D collider;

        [SerializeField, Range(0,360)]
        float passAngle;

        [SerializeField, Range(0, 360)]
        float rotation;

        float passAngleRad;
        float rotationRad;

        public bool CanPassThrought(Vector2 from)
        {
            if (from == Vector2.zero)
                return false;

            passAngleRad = passAngle * Mathf.Deg2Rad;
            rotationRad = rotation * Mathf.Deg2Rad;

            return  (180f-Vector2.Angle(new Vector2(Mathf.Cos( rotationRad - passAngleRad / 2f), Mathf.Sin(rotationRad - passAngleRad / 2f)), from)) <= passAngle / 2f;
        }

 #if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
        void OnDrawGizmos()
        {
            if (collider == null)
                return;

            passAngleRad = passAngle * Mathf.Deg2Rad;
            rotationRad = rotation * Mathf.Deg2Rad;
            
            Handles.color = new Color(0.3f, 0.8f, 0f, 0.2f);

            Handles.DrawSolidArc(collider.bounds.center, Vector3.forward, new Vector2(Mathf.Cos(  rotationRad - passAngleRad), Mathf.Sin( rotationRad - passAngleRad)), passAngle, 5);
            Debug.DrawRay(collider.bounds.center, new Vector2(Mathf.Cos(  rotationRad - passAngleRad / 2f), Mathf.Sin(  rotationRad - passAngleRad / 2f)) * 5, Color.red, 0.2f);
        }

#endif

    }
}
