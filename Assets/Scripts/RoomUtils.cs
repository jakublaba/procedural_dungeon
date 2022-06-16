using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils : MonoBehaviour
{
    private const int mapRadius = 10;
    private const float roomRadius = 3;
    private const int roomAmount = 50;
    private const int roomAmountPreserved = 10;
    private MapUtils map;
    public List<GameObject> roomClones;

    private Vector2 RandomPointInCircle()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
        float angle = 2*Mathf.PI*UnityEngine.Random.value;
        float u = 2*UnityEngine.Random.value;
        float r = u > 1 ? 2-u : u;

        float x = mapRadius*r*Mathf.Cos(angle);
        float y = mapRadius*r*Mathf.Sin(angle);
        return new Vector2(x, y);
    }

    private void SpawnPolygonRoom()
    {
        Vector2 center = RandomPointInCircle();
        int nSides = 4;
        List<Vector2> pVertices = ValtrPolygons.RandomConvexPolygon(nSides, roomRadius);
    
        GameObject room = new GameObject();
        room.tag = "RoomClone";
        room.transform.position = center;

        room.AddComponent<Rigidbody2D>();
        room.AddComponent<PolygonCollider2D>();
        Rigidbody2D roomBody = room.GetComponent<Rigidbody2D>();
        PolygonCollider2D roomCollider = room.GetComponent<PolygonCollider2D>();
        roomBody.freezeRotation = true;
        roomBody.isKinematic = true;
        roomCollider.pathCount = nSides;
        roomCollider.points = pVertices.ToArray();
    }

    private void DrawRoom(GameObject room)
    {
        Vector2 shift = room.transform.position;
        List<Vector2> pVertices = new List<Vector2>(room.GetComponent<PolygonCollider2D>().points);
        int nSides = pVertices.Count;
        room.AddComponent<LineRenderer>();
        LineRenderer roomRenderer = room.GetComponent<LineRenderer>();
        roomRenderer.material = new Material(Shader.Find("Sprites/Default"));
        roomRenderer.startColor = Color.black;
        roomRenderer.endColor = Color.black;
        roomRenderer.startWidth = 0.08f;
        roomRenderer.endWidth = 0.08f;
        roomRenderer.positionCount = nSides;
        for(int i=0; i<nSides; i++)
        {
            Vector2 polygonPoint = pVertices[i];
            Vector2 actualPosition = new Vector2(polygonPoint.x+shift.x, polygonPoint.y+shift.y);
            roomRenderer.SetPosition(i, actualPosition);
        }
        roomRenderer.loop = true;
    }

    public IEnumerator SpawnRoomsWithDelay()
    {
        Debug.Log("SpawnRoomsWithDelay coroutine started");

        map = GameObject.Find("GenerationScripts").GetComponent<MapUtils>();
        for(int i = 0; i < roomAmount; i++)
        {
            SpawnPolygonRoom();
        }

        yield return new WaitForSeconds(Controller.delay);
        StartCoroutine(PushRooms());
    }

    private IEnumerator PushRooms()
    {
        Debug.Log("PushRooms coroutine started");

        roomClones = new List<GameObject>(GameObject.FindGameObjectsWithTag("RoomClone"));
        Debug.Log(roomClones.Count);
        foreach(GameObject room in roomClones)
        {
            room.GetComponent<Rigidbody2D>().isKinematic = false;
        }

        yield return new WaitForSeconds(Controller.delay*roomAmount);
        StartCoroutine(RemoveExcessRooms());
    }

    private IEnumerator RemoveExcessRooms()
    {
        Debug.Log("RemoveExcessRooms coroutine started");

        while(roomClones.Count > roomAmountPreserved)
        {
            int rand = UnityEngine.Random.Range(0, roomClones.Count-1);
            GameObject.Destroy(roomClones[rand]);
            roomClones.RemoveAt(rand);
        }

        yield return new WaitForSeconds(Controller.delay);
        StartCoroutine(DrawRooms());
    }

    private IEnumerator DrawRooms()
    {
        Debug.Log("DrawRooms coroutine started");

        foreach(GameObject room in roomClones)
        {
            DrawRoom(room);
        }

        yield return new WaitForSeconds(Controller.delay);
        StartCoroutine(CorrectRoomCoordinates());
    }

    private IEnumerator CorrectRoomCoordinates()
    {
        Debug.Log("CorrectRoomCoordinates coroutine started");

        foreach(GameObject room in roomClones)
        {
            List<Vector2> points = new List<Vector2>(room.GetComponent<PolygonCollider2D>().points);
            List<Vector2> actualPoints = new List<Vector2>(points.Count);
            Vector2 shift = room.transform.position;

            for(int i=0; i<points.Count; i++)
            {
                actualPoints.Add(new Vector2(points[i].x+shift.x, points[i].y+shift.y));
            }
        }

        yield return new WaitForSeconds(Controller.delay);
        StartCoroutine(map.DelaunayTriangulation(roomClones));
    }

}