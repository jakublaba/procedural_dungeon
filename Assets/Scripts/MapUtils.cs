using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using QuickGraph;
using QuickGraph.Algorithms;

public class MapUtils : MonoBehaviour {
    private Polygon polygon;
    private TriangleNet.Mesh mesh;
    private UndirectedGraph<Vector2,Edge<Vector2>> graph;
    private IEnumerable<Edge<Vector2>> mst;

    public IEnumerator DelaunayTriangulation(List<GameObject> rooms, float delay) {
        Debug.Log("Delaunay triangulation started");
        Debug.Log(rooms.Count);

        polygon = new Polygon();
        foreach(GameObject room in rooms) {
            polygon.Add(new Vertex(room.transform.position.x, room.transform.position.y));
        }
        mesh = (TriangleNet.Mesh)polygon.Triangulate();

        yield return new WaitForSeconds(delay);
        StartCoroutine(MST(delay));
    }

    public IEnumerator MST(float delay) {
        Debug.Log("MST coroutine started");
    
        // First wrap the Triangle.NET dictionary into a graph
        graph = new UndirectedGraph<Vector2,Edge<Vector2>>();
        foreach(Edge edge in mesh.Edges) {
            Dictionary<int,Vertex> vertices = mesh.getVertices();
            Vertex v0 = vertices[edge.P0];
            Vertex v1 = vertices[edge.P1];
            Vector2 src = new Vector2((float)v0.X, (float)v0.Y);
            Vector2 dst = new Vector2((float)v1.X, (float)v1.Y);
            Edge<Vector2> e = new Edge<Vector2>(src, dst);
            graph.AddVerticesAndEdge(e);
        }

        // Now MST that bad boi
        Func<Edge<Vector2>,double> weightFunc = delegate(Edge<Vector2> e) {
            return Vector2.Distance(e.Source, e.Target);
        };
        mst = graph.MinimumSpanningTreePrim(weightFunc);
        //mst = graph.MinimumSpanningTreeKruskal(weightFunc);
    
        yield return new WaitForSeconds(delay);
        StartCoroutine(VisualizeGraph(delay));
    }

    private IEnumerator VisualizeGraph(float delay) {
        // Old version, drawing directly from Triangle.NET data structure
        //foreach(Edge edge in mesh.Edges) {
        //    Dictionary<int,Vertex> vertices = mesh.getVertices();
        //    Vertex v0 = vertices[edge.P0];
        //    Vertex v1 = vertices[edge.P1];
        //    Vector2 p0 = new Vector2((float)v0.X, (float)v0.Y);
        //    Vector2 p1 = new Vector2((float)v1.X, (float)v1.Y);
        //    DrawLine(p0, p1, Color.red);
        //    yield return new WaitForSeconds(delay);
        //}

        foreach(var edge in mst) {
            DrawLine(edge.Source, edge.Target, Color.red);
            yield return new WaitForSeconds(delay);
        }

        yield return null;
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color) {
        GameObject line = new GameObject();
        line.transform.position = start;
        line.AddComponent<LineRenderer>();
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
