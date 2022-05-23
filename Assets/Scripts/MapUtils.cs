using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using QuickGraph;

public class MapUtils : MonoBehaviour {
    private Polygon polygon;
    private TriangleNet.Mesh mesh;

    public IEnumerator DelaunayTriangulation(List<GameObject> rooms, float delay) {
        Debug.Log("Delaunay triangulation started");
        Debug.Log(rooms.Count);

        polygon = new Polygon();
        foreach(GameObject room in rooms) {
            polygon.Add(new Vertex(room.transform.position.x, room.transform.position.y));
        }
        mesh = (TriangleNet.Mesh)polygon.Triangulate();

        yield return new WaitForSeconds(delay);
        StartCoroutine(VisualizeGraph(delay));
    }

    public IEnumerator MST(float delay) {
        Debug.Log("MST coroutine started");
        
        var graph = mesh.getVertices().ToVertexAndEdgeListGraph(
            kv => Array.ConvertAll<int,SEquatableEdge<int>>(kv.Value, v => new SEquatableEdge<int>(kv.Key, v))
        );

        yield return new WaitForSeconds(delay);
    }

    private IEnumerator VisualizeGraph(float delay) {
        foreach(Edge edge in mesh.Edges) {
            Dictionary<int,Vertex> vertices = mesh.getVertices();
            Vertex v0 = vertices[edge.P0];
            Vertex v1 = vertices[edge.P1];
            Vector2 p0 = new Vector2((float)v0.X, (float)v0.Y);
            Vector2 p1 = new Vector2((float)v1.X, (float)v1.Y);
            DrawLine(p0, p1, Color.red);
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
