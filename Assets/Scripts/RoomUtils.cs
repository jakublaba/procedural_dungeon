using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils : MonoBehaviour {
    private const int radius = 5;
    private const int roomAmount = 50;
    private const int roomAmountPreserved = 15;
    public List<GameObject> roomClones;

    private Vector2 GetRandomPointInCircle(int radius) {
        int seed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
        float t = 2*Mathf.PI*UnityEngine.Random.value;
        float u = 2*UnityEngine.Random.value;
        float r = u > 1 ? 2-u : u;
        return new Vector2(radius*r*Mathf.Cos(t), radius*r*Mathf.Sin(t));
    }

    private void SpawnRoom() {
        Vector2 coords = GetRandomPointInCircle(radius);
        List<GameObject> rooms = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Templates>().rooms;
        int rand = UnityEngine.Random.Range(0, rooms.Count);
        GameObject room = rooms[rand];
        room.tag = "RoomClone";
        //room.GetComponent<Rigidbody2D>().isKinematic = true;
        Instantiate(room, coords, Quaternion.identity);
    }

    public IEnumerator SpawnRoomsWithDelay(float delay) {
        Debug.Log("SpawnRoomsWithDelay couroutine started");

        for(int i = 0; i < roomAmount; i++) {
            SpawnRoom();
            yield return new WaitForSeconds(delay);
        }
        StartCoroutine(PushRooms(delay));
    }

    private IEnumerator PushRooms(float delay) {
        Debug.Log("PushRooms couroutine started");

        roomClones = new List<GameObject>(GameObject.FindGameObjectsWithTag("RoomClone"));
        foreach(GameObject room in roomClones) {
            room.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        yield return new WaitForSeconds(10*delay);
        StartCoroutine(RemoveExcessRooms(delay));
    }

    private IEnumerator RemoveExcessRooms(float delay) {
        Debug.Log("RemoveExcessRooms couroutine started");

        while(roomClones.Count > roomAmountPreserved) {
            int rand = UnityEngine.Random.Range(0, roomClones.Count-1);
            GameObject.Destroy(roomClones[rand]);
            roomClones.RemoveAt(rand);
            yield return new WaitForSeconds(delay);
        }
        StartCoroutine(LogRoomCoordinates(delay));
    }

    private IEnumerator LogRoomCoordinates(float delay) {
        Debug.Log("Room coordinates:");
        
        foreach(GameObject room in roomClones) {
            Debug.Log(System.String.Format("({0},{1})", room.transform.position.x, room.transform.position.y));
        }
        yield return new WaitForSeconds(delay);

        Delaunay delaunay = GameObject.Find("GenerationScripts").GetComponent<Delaunay>();
        StartCoroutine(delaunay.Triangulate(roomClones, delay));
    }
}
