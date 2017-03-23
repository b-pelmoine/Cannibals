using UnityEngine;
using System.Collections;


namespace EquilibreGames
{
    public class ExtendedMath : MonoBehaviour
    {

        /// <summary>
        /// Find the closest point on the surface of a circle defined by the circle collider 2D
        /// </summary>
        /// <param name="collider">The collider that you analyse</param>
        /// <param name="toPosition">The point That you analyse</param>
        /// <returns>The closesPoint for a circleCollider2D Shape</returns>
        public static Vector2 ClosestPointOnSurface(CircleCollider2D collider, Vector2 toPosition)
        {
            Vector2 positionReturned;

            //Calcul the normal vector
            positionReturned = toPosition - (Vector2)collider.bounds.center;
            positionReturned.Normalize();

            //Multiply the normal by the global radius of the collider (extents.x or extents.y are equal for cercle)
            positionReturned *= collider.bounds.extents.x;


            //Translate the position with the collider position.
            positionReturned += (Vector2)collider.bounds.center;

            return positionReturned;
        }


        /// <summary>
        /// Find the closest point on the surface of a Box2D defined by the box collider 2D. Can be oriented.
        /// </summary>
        /// <param name="collider">The collider that you analyse</param>
        /// <param name="toPosition">The point That you analyse</param>
        /// <returns>The closesPoint for a BoxCollider2D Shape</returns>
        public static Vector2 ClosestPointOnSurface(BoxCollider2D collider, Vector2 toPosition)
        {
            // Cache the collider transform
            var ct = collider.transform;

            // Firstly, transform the point into the space of the collider
            Vector2 local = ct.InverseTransformPoint(toPosition);

            // Now, shift it to be in the center of the box
            local = local - collider.offset;

            //Pre multiply to save operations.
            Vector2 halfSize = collider.size * 0.5f;
            Vector2 worldhalfSize;

            worldhalfSize.x = halfSize.x * ct.lossyScale.x;
            worldhalfSize.y = halfSize.y * ct.lossyScale.y;


            // Clamp the points to the collider's extents
            Vector2 localNorm = new Vector3(
                    Mathf.Clamp(local.x, -halfSize.x, halfSize.x),
                    Mathf.Clamp(local.y, -halfSize.y, halfSize.y)
                );


            //Calculate distances from each edge
            float dx = Mathf.Abs(Mathf.Min(worldhalfSize.x - localNorm.x * ct.lossyScale.x, worldhalfSize.x + localNorm.x * ct.lossyScale.x));
            float dy = Mathf.Abs(Mathf.Min(worldhalfSize.y - localNorm.y * ct.lossyScale.y, worldhalfSize.y + localNorm.y * ct.lossyScale.y));



            // Select a face to project on
            if (dx < dy)
            {
                localNorm.x = Mathf.Sign(localNorm.x) * halfSize.x;
            }
            else
            {
                localNorm.y = Mathf.Sign(localNorm.y) * halfSize.y;
            }

            // Now we undo our transformations
            localNorm = localNorm + collider.offset;

            // Return resulting point
            return ct.TransformPoint(localNorm);
        }

        /// <summary>
        /// Find the closest point on the surface of a Polygon2D defined by the polygon collider 2D. Can be oriented.
        /// IS NOT OPTIMISED BY PARTITION
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="toPosition"></param>
        /// <returns></returns>
        public static Vector2 ClosestPointOnSurface(PolygonCollider2D collider, Vector2 toPosition)
        {
            Vector2 positionReturned;
            float minDistance = Mathf.Infinity;

            // Cache the collider transform
            var ct = collider.transform;

            // Firstly, transform the point into the space of the collider
            Vector2 local = ct.InverseTransformPoint(toPosition);

            // Now, shift it to be in the center of the box
            local = local - collider.offset;

            int length = collider.points.Length;


            Vector2 firstPoint = collider.points[length - 1];
            Vector2 secondPoint = collider.points[0];

            Vector2 projectedPointOnSegment = ProjectPointOnLineSegment(firstPoint, secondPoint, local);
            minDistance = Vector2.SqrMagnitude(local - projectedPointOnSegment);
            positionReturned = projectedPointOnSegment;

            for (int i = 0; i < length-1; i++)
            {
                firstPoint = collider.points[i];
                secondPoint = collider.points[i+1];

                projectedPointOnSegment = ProjectPointOnLineSegment(firstPoint, secondPoint, local);

                float calc = Vector2.SqrMagnitude(local - projectedPointOnSegment);

                if (calc < minDistance)
                {
                    minDistance = calc;
                    positionReturned = projectedPointOnSegment;
                }
            }


            return (Vector2)ct.TransformPoint(positionReturned) + collider.offset;
        }



        public static Vector3 ClosestPointOnSurface(SphereCollider collider, Vector3 to)
        {
            Vector3 p;

            p = to - (collider.transform.position + collider.center);
            p.Normalize();

            p *= collider.radius * collider.transform.localScale.x;
            p += collider.transform.position + collider.center;

            return p;
        }

        public static Vector3 ClosestPointOnSurface(BoxCollider collider, Vector3 to)
        {
            // Cache the collider transform
            var ct = collider.transform;

            // Firstly, transform the point into the space of the collider
            var local = ct.InverseTransformPoint(to);

            // Now, shift it to be in the center of the box
            local -= collider.center;

            //Pre multiply to save operations.
            var halfSize = collider.size * 0.5f;

            // Clamp the points to the collider's extents
            var localNorm = new Vector3(
                    Mathf.Clamp(local.x, -halfSize.x, halfSize.x),
                    Mathf.Clamp(local.y, -halfSize.y, halfSize.y),
                    Mathf.Clamp(local.z, -halfSize.z, halfSize.z)
                );

            //Calculate distances from each edge
            var dx = Mathf.Min(Mathf.Abs(halfSize.x - localNorm.x), Mathf.Abs(-halfSize.x - localNorm.x));
            var dy = Mathf.Min(Mathf.Abs(halfSize.y - localNorm.y), Mathf.Abs(-halfSize.y - localNorm.y));
            var dz = Mathf.Min(Mathf.Abs(halfSize.z - localNorm.z), Mathf.Abs(-halfSize.z - localNorm.z));

            // Select a face to project on
            if (dx < dy && dx < dz)
            {
                localNorm.x = Mathf.Sign(localNorm.x) * halfSize.x;
            }
            else if (dy < dx && dy < dz)
            {
                localNorm.y = Mathf.Sign(localNorm.y) * halfSize.y;
            }
            else if (dz < dx && dz < dy)
            {
                localNorm.z = Mathf.Sign(localNorm.z) * halfSize.z;
            }

            // Now we undo our transformations
            localNorm += collider.center;

            // Return resulting point
            return ct.TransformPoint(localNorm);
        }

        // Courtesy of Moodie
        public static Vector3 ClosestPointOnSurface(CapsuleCollider collider, Vector3 to)
        {
            Transform ct = collider.transform; // Transform of the collider

            float lineLength = collider.height - collider.radius * 2; // The length of the line connecting the center of both sphere
            Vector3 dir = Vector3.up;

            Vector3 upperSphere = dir * lineLength * 0.5f + collider.center; // The position of the radius of the upper sphere in local coordinates
            Vector3 lowerSphere = -dir * lineLength * 0.5f + collider.center; // The position of the radius of the lower sphere in local coordinates

            Vector3 local = ct.InverseTransformPoint(to); // The position of the controller in local coordinates

            Vector3 p = Vector3.zero; // Contact point
            Vector3 pt = Vector3.zero; // The point we need to use to get a direction vector with the controller to calculate contact point

            if (local.y < lineLength * 0.5f && local.y > -lineLength * 0.5f) // Controller is contacting with cylinder, not spheres
                pt = dir * local.y + collider.center;
            else if (local.y > lineLength * 0.5f) // Controller is contacting with the upper sphere 
                pt = upperSphere;
            else if (local.y < -lineLength * 0.5f) // Controller is contacting with lower sphere
                pt = lowerSphere;

            //Calculate contact point in local coordinates and return it in world coordinates
            p = local - pt;
            p.Normalize();
            p = p * collider.radius + pt;
            return ct.TransformPoint(p);

        }

        public static Vector3 ClosestPointOnSurface(TerrainCollider collider, Vector3 to, float radius)
        {
            var terrainData = collider.terrainData;

            var local = collider.transform.InverseTransformPoint(to);

            // Calculate the size of each tile on the terrain horizontally and vertically
            float pixelSizeX = terrainData.size.x / (terrainData.heightmapResolution - 1);
            float pixelSizeZ = terrainData.size.z / (terrainData.heightmapResolution - 1);

            var percentZ = Mathf.Clamp01(local.z / terrainData.size.z);
            var percentX = Mathf.Clamp01(local.x / terrainData.size.x);

            float positionX = percentX * (terrainData.heightmapResolution - 1);
            float positionZ = percentZ * (terrainData.heightmapResolution - 1);

            // Calculate our position, in tiles, on the terrain
            int pixelX = Mathf.FloorToInt(positionX);
            int pixelZ = Mathf.FloorToInt(positionZ);

            // Calculate the distance from our point to the edge of the tile we are in
            float distanceX = (positionX - pixelX) * pixelSizeX;
            float distanceZ = (positionZ - pixelZ) * pixelSizeZ;

            // Find out how many tiles we are overlapping on the X plane
            float radiusExtentsLeftX = radius - distanceX;
            float radiusExtentsRightX = radius - (pixelSizeX - distanceX);

            int overlappedTilesXLeft = radiusExtentsLeftX > 0 ? Mathf.FloorToInt(radiusExtentsLeftX / pixelSizeX) + 1 : 0;
            int overlappedTilesXRight = radiusExtentsRightX > 0 ? Mathf.FloorToInt(radiusExtentsRightX / pixelSizeX) + 1 : 0;

            // Find out how many tiles we are overlapping on the Z plane
            float radiusExtentsLeftZ = radius - distanceZ;
            float radiusExtentsRightZ = radius - (pixelSizeZ - distanceZ);

            int overlappedTilesZLeft = radiusExtentsLeftZ > 0 ? Mathf.FloorToInt(radiusExtentsLeftZ / pixelSizeZ) + 1 : 0;
            int overlappedTilesZRight = radiusExtentsRightZ > 0 ? Mathf.FloorToInt(radiusExtentsRightZ / pixelSizeZ) + 1 : 0;

            // Retrieve the heights of the pixels we are testing against
            int startPositionX = pixelX - overlappedTilesXLeft;
            int startPositionZ = pixelZ - overlappedTilesZLeft;

            int numberOfXPixels = overlappedTilesXRight + overlappedTilesXLeft + 1;
            int numberOfZPixels = overlappedTilesZRight + overlappedTilesZLeft + 1;

            // Account for if we are off the terrain
            if (startPositionX < 0)
            {
                numberOfXPixels -= Mathf.Abs(startPositionX);
                startPositionX = 0;
            }

            if (startPositionZ < 0)
            {
                numberOfZPixels -= Mathf.Abs(startPositionZ);
                startPositionZ = 0;
            }

            if (startPositionX + numberOfXPixels + 1 > terrainData.heightmapResolution)
            {
                numberOfXPixels = terrainData.heightmapResolution - startPositionX - 1;
            }

            if (startPositionZ + numberOfZPixels + 1 > terrainData.heightmapResolution)
            {
                numberOfZPixels = terrainData.heightmapResolution - startPositionZ - 1;
            }

            // Retrieve the heights of the tile we are in and all overlapped tiles
            var heights = terrainData.GetHeights(startPositionX, startPositionZ, numberOfXPixels + 1, numberOfZPixels + 1);

            // Pre-scale the heights data to be world-scale instead of 0...1
            for (int i = 0; i < numberOfXPixels + 1; i++)
            {
                for (int j = 0; j < numberOfZPixels + 1; j++)
                {
                    heights[j, i] *= terrainData.size.y;
                }
            }

            // Find the shortest distance to any triangle in the set gathered
            float shortestDistance = float.MaxValue;

            Vector3 shortestPoint = Vector3.zero;

            for (int x = 0; x < numberOfXPixels; x++)
            {
                for (int z = 0; z < numberOfZPixels; z++)
                {
                    // Build the set of points that creates the two triangles that form this tile
                    Vector3 a = new Vector3((startPositionX + x) * pixelSizeX, heights[z, x], (startPositionZ + z) * pixelSizeZ);
                    Vector3 b = new Vector3((startPositionX + x + 1) * pixelSizeX, heights[z, x + 1], (startPositionZ + z) * pixelSizeZ);
                    Vector3 c = new Vector3((startPositionX + x) * pixelSizeX, heights[z + 1, x], (startPositionZ + z + 1) * pixelSizeZ);
                    Vector3 d = new Vector3((startPositionX + x + 1) * pixelSizeX, heights[z + 1, x + 1], (startPositionZ + z + 1) * pixelSizeZ);

                    Vector3 nearest;

                    BSPTree.ClosestPointOnTriangleToPoint(ref a, ref d, ref c, ref local, out nearest);

                    float distance = (local - nearest).sqrMagnitude;

                    if (distance <= shortestDistance)
                    {
                        shortestDistance = distance;
                        shortestPoint = nearest;
                    }

                    BSPTree.ClosestPointOnTriangleToPoint(ref a, ref b, ref d, ref local, out nearest);

                    distance = (local - nearest).sqrMagnitude;

                    if (distance <= shortestDistance)
                    {
                        shortestDistance = distance;
                        shortestPoint = nearest;
                    }
                }
            }

            return collider.transform.TransformPoint(shortestPoint);
        }

        public static Vector3 ClosestPointOnSurface(MeshCollider collider, Vector3 to, float radius)
        {
            BSPTree bsp = collider.GetComponent<BSPTree>();

            if (bsp != null)
            {
                return bsp.ClosestPointOn(to, radius);
            }

            return to;
        }


public static Vector2 ProjectVectorOnLine(Vector2 lineDir, Vector2 vector)
        {
            return -(vector - (Vector2.Dot(vector, lineDir) * lineDir));
        }


        /// <summary>
        /// Return the projected point on a line. The projected point is the point on line that minimise the distance between him and the point parameter
        /// </summary>
        /// <param name="lineDir"></param>
        /// <param name="linePoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 ProjectPointOnLine(Vector2 lineDir, Vector2 linePoint, Vector2 point)
        {
            float calc = Vector2.Dot(point - linePoint, lineDir); ;

            return new Vector2(linePoint.x + calc * lineDir.x, linePoint.y + calc * lineDir.y);
        }


        //Get the shortest distance between a point and a line. The output is signed so it holds information
        public static float DistancePointToLine(Vector2 lineDir, Vector2 linePoint, Vector2 point)
        {
            Vector2 vector = linePoint - point;

            return Mathf.Abs(vector.x*lineDir.y - vector.y*lineDir.x);
        }


        /// <summary>
        /// create a vector of direction "vector" with length "size"
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector2 SetVectorLength(Vector2 vector, float size)
        {
            //normalize the vector
            return vector.normalized * size;
        }

        public static bool PointIsOnSegment(Vector2 firstPoint,Vector2 secondPoint, Vector2 pointToTest)
        {
            Vector2 dir1 = pointToTest - firstPoint;
            Vector2 dir2 = pointToTest - secondPoint;


            //Cross product tell us if all point are on the same line
            if( (dir1.x*dir2.y - dir1.y*dir2.x) < 0.0000001f)
            {
                //Dot product tell us if vector are or not oriented on the same way (with the sign)
                return Vector2.Dot(dir1, dir2) < 0;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 ProjectPointOnLineSegment(Vector2 firstPoint, Vector2 secondPoint, Vector2 point)
        {
            Vector2 vector = secondPoint - firstPoint;

            Vector2 projectedPoint = ProjectPointOnLine(vector.normalized,firstPoint, point);

            Vector2 pointVec = point - firstPoint;

            float dot = Vector2.Dot(pointVec, vector);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0)
            {
                //point is on the line segment
                if (pointVec.magnitude <= vector.magnitude)
                {
                    return projectedPoint;
                }
                //point is not on the line segment and it is on the side of linePoint2
                else
                {
                    return secondPoint;
                }
            }
            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            else
            {
                return firstPoint;
            }
        }



        /// <summary>
        /// This function returns a point which is a projection from a point to a plane.
        /// </summary>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {

            float distance;
            Vector3 translationVector;

            //First calculate the distance from the point to the plane:
            distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

            //Reverse the sign of the distance
            distance *= -1;

            //Get a translation vector
            translationVector = SetVectorLength(planeNormal, distance);

            //Translate the point to form a projection
            return point + translationVector;
        }

        //Projects a vector onto a plane. The output is not normalized.
        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        {

            return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
        }

        /// <summary>
        ///Get the shortest distance between a point and a plane. The output is signed so it holds information
        ///as to which side of the plane normal the point is.
        /// </summary>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            return Vector3.Dot(planeNormal, (point - planePoint));
        }

        /// <summary>
        /// Calcul a CatmulRom spline interpolation with 4 given points.
        /// </summary>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="t"> interpolation parameter between point1 and point2</param>
        /// <returns>An interpolation point between point 2 and 3 </returns>
        public static Vector2 CatmullRomInterpolation(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3,float t0, float t1, float t2, float t3, float t)
        {
            Vector2 A1 = ((t1 - t)  * point0 + (t - t0) * point1) / (t1 - t0);
            Vector2 A2 = ((t2 - t) * point1 + (t - t1) * point2) / (t2 - t1);
            Vector2 A3 = ((t3 - t) * point2 + (t - t2) * point3) / (t3 - t2);

            Vector2 B1 = ((t2 - t) * A1 + (t - t0) * A2) / (t2 - t0);
            Vector2 B2 = ((t3 - t) * A2 + (t - t1) * A3)/ (t3 - t1);

            return ((t2 - t) * B1 + (t - t1) * B2) / (t2 - t1);
        }


        public static void DrawCatmullRomInterpolation(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3, float t0, float t1, float t2, float t3)
        {
            if (t2 != t1)
            {
                for (float t = t1; t <= t2; t += ((t2 - t1) / 50f))
                {
                    Vector2 A1 = ((t1 - t) * point0 + (t - t0) * point1) / (t1 - t0);
                    Vector2 A2 = ((t2 - t) * point1 + (t - t1) * point2) / (t2 - t1);
                    Vector2 A3 = ((t3 - t) * point2 + (t - t2) * point3) / (t3 - t2);

                    Vector2 B1 = ((t2 - t) * A1 + (t - t0) * A2) / (t2 - t0);
                    Vector2 B2 = ((t3 - t) * A2 + (t - t1) * A3) / (t3 - t1);

                    DebugDraw.DrawMarker(((t2 - t) * B1 + (t - t1) * B2) / (t2 - t1), 0.05f, Color.red, 2f);
                }
            }
        }

        public static void DrawLinearInterpolation(Vector3 point1, Vector2 point2)
        {
            for (float t = 0; t <= 1; t += 1f / 50f)
            {             
                DebugDraw.DrawMarker(Vector3.Lerp(point1,point2,t), 0.05f, Color.red, 2f);
            }
        }
    }


}
