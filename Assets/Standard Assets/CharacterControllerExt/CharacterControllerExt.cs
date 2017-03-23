using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// All source inspiration provide from Roystan Ross blog : https://roystanross.wordpress.com/ 


namespace EquilibreGames
{
    public class CharacterControllerExt : MonoBehaviour
    {
        public enum MODE { _2D, _3D }

        [System.Serializable]
        public class ColliderInfo
        {
            public float radius;
            [SerializeField]
            private Transform transform;
            [SerializeField]
            private Vector2 offset;
            public string name;

            public Vector3 position
            {
                get
                {
                    return transform.TransformPoint(offset);
                }
            }
            public Vector3 up
            {
                get
                {
                    return transform.up;
                }
            }
            public Vector3 right
            {
                get
                {
                    return transform.right;
                }
            }
        }

        private class GroundHit
        {
            public Vector2 point { get; private set; }
            public Vector2 normal { get; private set; }
            public float distance { get; private set; }

            public GroundHit(Vector3 point, Vector3 normal, float distance)
            {
                this.point = point;
                this.normal = normal;
                this.distance = distance;
            }
        }

        public class Repulsion
        {
            private RaycastHit2D hit;
            private float repulsionFactor;
            private Vector2 velocity;

            public Repulsion(Vector2 velocity, float repulsionFactor, RaycastHit2D hit)
            {
                this.hit = hit;
                this.repulsionFactor = repulsionFactor;
                this.velocity = velocity;
            }

            public void Repulse()
            {
                float dot = Mathf.Abs(Vector2.Dot(velocity.normalized, hit.normal));
                
                hit.collider.attachedRigidbody.AddForceAtPosition((-hit.normal * this.velocity.magnitude * dot * repulsionFactor), hit.point, ForceMode2D.Impulse);
            }
        }

        [System.Serializable]
        public struct CharacterController2DColliders
        {
           public ColliderInfo[] colliders;
        }

        public MODE mode = MODE._2D;

        [SerializeField]
        Transform characterTransform;

        public Transform CharacterTransform
        {
            get { return characterTransform; }
        }


        [SerializeField]
        CharacterController2DColliders[] characterControllerColliders;

        [SerializeField][Tooltip("Between all circleColliders, where is the collider for character's feet ?")]
        int feetIndex;

        [SerializeField]
        List<Collider2D> ignoredColliders2D = new List<Collider2D>();

        [SerializeField]
        List<Collider> ignoredColliders3D = new List<Collider>();

        //OLD CODE -- 2016_07_31
        // [SerializeField]
        //float maxGroundDistance = 0.5f;

        public LayerMask collisionLayer;
        public LayerMask walkableLayer;


        [SerializeField] [Tooltip("The character will not be considered as grounded if the slope has an angle > to maxGroundAngle")]
        float maxGroundAngle = 2f;

        [SerializeField] [Tooltip("The character velocity will be set to a different value if it touch a ceiling")]
        float maxCeilingAngle = 2f;

        // Set of usefull variable used to construct the character controller
        public float toleranceConst = 0.01f;
        public int pushBackResolution = 2;

        [Space(20)][Tooltip("The current configuration of your circle used")]
        public int circleConfigurationIndex = 0;


        [Space(5)][Tooltip("Object in this layer will be repulsed by the characterController2D")]
        public LayerMask repulsedLayer;
        [Tooltip(" >1 mean that your character eject objects.  < 0 is not coherent but why not ?")]
        public float repulsionFactor = 3f;

        [Space(10)]
        [Tooltip("Generals rules :\n1- When your character collide with surface < maxAngle : velocity.y is decrease\n2- When your character collide with surface > maxAngle : velocity.x is decrease")]
        public bool useGeneralPlatformerVelocityRule = true;
        [Tooltip("1- If you want to increase the slow-down minimise this value\n2- Bouncy character can be create with slowFactor <1\n3- Value >1 are less coherent")]
        public float slowFactor = 1;

        [Space(20)][Tooltip("Use this instead of SetVelocity (better resolution)")]
        public Gravity gravity;

        [SerializeField][Range(0,200)] [Tooltip("The length of the array for collisions. If more collisions are detected, it will be ignored")]
        int maxCollisionPerFrame = 20;

        /// <summary>
        /// This is the acceleration of the controller, this value is reset each FixedUpdate;
        /// </summary>
        public Vector3 acceleration;
        /// <summary>
        /// The velocity of the characterController
        /// </summary>
        public Vector3 velocity;

        /// <summary>
        /// Delegate for repulsion algorithme
        /// </summary>
        /// <param name="colInfo"></param>
        /// <param name="col"></param>
        /// <param name="hit"></param>
        /// <returns>Return null if you don't want to repulse</returns>
        public delegate Repulsion CharacterController2DRepulsionDelegate(Vector2 velocity, ColliderInfo colInfo,RaycastHit2D hit);
        public CharacterController2DRepulsionDelegate OnRepulseObject;

        public delegate bool CharacterController2DDelegate(ColliderInfo colInfo, GameObject colGameObject, Vector2 hit = default(Vector2));
        public CharacterController2DDelegate OnHit;

        private bool isGrounded;
        public bool IsGrounded
        {
            get { return isGrounded; }
        }

        private Vector3 groundNormal;
        public Vector3 GroundNormal
        {
            get { return groundNormal; }
        }

        private bool[] contactDebugguer;

        private GroundHit primaryGround;
        private GroundHit nearGround;
        private GroundHit farGround;
        private GroundHit stepGround;
        private GroundHit flushGround;

        private const float groundingUpperBoundAngle = 60.0f;
        private const float groundingMaxPercentFromCenter = 0.85f;
        private const float groundingMinPercentFromcenter = 0.50f;
        private Collider2D[] collisionsResult2D;
        private Collider[] collisionsResult3D;

#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
        public bool debugguer = true;
#endif


        void Awake()
        {
            collisionsResult2D = new Collider2D[maxCollisionPerFrame];
            collisionsResult3D = new Collider[maxCollisionPerFrame];

            int max = 0;

            foreach(CharacterController2DColliders i in characterControllerColliders)
            {
                if (i.colliders.Length > max)
                    max = i.colliders.Length;
            }
             contactDebugguer = new bool[max];

#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG  
            if (debugguer && characterControllerColliders.Length == 0)
            {
                Debug.LogWarning("No circle collider found");
            }
#endif
        }

        void FixedUpdate()
        {
            UpdatePosition(Time.fixedDeltaTime);

            if (characterControllerColliders.Length <= circleConfigurationIndex)
            {
                if (characterControllerColliders.Length != 0)
                    Debug.LogWarning("Mode doesn't correspond to number of CharacterController2DColliders !");

                return;
            }

            if (mode == MODE._2D)
            {
                RepulseObjects2D();

                HandleCollision2D(0, pushBackResolution);
                ProbeGround2D();
                isGrounded = CheckGround2D(characterControllerColliders[circleConfigurationIndex].colliders[feetIndex].radius + toleranceConst, out groundNormal);
            }
            else
            {
                HandleCollision3D(0, pushBackResolution);
                ProbeGround3D();
                isGrounded = CheckGround3D(characterControllerColliders[circleConfigurationIndex].colliders[feetIndex].radius + toleranceConst, out groundNormal);
            }

            acceleration = Vector3.zero;
        }

        void OnDisable()
        {
            velocity = Vector2.zero;
        }

        /// <summary>
        /// This function update the position of the character transform, using Euler integration algorithm.
        /// </summary>
        /// <param name="time"></param>
        void UpdatePosition(float time)
        {                        
            if(gravity!= null && gravity.isActive && !isGrounded)
                acceleration += gravity.GetValue();

            velocity += acceleration * time;
            characterTransform.position += velocity*time + 0.5f* acceleration * time * time;
        }


        /// <summary>
        /// Get the velocity the next time characterControler will update.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Vector3 GetNextVelocityValue(float time)
        {
            return velocity + acceleration * time;
        }

        public void Jump(Vector3 force)
        {
            velocity += force;
            isGrounded = false;
        }

        /// <summary>
        /// Will push back the objects which touch the circle collider of the character controller
        /// </summary>
        public void RepulseObjects2D()
        {
            //Check for repulsed object.
            //CARE IT'S NOT FULLY PHYSICS COHERENT (AN OBJECT DON'T ADD FORCE ONLY WITH ITS VELOCITY)
            if (repulsedLayer != default(LayerMask))
            {
                foreach (ColliderInfo colInfo in characterControllerColliders[circleConfigurationIndex].colliders)
                {
                    RaycastHit2D hit = Physics2D.CircleCast(colInfo.position - this.velocity.normalized*toleranceConst, colInfo.radius, this.velocity, this.velocity.magnitude * Time.fixedDeltaTime);

                    Collider2D col = hit.collider;

                    if (hit && (col.attachedRigidbody != null && !col.attachedRigidbody.isKinematic))
                        {
                            if (hit)
                            {
                                if (OnRepulseObject != null)
                                {
                                    Repulsion r = OnRepulseObject(this.velocity, colInfo, hit);

                                    if (r != null)
                                        r.Repulse();
                                }
                                else
                                    new Repulsion(this.velocity, repulsionFactor, hit).Repulse();
                            }

                        }
                     }
                }
            }


        /// <summary>
        /// Handle all collision with the character and push back it if collision was found
        /// </summary>
        public void HandleCollision2D(int currentDepth, int DepthResolution)
        {

#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
            int cpt = 0;

            if (debugguer)
            {
                for (int i = 0; i < contactDebugguer.Length; i++)
                {
                    contactDebugguer[i] = false;
                }
            }
#endif

            bool contact = false;
            Vector3 positionMemory = characterTransform.position;

            foreach (ColliderInfo colInfo in characterControllerColliders[circleConfigurationIndex].colliders)
            {
                int collisionLength = Physics2D.OverlapCircleNonAlloc(colInfo.position, colInfo.radius, collisionsResult2D, collisionLayer);

                for(int i =0; i < collisionLength; i++)
                {
                    Collider2D col = collisionsResult2D[i];

                    //Don't collide with trigger
                    if (col.isTrigger || ignoredColliders2D.Contains(col))
                        continue;


                    OneWayPlatform2D oneWayPlatform = col.gameObject.GetComponent<OneWayPlatform2D>();

                    //If there is a OneWayPlatform, you can determine if you pass or not with velocity (first check) !
                    if (oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity))
                    {
                        continue;
                    }

                    CollisionIgnorance collisionIngorance = col.gameObject.GetComponent<CollisionIgnorance>();
                    if (collisionIngorance != null && collisionIngorance.Ignore(this.gameObject, colInfo.name))
                        continue;

                    if (oneWayPlatform == null || (colInfo == characterControllerColliders[circleConfigurationIndex].colliders[feetIndex]))
                    {
                        //Find the closest point on the collider2D shape
                        Vector2 contactPoint = default(Vector2);

                        if (col is BoxCollider2D)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((BoxCollider2D)col, colInfo.position);
                        else if (col is CircleCollider2D)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((CircleCollider2D)col, colInfo.position);
                        else if (col is PolygonCollider2D)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((PolygonCollider2D)col, colInfo.position);



#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
                        if (debugguer)
                            DebugDraw.DrawMarker(contactPoint, 2.0f, Color.red, 0f, false);
#endif
                        //Calcul the normal direction
                        Vector2 direction = contactPoint - (Vector2)colInfo.position;


                        if (!direction.Equals(Vector2.zero))
                        {

                            //Check if our circle collider center is inside the collider
                            //toleranceConst is used to avoid edge problem
                            bool facingNormal = false;

                            if (!(col is PolygonCollider2D))
                            {
                                Vector2 dir1 = (colInfo.position - col.bounds.center);
                                Vector2 dir2 = (Vector2)colInfo.position - contactPoint;

                                //Optimised way to check if vector are in the same direction
                                //(Math rule the world, that's it !)
                                if (Vector2.Dot(dir1, dir2) > 0)
                                {
                                    facingNormal = true;
                                }
                            }
                            else
                            {
                                if (!col.OverlapPoint(colInfo.position))
                                    facingNormal = true;
                            }



                            //If we facingNormal check if an over collision resolution push back the character off.
                            //This can't be done if we were inside the collider because radius can be << contactPoint and still collide.
                            if (facingNormal)
                            {
                                if (Vector2.Distance(colInfo.position, contactPoint) < colInfo.radius)
                                    direction = direction.normalized * (colInfo.radius - direction.magnitude) * -1;
                                else
                                    continue;
                            }
                            //Else we had to push the character back of the collider
                            else
                            {
                                direction = direction.normalized * (colInfo.radius + direction.magnitude);
                            }

                            if (oneWayPlatform != null && (oneWayPlatform.CanPassThrought(-direction)))
                            {
                                continue;
                            }
                            //We are now sure we collide with it, so call the custom delegate
                            if (OnHit != null && !OnHit(colInfo, col.gameObject, contactPoint))
                                continue;


                            //Change the characterTransform position to be on the collider shape.
                            characterTransform.position = (Vector2)colInfo.position + direction;

                            //Because the transform for colInfo is not necessary the character transform, substract the localPosition of this one;
                            characterTransform.position = 2 * (Vector2)characterTransform.position - (Vector2)colInfo.position;
                            characterTransform.position = new Vector3(characterTransform.position.x, characterTransform.position.y, positionMemory.z);


                            if (useGeneralPlatformerVelocityRule && oneWayPlatform == null)
                            {
                                Vector2 normal = ((Vector2)colInfo.position - contactPoint).normalized;
                                float angle = Vector2.Angle(Vector2.up, normal);

                                float dot = Vector2.Dot(velocity.normalized, normal);


                                if (angle > 180f - maxCeilingAngle)
                                {
                                    velocity.y = 0;
                                }
                                else if (angle < maxGroundAngle)
                                    velocity.y = (-slowFactor * dot) * velocity.y;
                                else
                                    velocity.x = (-slowFactor * dot) * velocity.x;
                            }


#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
                            if (debugguer)
                                contactDebugguer[cpt] = true;
#endif

                            contact = true;
                        }
                    }
                }
            }
#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
                if (debugguer)
                    cpt++;
#endif

                //Recursively do again the HandleCollision function to resolve this again.
                if (currentDepth < pushBackResolution && contact)
                {
                    HandleCollision2D(currentDepth + 1, pushBackResolution);
                }           
        }



        /// <summary>
        /// Handle all collision with the character and push back it if collision was found
        /// </summary>
        public void HandleCollision3D(int currentDepth, int DepthResolution)
        {

#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
            int cpt = 0;

            if (debugguer)
            {
                for (int i = 0; i < contactDebugguer.Length; i++)
                {
                    contactDebugguer[i] = false;
                }
            }
#endif

            bool contact = false;
            Vector3 positionMemory = characterTransform.position;

            foreach (ColliderInfo colInfo in characterControllerColliders[circleConfigurationIndex].colliders)
            {
                int collisionLength = Physics.OverlapSphereNonAlloc(colInfo.position, colInfo.radius, collisionsResult3D, collisionLayer);

                for (int i = 0; i < collisionLength; i++)
                {
                    Collider col = collisionsResult3D[i];

                    //Don't collide with trigger
                    if (col.isTrigger || ignoredColliders3D.Contains(col))
                        continue;


                   // OneWayPlatform2D oneWayPlatform = col.gameObject.GetComponent<OneWayPlatform2D>();

                    //If there is a OneWayPlatform, you can determine if you pass or not with velocity (first check) !
                  /*  if (oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity))
                    {
                        continue;
                    }*/

                    CollisionIgnorance collisionIngorance = col.gameObject.GetComponent<CollisionIgnorance>();
                    if (collisionIngorance != null && collisionIngorance.Ignore(this.gameObject, colInfo.name))
                        continue;

                    if (/*oneWayPlatform == null ||*/ (colInfo == characterControllerColliders[circleConfigurationIndex].colliders[feetIndex]))
                    {
                        //Find the closest point on the collider2D shape
                        Vector3 contactPoint = default(Vector3);

                        if (col is BoxCollider)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((BoxCollider)col, colInfo.position);
                        else if (col is SphereCollider)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((SphereCollider)col, colInfo.position);
                        else if (col is TerrainCollider)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((TerrainCollider)col, colInfo.position, colInfo.radius);
                        else if (col is MeshCollider)
                            contactPoint = ExtendedMath.ClosestPointOnSurface((MeshCollider)col, colInfo.position, colInfo.radius);




#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
                        if (debugguer)
                            DebugDraw.DrawMarker(contactPoint, 2.0f, Color.red, 0f, false);
#endif
                        //Calcul the normal direction
                        Vector3 direction = contactPoint - colInfo.position;


                        if (!direction.Equals(Vector3.zero))
                        {

                            //Check if our circle collider center is inside the collider
                            //toleranceConst is used to avoid edge problem
                            bool facingNormal = Physics.SphereCast(new Ray(colInfo.position, direction.normalized), toleranceConst, direction.magnitude + toleranceConst, collisionLayer);


                            // Orient and scale our vector based on which side of the normal we are situated
                            if (facingNormal)
                            {
                                if (Vector3.Distance(colInfo.position, contactPoint) < colInfo.radius)
                                {
                                    direction = direction.normalized * (colInfo.radius - direction.magnitude) * -1;
                                }
                                else
                                {
                                    // A previously resolved collision has had a side effect that moved us outside this collider
                                    continue;
                                }
                            }
                            else
                            {
                                direction = direction.normalized * (colInfo.radius + direction.magnitude);
                            }


/*
                            if (oneWayPlatform != null && (oneWayPlatform.CanPassThrought(-direction)))
                            {
                                continue;
                            } */
                            //We are now sure we collide with it, so call the custom delegate
                            if (OnHit != null && !OnHit(colInfo, col.gameObject, contactPoint))
                                continue;


                            //Change the characterTransform position to be on the collider shape.
                            characterTransform.position = colInfo.position + direction;

                            //Because the transform for colInfo is not necessary the character transform, substract the localPosition of this one;
                            characterTransform.position = 2 * characterTransform.position - colInfo.position;
                            characterTransform.position = new Vector3(characterTransform.position.x, characterTransform.position.y, positionMemory.z);


                            if (useGeneralPlatformerVelocityRule /*&& oneWayPlatform == null*/)
                            {
                                Vector2 normal = (colInfo.position - contactPoint).normalized;
                                float angle = Vector2.Angle(Vector2.up, normal);

                                float dot = Vector2.Dot(velocity.normalized, normal);


                                if (angle > 180f - maxCeilingAngle)
                                {
                                    velocity.y = 0;
                                }
                                else if (angle < maxGroundAngle)
                                    velocity.y = (-slowFactor * dot) * velocity.y;
                                else
                                    velocity.x = (-slowFactor * dot) * velocity.x;
                            }


#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
                            if (debugguer)
                                contactDebugguer[cpt] = true;
#endif

                            contact = true;
                        }
                    }
                }
            }
#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
            if (debugguer)
                cpt++;
#endif

            //Recursively do again the HandleCollision function to resolve this again.
            if (currentDepth < pushBackResolution && contact)
            {
                HandleCollision3D(currentDepth + 1, pushBackResolution);
            }
        }


        #region ProbeGround2D
        /// <summary>
        /// Scan the surface below us for ground. Follow up the initial scan with subsequent scans
        /// designed to test what kind of surface we are standing above and handle different edge cases
        /// </summary>
        /// <param name="origin">Center of the sphere for the initial SphereCast</param>
        /// <param name="iter">Debug tool to print out which ProbeGround iteration is being run (3 are run each frame for the controller)</param>
        public void ProbeGround2D()
        {
            ResetGrounds();

            ColliderInfo feetCollider = characterControllerColliders[circleConfigurationIndex].colliders[feetIndex];

            Vector2 up = feetCollider.up;
            Vector2 down = -up;

            Vector2 startingPoint = (Vector2)feetCollider.position + (up * toleranceConst);

            // Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
            float smallerRadius = feetCollider.radius - (toleranceConst * toleranceConst);

            RaycastHit2D hit;

            if (hit = Physics2D.CircleCast(startingPoint, smallerRadius, down, Mathf.Infinity, walkableLayer))
            {
                if (hit.collider.isTrigger || ignoredColliders2D.Contains(hit.collider))
                    return;

                OneWayPlatform2D oneWayPlatform = hit.collider.gameObject.GetComponent<OneWayPlatform2D>();
                if ((oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity)) || ignoredColliders2D.Contains(hit.collider))
                {
                    return;
                }

                CollisionIgnorance collisionIngorance = hit.collider.gameObject.GetComponent<CollisionIgnorance>();
                if (collisionIngorance != null && collisionIngorance.Ignore(this.gameObject, feetCollider.name))
                    return;

                // By reducing the initial SphereCast's radius by Tolerance, our casted sphere no longer fits with
                // our controller's shape. Reconstruct the sphere cast with the proper radius
                SimulateCircleCast(hit.normal, out hit);

                if (!hit)
                    return;

                if (hit.collider.isTrigger || ignoredColliders2D.Contains(hit.collider))
                    return;

                oneWayPlatform = hit.collider.gameObject.GetComponent<OneWayPlatform2D>();
                if ((oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity)) || ignoredColliders2D.Contains(hit.collider))
                {
                    return;
                }

                collisionIngorance = hit.collider.gameObject.GetComponent<CollisionIgnorance>();
                if (collisionIngorance != null && collisionIngorance.Ignore(this.gameObject, feetCollider.name))
                    return;


                primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);


                // If we are standing on a perfectly flat surface, we cannot be either on an edge,
                // On a slope or stepping off a ledge
                if (Vector2.Distance(ExtendedMath.ProjectPointOnLine(feetCollider.right, feetCollider.position, hit.point), feetCollider.position) < toleranceConst)
                {
                    return;
                }


                // As we are standing on an edge, we need to retrieve the normals of the two
                // faces on either side of the edge and store them in nearHit and farHit


                Vector2 toCenter = ExtendedMath.ProjectVectorOnLine(up, ((Vector2)feetCollider.position - hit.point).normalized * toleranceConst);
                Vector2 awayFromCenter = Quaternion.AngleAxis(-80.0f, Vector3.Cross(toCenter, up)) * -toCenter;

                Vector2 nearPoint = hit.point + toCenter + (up * toleranceConst);
                Vector2 farPoint = hit.point + (awayFromCenter * 3);



                RaycastHit2D nearHit;
                RaycastHit2D farHit;

                nearHit = Physics2D.Raycast(nearPoint, down, Mathf.Infinity, walkableLayer);
                farHit = Physics2D.Raycast(farPoint, down, Mathf.Infinity, walkableLayer);


                nearGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                farGround = new GroundHit(farHit.point, farHit.normal, farHit.distance);

                //ANGLE BEHAVIOUR NOT IMPLEMENTED

                /*
                                // If we are currently standing on ground that should be counted as a wall,
                                // we are likely flush against it on the ground. Retrieve what we are standing on
                                if (Vector2.Angle(hit.normal, up) > maxAngle)
                                {
                                    // Retrieve a vector pointing down the slope
                                    Vector3 r = Vector3.Cross(hit.normal, down);
                                    Vector3 v = Vector3.Cross(r, hit.normal);

                                    Vector3 flushOrigin = hit.point + hit.normal * toleranceConst;

                                    RaycastHit flushHit;

                                    if (flushHit = Physics2D.Raycast(flushOrigin, v, Mathf.Infinity, walkableLayer))
                                    {
                                        RaycastHit sphereCastHit;

                                        if (SimulateSphereCast(flushHit.normal, out sphereCastHit))
                                        {
                                            flushGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                                        }
                                        else
                                        {
                                            // Uh oh
                                        }
                                    }



                                } * */


                /*     // If we are currently standing on a ledge then the face nearest the center of the
                     // controller should be steep enough to be counted as a wall. Retrieve the ground
                     // it is connected to at it's base, if there exists any
                     if (Vector2.Angle(nearHit.normal, up) > maxAngle || nearHit.distance > toleranceConst)
                     {
                         // We contacted the wall of the ledge, rather than the landing. Raycast down
                         // the wall to retrieve the proper landing
                         if (Vector2.Angle(nearHit.normal, up) > maxAngle)
                         {
                             // Retrieve a vector pointing down the slope
                             Vector3 r = Vector3.Cross(nearHit.normal, down);
                             Vector3 v = Vector3.Cross(r, nearHit.normal);

                             RaycastHit2D stepHit;

                             if (stepHit = Physics2D.Raycast(nearPoint, v, Mathf.Infinity, walkableLayer))
                             {
                                 stepGround = new GroundHit(stepHit.point, stepHit.normal, stepHit.distance);
                             }
                         }
                         else
                         {
                             stepGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                         }
                     }
                 }*/

            }
            // If the initial SphereCast fails, likely due to the controller clipping a wall,
            // fallback to a raycast simulated to SphereCast data
            else if (hit = Physics2D.Raycast(startingPoint, down, Mathf.Infinity, walkableLayer))
            {
                if (hit.collider.isTrigger || ignoredColliders2D.Contains(hit.collider))
                    return;

                OneWayPlatform2D oneWayPlatform = hit.collider.gameObject.GetComponent<OneWayPlatform2D>();
                if ((oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity)) || ignoredColliders2D.Contains(hit.collider))
                {
                    return;
                }

                CollisionIgnorance ignoreIndex = hit.collider.gameObject.GetComponent<CollisionIgnorance>();
                if (ignoreIndex != null && ignoreIndex.Ignore(this.gameObject, feetCollider.name))
                    return;


                RaycastHit2D sphereCastHit;

                if (SimulateCircleCast(hit.normal, out sphereCastHit))
                {
                    primaryGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                }
                else
                {
                    primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);
                }
            }
        }
        #endregion

        #region ProbeGround3D

        /// <summary>
        /// Scan the surface below us for ground. Follow up the initial scan with subsequent scans
        /// designed to test what kind of surface we are standing above and handle different edge cases
        /// </summary>
        /// <param name="origin">Center of the sphere for the initial SphereCast</param>
        /// <param name="iter">Debug tool to print out which ProbeGround iteration is being run (3 are run each frame for the controller)</param>
        public void ProbeGround3D()
        {
            ResetGrounds();

            ColliderInfo feetCollider = characterControllerColliders[circleConfigurationIndex].colliders[feetIndex];

            Vector3 up = feetCollider.up;
            Vector3 down = -up;

            Vector3 startingPoint = feetCollider.position + (up * toleranceConst);

            // Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
            float smallerRadius = feetCollider.radius - (toleranceConst * toleranceConst);

            RaycastHit hit;

            if (Physics.SphereCast(startingPoint, smallerRadius, down, out hit, Mathf.Infinity, walkableLayer))
            {
                if (hit.collider.isTrigger || ignoredColliders3D.Contains(hit.collider))
                    return;

               /* OneWayPlatform2D oneWayPlatform = hit.collider.gameObject.GetComponent<OneWayPlatform2D>();
                if ((oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity)) || ignoredColliders2D.Contains(hit.collider))
                {
                    return;
                }*/

                CollisionIgnorance collisionIgnorance = hit.collider.gameObject.GetComponent<CollisionIgnorance>();
                if (collisionIgnorance != null && collisionIgnorance.Ignore(this.gameObject, feetCollider.name))
                    return;

                // By reducing the initial SphereCast's radius by Tolerance, our casted sphere no longer fits with
                // our controller's shape. Reconstruct the sphere cast with the proper radius
                SimulateSphereCast(hit.normal, out hit);

                if (hit.collider.isTrigger || ignoredColliders3D.Contains(hit.collider))
                    return;

             /*   oneWayPlatform = hit.collider.gameObject.GetComponent<OneWayPlatform2D>();
                if ((oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity)) || ignoredColliders2D.Contains(hit.collider))
                {
                    return;
                }*/

                collisionIgnorance = hit.collider.gameObject.GetComponent<CollisionIgnorance>();
                if (collisionIgnorance != null && collisionIgnorance.Ignore(this.gameObject, feetCollider.name))
                    return;


                primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);


                // If we are standing on a perfectly flat surface, we cannot be either on an edge,
                // On a slope or stepping off a ledge
                if (Vector3.Distance(ExtendedMath.ProjectPointOnPlane(feetCollider.up, feetCollider.position, hit.point), feetCollider.position) < toleranceConst)
                {
                    return;
                }

                // As we are standing on an edge, we need to retrieve the normals of the two
                // faces on either side of the edge and store them in nearHit and farHit

                Vector3 toCenter = ExtendedMath.ProjectVectorOnPlane(up, (feetCollider.position - hit.point).normalized * toleranceConst);
                Vector3 awayFromCenter = Quaternion.AngleAxis(-80.0f, Vector3.Cross(toCenter, up)) * -toCenter;

                Vector3 nearPoint = hit.point + toCenter + (up * toleranceConst);
                Vector3 farPoint = hit.point + (awayFromCenter * 3);

                RaycastHit nearHit;
                RaycastHit farHit;

                Physics.Raycast(nearPoint, down, out nearHit, Mathf.Infinity, walkableLayer);
                Physics.Raycast(farPoint, down, out farHit, Mathf.Infinity, walkableLayer);


                nearGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                farGround = new GroundHit(farHit.point, farHit.normal, farHit.distance);


                // If we are currently standing on ground that should be counted as a wall,
                // we are likely flush against it on the ground. Retrieve what we are standing on
                if (Vector3.Angle(hit.normal, up) > maxGroundAngle)
                {
                    // Retrieve a vector pointing down the slope
                    Vector3 r = Vector3.Cross(hit.normal, down);
                    Vector3 v = Vector3.Cross(r, hit.normal);

                    Vector3 flushOrigin = hit.point + hit.normal * toleranceConst;

                    RaycastHit flushHit;

                    if (Physics.Raycast(flushOrigin, v, out flushHit, Mathf.Infinity, walkableLayer))
                    {
                        RaycastHit sphereCastHit;

                        if (SimulateSphereCast(flushHit.normal, out sphereCastHit))
                        {
                            flushGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                        }
                        else
                        {
                            // Uh oh
                        }
                    }
                }

                // If we are currently standing on a ledge then the face nearest the center of the
                // controller should be steep enough to be counted as a wall. Retrieve the ground
                // it is connected to at it's base, if there exists any
                if (Vector3.Angle(nearHit.normal, up) > maxGroundAngle || nearHit.distance > toleranceConst)
                {
                    // We contacted the wall of the ledge, rather than the landing. Raycast down
                    // the wall to retrieve the proper landing
                    if (Vector3.Angle(nearHit.normal, up) > maxGroundAngle)
                    {
                        // Retrieve a vector pointing down the slope
                        Vector3 r = Vector3.Cross(nearHit.normal, down);
                        Vector3 v = Vector3.Cross(r, nearHit.normal);

                        RaycastHit2D stepHit;

                        if (stepHit = Physics2D.Raycast(nearPoint, v, Mathf.Infinity, walkableLayer))
                        {
                            stepGround = new GroundHit(stepHit.point, stepHit.normal, stepHit.distance);
                        }
                    }
                    else
                    {
                        stepGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                    }
                }
            }
            // If the initial SphereCast fails, likely due to the controller clipping a wall,
            // fallback to a raycast simulated to SphereCast data
            else if (Physics.Raycast(startingPoint, down, Mathf.Infinity, walkableLayer))
            {
                if (hit.collider.isTrigger || ignoredColliders3D.Contains(hit.collider))
                    return;

              /*  OneWayPlatform2D oneWayPlatform = hit.collider.gameObject.GetComponent<OneWayPlatform2D>();
                if ((oneWayPlatform != null && oneWayPlatform.CanPassThrought(velocity)) || ignoredColliders2D.Contains(hit.collider))
                {
                    return;
                }*/

                CollisionIgnorance ignoreIndex = hit.collider.gameObject.GetComponent<CollisionIgnorance>();
                if (ignoreIndex != null && ignoreIndex.Ignore(this.gameObject, feetCollider.name))
                    return;

                RaycastHit sphereCastHit;

                if (SimulateSphereCast(hit.normal, out sphereCastHit))
                {
                    primaryGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                }
                else
                {
                    primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);
                }
            }
        }
        #endregion

        #region SimulateCircleCast
        private bool SimulateCircleCast(Vector2 groundNormal, out RaycastHit2D hit)
        {
            ColliderInfo feetCollider = characterControllerColliders[circleConfigurationIndex].colliders[feetIndex];

            float groundAngle = Vector2.Angle(groundNormal, feetCollider.up) * Mathf.Deg2Rad;

            Vector2 secondaryOrigin = (Vector2)(feetCollider.position) + (Vector2)feetCollider.up * toleranceConst;

            if (!Mathf.Approximately(groundAngle, 0))
            {

                //Pythagore 
                //Look what is done with a circle which touch a line
                //There are 2 rectangulares triangles
                //We want the opposite distance of one and the hypothenuse of the other to do our final raycast
                float horizontal = Mathf.Sin(groundAngle) * feetCollider.radius;
                float vertical = (1.0f - Mathf.Cos(groundAngle)) * feetCollider.radius;

                //2D SIMPLIFICATION//
                float sign = Mathf.Sign(groundNormal.x * feetCollider.up.y - groundNormal.y * feetCollider.up.x);
                Vector2 projection = sign * (new Vector2(-feetCollider.up.y, feetCollider.up.x).normalized) * horizontal + (Vector2)feetCollider.up * vertical;

                secondaryOrigin += projection;
            }

            if (hit = Physics2D.Raycast(secondaryOrigin, -feetCollider.up, Mathf.Infinity, walkableLayer))
            {
                // Remove the tolerance from the distance travelled
                hit.distance -= toleranceConst;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region SimulateSphereCast

        /// <summary>
        /// Provides raycast data based on where a SphereCast would contact the specified normal
        /// Raycasting downwards from a point along the controller's bottom sphere, based on the provided
        /// normal
        /// </summary>
        /// <param name="groundNormal">Normal of a triangle assumed to be directly below the controller</param>
        /// <param name="hit">Simulated SphereCast data</param>
        /// <returns>True if the raycast is successful</returns>
        private bool SimulateSphereCast(Vector3 groundNormal, out RaycastHit hit)
        {

            ColliderInfo feetCollider = characterControllerColliders[circleConfigurationIndex].colliders[feetIndex];
            float groundAngle = Vector3.Angle(groundNormal, feetCollider.up) * Mathf.Deg2Rad;

            Vector3 secondaryOrigin = feetCollider.position + feetCollider.up * toleranceConst;

            if (!Mathf.Approximately(groundAngle, 0))
            {
                float horizontal = Mathf.Sin(groundAngle) * feetCollider.radius;
                float vertical = (1.0f - Mathf.Cos(groundAngle)) * feetCollider.radius;

                // Retrieve a vector pointing up the slope
                Vector3 r2 = Vector3.Cross(groundNormal, -feetCollider.up);
                Vector3 v2 = -Vector3.Cross(r2, groundNormal);

                secondaryOrigin += ExtendedMath.ProjectVectorOnPlane(feetCollider.up, v2).normalized * horizontal + feetCollider.up * vertical;
            }

            if (Physics.Raycast(secondaryOrigin, -feetCollider.up, out hit, Mathf.Infinity, walkableLayer))
            {
                // Remove the tolerance from the distance travelled
                hit.distance -= toleranceConst;

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void ResetGrounds()
        {
            primaryGround = null;
            nearGround = null;
            farGround = null;
            flushGround = null;
            stepGround = null;
        }

        #region CheckGround2D
        /// <summary>
        /// Check if the character is standing on the ground
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="groundNormal"></param>
        /// <returns></returns>
        public bool CheckGround2D(float distance, out Vector3 groundNormal)
        {
            groundNormal = Vector2.zero;

            if (primaryGround == null || primaryGround.distance > distance)
            {
                return false;
            }

            Transform characterTransform = this.characterTransform;

            // Check if we are flush against a wall
            if (farGround != null && Vector2.Angle(farGround.normal, characterTransform.up) > maxGroundAngle)
            {
                /* if (flushGround != null && Vector3.Angle(flushGround.normal, controller.up) < superCollisionType.StandAngle && flushGround.distance < distance)
                 {
                     groundNormal = flushGround.normal;
                     return true;
                 }*/
                return false;
            }

            // Check if we are at the edge of a ledge, or on a high angle slope
            if (farGround != null && !OnSteadyGround2D(farGround.normal, primaryGround.point))
            {
                // Check if we are walking onto steadier ground
                if (nearGround != null && nearGround.distance <= distance && Vector3.Angle(nearGround.normal, characterTransform.up) < maxGroundAngle && !OnSteadyGround2D(nearGround.normal, nearGround.point))
                {

                    //Debug.Log("nearDistance vs distance " + (nearGround.distance < distance));
                    //Debug.Log("nearDistance vs angle " + (Vector3.Angle(nearGround.normal, controller.transform.up) < maxAngle));
                    //Debug.Log("steadyGround : " + (!OnSteadyGround(nearGround.normal, nearGround.point)));

                    groundNormal = nearGround.normal;
                    return true;
                }
                // Check if we are on a step or stair
                /* if (stepGround != null && stepGround.distance < distance && Vector3.Angle(stepGround.normal, controller.up) < superCollisionType.StandAngle)
                 {
                     groundNormal = stepGround.normal;
                     return true;
                 }*/

                return false;
            }


            if (farGround != null)
            {
                groundNormal = farGround.normal;
            }
            else
            {
                groundNormal = primaryGround.normal;
            }

            return true;
        }
        #endregion


        public bool CheckGround3D(float distance, out Vector3 groundNormal)
        {
            groundNormal = Vector3.zero;

            if (primaryGround == null || primaryGround.distance > distance)
            {
                return false;
            }

            // Check if we are flush against a wall
            if (farGround != null && Vector3.Angle(farGround.normal, characterTransform.up) > maxGroundAngle)
            {
                if (flushGround != null && Vector3.Angle(flushGround.normal, characterTransform.up) < maxGroundAngle && flushGround.distance < distance)
                {
                    groundNormal = flushGround.normal;
                    return true;
                }

                return false;
            }

            // Check if we are at the edge of a ledge, or on a high angle slope
            if (farGround != null && !OnSteadyGround3D(farGround.normal, primaryGround.point))
            {
                // Check if we are walking onto steadier ground
                if (nearGround != null && nearGround.distance < distance && Vector3.Angle(nearGround.normal, characterTransform.up) < maxGroundAngle && !OnSteadyGround3D(nearGround.normal, nearGround.point))
                {
                    groundNormal = nearGround.normal;
                    return true;
                }

                // Check if we are on a step or stair
                if (stepGround != null && stepGround.distance < distance && Vector3.Angle(stepGround.normal, characterTransform.up) < maxGroundAngle)
                {
                    groundNormal = stepGround.normal;
                    return true;
                }

                return false;
            }


            if (farGround != null)
            {
                groundNormal = farGround.normal;
            }
            else
            {
                groundNormal = primaryGround.normal;
            }

            return true;
        }


        /// <summary>
        /// To help the controller smoothly "fall" off surfaces and not hang on the edge of ledges,
        /// check that the ground below us is "steady", or that the controller is not standing
        /// on too extreme of a ledge
        /// </summary>
        /// <param name="normal">Normal of the surface to test against</param>
        /// <param name="point">Point of contact with the surface</param>
        /// <returns>True if the ground is steady</returns>
        private bool OnSteadyGround2D(Vector2 normal, Vector2 point)
        {
            ColliderInfo feetCollider = characterControllerColliders[circleConfigurationIndex].colliders[feetIndex];

            float angle = Vector2.Angle(normal, feetCollider.up);

            float angleRatio = angle / groundingUpperBoundAngle;

            float distanceRatio = Mathf.Lerp(groundingMinPercentFromcenter, groundingMaxPercentFromCenter, angleRatio);

            Vector2 p = ExtendedMath.ProjectPointOnLine(feetCollider.up, feetCollider.position, point);

            float distanceFromCenter = Vector2.Distance(p, feetCollider.position);

            return distanceFromCenter <= distanceRatio * feetCollider.radius;
        }

        /// <summary>
        /// To help the controller smoothly "fall" off surfaces and not hang on the edge of ledges,
        /// check that the ground below us is "steady", or that the controller is not standing
        /// on too extreme of a ledge
        /// </summary>
        /// <param name="normal">Normal of the surface to test against</param>
        /// <param name="point">Point of contact with the surface</param>
        /// <returns>True if the ground is steady</returns>
        private bool OnSteadyGround3D(Vector3 normal, Vector3 point)
        {
            ColliderInfo feetCollider = characterControllerColliders[circleConfigurationIndex].colliders[feetIndex];

            float angle = Vector3.Angle(normal, feetCollider.up);

            float angleRatio = angle / groundingUpperBoundAngle;

            float distanceRatio = Mathf.Lerp(groundingMinPercentFromcenter, groundingMaxPercentFromCenter, angleRatio);

            Vector3 p = ExtendedMath.ProjectPointOnPlane(feetCollider.up, feetCollider.position, point);

            float distanceFromCenter = Vector3.Distance(p, feetCollider.position);

            return distanceFromCenter <= distanceRatio * feetCollider.radius;
        }

#if UNITY_EDITOR || EQUILIBRE_GAMES_DEBUG
        /// <summary>
        /// Draw the debug function of Your circle collider character controller
        /// </summary>
        void OnDrawGizmos()
        {
            if (!enabled || characterControllerColliders == null || characterControllerColliders.Length <= circleConfigurationIndex || ! debugguer)
                return;

            if (contactDebugguer == null && characterControllerColliders[circleConfigurationIndex].colliders != null)
            {
                int max = 0;

                foreach (CharacterController2DColliders i in characterControllerColliders)
                {
                    if (i.colliders.Length > max)
                        max = i.colliders.Length;
                }
                contactDebugguer = new bool[max];
            }

            if (contactDebugguer != null)
            {
                foreach (bool b in contactDebugguer)
                {
                    Gizmos.color = b ? Color.cyan : Color.yellow;
                }
            }
            foreach (ColliderInfo colInfo in characterControllerColliders[circleConfigurationIndex].colliders)
            {
                Gizmos.DrawWireSphere((Vector2)colInfo.position, colInfo.radius);
            }


            if (primaryGround != null)
            {
                DebugDraw.DrawVector(primaryGround.point, primaryGround.normal, 2.0f, 1.0f, Color.yellow, 0, false);
            }

            if (nearGround != null)
            {
                DebugDraw.DrawVector(nearGround.point, nearGround.normal, 2.0f, 1.0f, Color.blue, 0, false);
            }

            if (farGround != null)
            {
                DebugDraw.DrawVector(farGround.point, farGround.normal, 2.0f, 1.0f, Color.red, 0, false);
            }

            /*
            if (flush && flushGround != null)
            {
                DebugDraw.DrawVector(flushGround.point, flushGround.normal, 2.0f, 1.0f, Color.cyan, 0, false);
            }

            if (step && stepGround != null)
            {
                DebugDraw.DrawVector(stepGround.point, stepGround.normal, 2.0f, 1.0f, Color.green, 0, false);
            }*/
        }

#endif
    }
}