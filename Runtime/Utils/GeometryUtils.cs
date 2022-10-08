using UnityEngine;

namespace PolyternityStuff.Utils
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Checks if a point is inside a polygon defined by corners.
        /// </summary>
        public static bool IsPointInsidePolygon(Vector3 point, Vector3[] corners)
        {
            var polygonLength = corners.Length;
            var i = 0;
            var isInside = false;
            
            var pointX = point.x;
            var pointY = point.z;

            var endPoint = corners[polygonLength - 1];
            var endX = endPoint.x;
            var endY = endPoint.z;
            while (i < polygonLength)
            {
                var startX = endX;
                var startY = endY;
                endPoint = corners[i++];
                endX = endPoint.x;
                endY = endPoint.z;
                isInside ^= endY > pointY ^ startY > pointY /* pointY inside [startY;endY] segment ? */
                            && /* if so, test if it is under the segment */
                            (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY);
            }
                
            return isInside;
        }
    }
}