using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ValtrPolygons
{
    private static System.Random r = new System.Random();

    public static bool RandBool()
    {
        return r.Next() > Int32.MaxValue/2;
    }

    public static float PolygonArea(List<Vector2> points)
    {
        var p = points;
        p.Add(points[0]);
        float area = 0;

        for(int i=0; i<points.Count-1; i++)
        {
            area += 0.5f*(p[i].x*p[i+1].y-p[i+1].x*p[i].y);
        }
        
        return area;
    }

    public static Vector2 PolygonCentroid(List<Vector2> points)
    {
        var p = points;
        p.Add(points[0]);
        float area = PolygonArea(points);
        float cx = 0;
        float cy = 0;

        for(int i=0; i<points.Count-1; i++)
        {
            float q = (p[i].x*p[i+1].y-p[i+1].x*p[i].y);
            cx += (1/(6*area))*(p[i].x+p[i+1].x)*q;
            cy += (1/(6*area))*(p[i].y+p[i+1].y)*q;
        }

        return new Vector2(cx, cy);
    }

    public static List<Vector2> RandomConvexPolygon(int nSides, float radius)
    {
        List<float> xPool = new List<float>(nSides);
        List<float> yPool = new List<float>(nSides);

        for(int i=0; i<nSides; i++)
        {
            xPool.Add(UnityEngine.Random.Range(-1*radius, radius));
            yPool.Add(UnityEngine.Random.Range(-1*radius, radius));
        }

        xPool.Sort();
        yPool.Sort();

        Vector2 min = new Vector2(xPool.First(), yPool.First());
        Vector2 max = new Vector2(xPool.Last(), yPool.Last());

        List<float> xVec = new List<float>(nSides);
        List<float> yVec = new List<float>(nSides);

        float lastTop = min.x;
        float lastBot = min.x;

        for(int i=1; i<nSides-1; i++)
        {
            if(RandBool())
            {
                xVec.Add(xPool[i]-lastTop);
                lastTop = xPool[i];
            } 
            else
            {
                xVec.Add(lastBot-xPool[i]);
                lastBot = xPool[i];
            }
        }
        xVec.Add(max.x-lastTop);
        xVec.Add(lastBot-max.x);

        float lastLeft = min.y;
        float lastRight = min.y;

        for(int i=0; i<nSides-1; i++)
        {
            if(RandBool())
            {
                yVec.Add(yPool[i]-lastLeft);
                lastLeft = yPool[i];
            }
            else
            {
                yVec.Add(lastRight-yPool[i]);
                lastRight = yPool[i];
            }
        }
        yVec.Add(max.y-lastLeft);
        yVec.Add(lastRight-max.y);

        yVec = yVec.OrderBy(i => r.Next()).ToList();
        List<Vector2> vec = new List<Vector2>(nSides);
        for(int i=0; i<nSides; i++) 
        {
            vec.Add(new Vector2(xVec[i], yVec[i]));
        }

        vec.Sort((v1,v2) =>
        {
            float angle1 = Mathf.Atan2(v1.x, v1.y);
            float angle2 = Mathf.Atan2(v2.x, v2.y);
            return angle1.CompareTo(angle2);
        });

        // Returned polygon is always oriented in a way, where 1st point is at (0,0) coordinates
        float x = 0;
        float y = 0;
        List<Vector2> points = new List<Vector2>(nSides);
        for(int i=0; i<nSides; i++)
        {
            points.Add(new Vector2(x, y));
            x += vec[i].x;
            y += vec[i].y;
        }

        return points;
    }

}