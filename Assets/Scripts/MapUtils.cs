using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using QuickGraph;
using QuickGraph.Algorithms;

public class MapUtils : MonoBehaviour
{
    private Polygon polygon;
    private TriangleNet.Mesh mesh;
    private UndirectedGraph<Vector2,Edge<Vector2>> graph;
    private IEnumerable<Edge<Vector2>> mst;

    public IEnumerator DelaunayTriangulation(List<GameObject> rooms)
    {
        Debug.Log("Delaunay triangulation started");
        Debug.Log(rooms.Count);

        polygon = new Polygon();
        foreach(GameObject room in rooms)
        {
            List<Vector2> points = new List<Vector2>(room.GetComponent<PolygonCollider2D>().points);
            List<Vector2> actualPoints = new List<Vector2>(points.Count);
            Vector2 shift = room.transform.position;
            for(int i=0; i<points.Count; i++)
            {
                actualPoints.Add(new Vector2(points[i].x+shift.x, points[i].y+shift.y));
            }
            Vector2 centroid = ValtrPolygons.PolygonCentroid(actualPoints);

            polygon.Add(new Vertex(centroid.x, centroid.y));
        }
        mesh = (TriangleNet.Mesh)polygon.Triangulate();

        yield return new WaitForSeconds(Controller.delay);
        StartCoroutine(MST());
    }

    private IEnumerator MST()
    {
        Debug.Log("MST coroutine started");
    
        graph = new UndirectedGraph<Vector2,Edge<Vector2>>();
        foreach(Edge edge in mesh.Edges)
        {
            Dictionary<int,Vertex> vertices = mesh.getVertices();
            Vertex v0 = vertices[edge.P0];
            Vertex v1 = vertices[edge.P1];
            Vector2 src = new Vector2((float)v0.X, (float)v0.Y);
            Vector2 dst = new Vector2((float)v1.X, (float)v1.Y);
            Edge<Vector2> e = new Edge<Vector2>(src, dst);
            graph.AddVerticesAndEdge(e);
        }

        Func<Edge<Vector2>,double> weightFunc = delegate(Edge<Vector2> e)
        {
            return Vector2.Distance(e.Source, e.Target);
        };
        mst = graph.MinimumSpanningTreePrim(weightFunc);

        yield return null;
        // Commented out methods used for debug only
        //StartCoroutine(VisualizeDelaunay());
        //StartCoroutine(VisualizeGraph());
        StartCoroutine(DrawCorridors());
    }

    private IEnumerator VisualizeDelaunay()
    {
        Debug.Log("VisualizeDelaunay coroutine started");

        foreach(TriangleNet.Geometry.Edge edge in mesh.Edges)
        {
            Dictionary<int,Vertex> vertices = mesh.getVertices();
            Vertex v0 = vertices[edge.P0];
            Vertex v1 = vertices[edge.P1];
            Vector2 p0 = new Vector2((float)v0.X, (float)v0.Y);
            Vector2 p1 = new Vector2((float)v1.X, (float)v1.Y);
            DrawLine(p0, p1, 0.05f, Color.yellow);
            yield return new WaitForSeconds(Controller.delay);
        }
        yield return null;
    }

    private IEnumerator VisualizeGraph()
    {
        Debug.Log("VisualizeGraph coroutine started");

        foreach(QuickGraph.Edge<Vector2> edge in mst)
        {
            DrawLine(edge.Source, edge.Target, 0.1f, Color.red);
            yield return new WaitForSeconds(Controller.delay);
        }

        yield return null;
    }

    private IEnumerator DrawCorridors()
    {
        Debug.Log("DrawCorridors coroutine started");
        
        foreach(QuickGraph.Edge<Vector2> edge in mst)
        {
            Vector2 a = edge.Source;
            Vector2 b = edge.Target;
            Vector2 c = ValtrPolygons.RandBool() ? new Vector2(a.x, b.y) : new Vector2(b.x, a.y);
            DrawLine(a, c, 0.1f, Color.blue);
            DrawLine(b, c, 0.1f, Color.blue);
            yield return new WaitForSeconds(Controller.delay);
        }
        
        yield return null;
        StartCoroutine(SpawnRoomContent());        
    }

    private IEnumerator SpawnRoomContent()
    {
        Debug.Log("SpawnRoomContent coroutine started");

        HashSet<Vector2> roomCoordSet = new HashSet<Vector2>();
        foreach(var edge in mst)
        {
            roomCoordSet.Add(edge.Source);
            roomCoordSet.Add(edge.Target);
        }
        List<Vector2> roomCoords = roomCoordSet.ToList();

        // items and places
        List<GameObject> contents = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Templates>().roomContents;
        foreach(GameObject item in contents)
        {
            int rand = UnityEngine.Random.Range(0, roomCoords.Count-1);
            Vector2 coords = roomCoords[rand];
            roomCoords.RemoveAt(rand);
            Instantiate(item, coords, Quaternion.identity);
        }

        // monsters
        List<GameObject> monsters = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Templates>().monsters;
        foreach(GameObject monster in monsters)
        {
            if(roomCoords.Count == 0) break;
            int rand = UnityEngine.Random.Range(0, roomCoords.Count-1);
            Vector2 coords = roomCoords[rand];
            roomCoords.RemoveAt(rand);
            Instantiate(monster, coords, Quaternion.identity);
        }

        yield return new WaitForSeconds(Controller.delay);
    }

    public void DrawLine(Vector2 start, Vector2 end, float width, Color color)
    {
        GameObject line = new GameObject();
        line.transform.position = start;
        line.AddComponent<LineRenderer>();
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
