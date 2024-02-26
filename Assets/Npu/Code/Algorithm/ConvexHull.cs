﻿using System.Collections.Generic;
using UnityEngine;

namespace Npu.Algorithm
{

    public static class ConvexHull
    {
        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        public static int Orientation(Vector3 p, Vector3 q, Vector3 r)
        {
            var val = (q.z - p.z) * (r.x - q.x) -
                      (q.x - p.x) * (r.z - q.z);

            if (val == 0) return 0; // collinear 
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // Prints convex hull of a set of n points. 
        public static List<Vector3> Compute(Vector3[] points)
        {
            // There must be at least 3 points 
            if (points.Length < 3) return null;

            // Initialize Result 
            var hull = new List<Vector3>();

            // Find the leftmost point 
            var n = points.Length;
            var l = 0;
            for (var i = 1; i < n; i++)
                if (points[i].x < points[l].x)
                    l = i;

            // Start from leftmost point, keep moving  
            // counterclockwise until reach the start point 
            // again. This loop runs O(h) times where h is 
            // number of points in result or output. 
            int p = l, q;
            do
            {
                // Add current point to result 
                hull.Add(points[p]);

                // Search for a point 'q' such that  
                // orientation(p, x, q) is counterclockwise  
                // for all points 'x'. The idea is to keep  
                // track of last visited most counterclock- 
                // wise point in q. If any point 'i' is more  
                // counterclock-wise than q, then update q. 
                q = (p + 1) % n;

                for (var i = 0; i < n; i++)
                {
                    // If i is more counterclockwise than  
                    // current q, then update q 
                    if (Orientation(points[p], points[i], points[q])
                        == 2)
                        q = i;
                }

                // Now q is the most counterclockwise with 
                // respect to p. Set p as q for next iteration,  
                // so that q is added to result 'hull' 
                p = q;

            } while (p != l); // While we don't come to first  
            // point 

            return hull;
        }
    }

}