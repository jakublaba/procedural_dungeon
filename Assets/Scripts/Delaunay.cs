using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;

public class Delaunay : MonoBehaviour {
    private Polygon polygon;
    private TriangleNet.Meshing.ConstraintOptions options;
    private TriangleNet.Mesh mesh;

    public IEnumerator Triangulate(List<GameObject> rooms, float delay) {
        Debug.Log("Delaunay triangulation started");
        Debug.Log(rooms.Count);

        polygon = new Polygon();
        foreach(GameObject room in rooms) {
            polygon.Add(new Vertex(room.transform.position.x, room.transform.position.y));
        }

        //options = new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
        mesh = (TriangleNet.Mesh)polygon.Triangulate();

        yield return new WaitForSeconds(delay);

        StartCoroutine(Visualize(delay));
    }

    private IEnumerator Visualize(float delay) {
        foreach(Edge edge in mesh.Edges) {
            Vertex v0 = mesh.vertices[edge.P0];
            Vertex v1 = mesh.vertices[edge.P1];
            Vector2 p0 = new Vector2((float)v0.x, (float)v0.y);
            Vector2 p1 = new Vector2((float)v1.x, (float)v1.y);
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
