using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BriefFiniteElementNet;
namespace FiniteElementMethod
{

    public class Triangulation2D
    {
        // From Wikipedia:
        // One way to triangulate a simple polygon is by using the assertion that any simple polygon
        // without holes has at least two so called 'ears'. An ear is a triangle with two sides on the edge
        // of the polygon and the other one completely inside it. The algorithm then consists of finding
        // such an ear, removing it from the polygon (which results in a new polygon that still meets
        // the conditions) and repeating until there is only one triangle left.

        // the algorithm here aims for simplicity over performance. there are other, more performant
        // algorithms that are much more complex.

        // convert a triangle to a list of triangles. each triangle is represented by a PointYZ array of length 2.
        public static void Triangulate(Polygon poly,  CrossSections.CrossSection CS)
        {
            PointYZ center = new PointYZ(0,0);
            List<Triangle> triangles = new List<Triangle>();  // accumulate the triangles here
            // keep clipping ears off of poly until only one triangle remains
            while (poly.PtListOpen.Count > 3)  // if only 3 points are left, we have the final triangle
            {
                int midvertex = FindEar(poly);  // find the middle vertex of the next "ear"
                Triangle t = new Triangle(poly.PtList[midvertex - 1], poly.PtList[midvertex], poly.PtList[midvertex + 1]);    
                CS.Area += t.Area;
                center.Y += t.Area * t.Center.Y;
                center.Z += t.Area * t.Center.Z;
                
                triangles.Add(t);
                // create a new polygon that clips off the ear; i.e., all vertices but midvertex
                List<PointYZ> newPts = new List<PointYZ>(poly.PtList);
                newPts.RemoveAt(midvertex);  // clip off the ear
                poly = new Polygon(newPts);  // poly now has one less point
            }
            // only a single triangle remains, so add it to the triangle list
            var lastt = new Triangle(poly.PtListOpen.ToArray());
            CS.Area += lastt.Area;
            center.Y += lastt.Area * lastt.Center.Y;
            center.Z += lastt.Area * lastt.Center.Z;
            triangles.Add(lastt);
            CS.Centroid = new PointYZ(center.Y / CS.Area, center.Z / CS.Area);
           
            foreach (Triangle t in triangles)
            {
                CS.Iy += t.Iy + Math.Pow(CS.Centroid.Z - t.Center.Z, 2) * t.Area;
                CS.Iz += t.Iz + Math.Pow(CS.Centroid.Y - t.Center.Y, 2) * t.Area;
                
            }
  
        }

        // find an ear (always a triangle) of the polygon and return the index of the middle (second) vertex in the ear
        public static int FindEar(Polygon poly)
        {
            for (int i = 0; i < poly.PtList.Count - 2; i++)
            {
                if (poly.VertexType(i + 1) == PolygonType.Convex)
                {
                    // get the three points of the triangle we are about to test
                    PointYZ a = poly.PtList[i];
                    PointYZ b = poly.PtList[i + 1];
                    PointYZ c = poly.PtList[i + 2];
                    bool foundAPointInTheTriangle = false;  // see if any of the other points in the polygon are in this triangle
                    for (int j = 0; j < poly.PtListOpen.Count; j++)  // don't check the last point, which is a duplicate of the first
                    {
                        if (j != i && j != i + 1 && j != i + 2 && PointInTriangle(poly.PtList[j], a, b, c)) foundAPointInTheTriangle = true;
                    }
                    if (!foundAPointInTheTriangle)  // the middle point of this triangle is convex and none of the other points in the polygon are in this triangle, so it is an ear
                        return i + 1;  // EXITING HERE!
                }
            }
            throw new ApplicationException("Improperly formed polygon");
        }

        // return true if point p is inside the triangle a,b,c
        public static bool PointInTriangle(PointYZ p, PointYZ a, PointYZ b, PointYZ c)
        {
            // three tests are required.
            // if p and c are both on the same side of the line a,b
            // and p and b are both on the same side of the line a,c
            // and p and a are both on the same side of the line b,c
            // then p is inside the triangle, o.w., not
            return PointsOnSameSide(p, a, b, c) && PointsOnSameSide(p, b, a, c) && PointsOnSameSide(p, c, a, b);
        }

        // if the two points p1 and p2 are both on the same side of the line a,b, return true
        private static bool PointsOnSameSide(PointYZ p1, PointYZ p2, PointYZ a, PointYZ b)
        {
            // these are probably the most interesting three lines of code in the algorithm (probably because I don't fully understand them)
            // the concept is nicely described at http://www.blackpawn.com/texts/pointinpoly/default.html
            double cp1 = CrossProduct(VSub(b, a), VSub(p1, a));
            double cp2 = CrossProduct(VSub(b, a), VSub(p2, a));
            return (cp1 * cp2) >= 0;  // they have the same sign if on the same side of the line
        }

        // subtract the vector (point) b from the vector (point) a
        private static PointYZ VSub(PointYZ a, PointYZ b)
        {
            return new PointYZ(a.Y - b.Y, a.Z - b.Z);
        }

        // find the cross product of two x,y vectors, which is always a single value, z, representing the three dimensional vector (0,0,z)
        private static double CrossProduct(PointYZ p1, PointYZ p2)
        {
            return (p1.Y * p2.Z) - (p1.Z * p2.Y);
        }
    }
}
