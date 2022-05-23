using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils : MonoBehaviour {
    private const int radius = 10;
    private const int roomAmount = 100;
    private const int roomAmountPreserved = 30;
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
        GameObject room = rooms[0];
        room.tag = "RoomClone";
        Instantiate(room, coords, Quaternion.identity);
    }

    public IEnumerator SpawnRoomsWithDelay(float delay) {
        Debug.Log("SpawnRoomsWithDelay couroutine started");

        for(int i = 0; i < roomAmount; i++) {
            SpawnRoom();
            yield return new WaitForSeconds(delay);
        }
        StartCoroutine(ResizeRooms(delay));
    }

    private IEnumerator ResizeRooms(float delay) {
        Debug.Log("ResizeRooms couroutine started");

        foreach(GameObject room in roomClones) {
            Vector2 targetSize = new Vector2(Random.Range(1,10), Random.Range(1,10));
            Mesh mesh = room.GetComponent<MeshFilter>().sharedMesh;
            Bounds meshBounds = mesh.bounds;
            Vector2 meshSize = meshBounds.size;
            float xScale = targetSize.x / meshSize.x;
            float yScale = targetSize.y / meshSize.y;
            transform.localScale = new Vector2(xScale, yScale);
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
        yield return new WaitForSeconds(3);         // need to remove magic number later
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
        StartCoroutine(SpawnRoomContent(delay));
    }

    private IEnumerator SpawnRoomContent(float delay) {
        Debug.Log("SpawnRoomContent couroutine started");

        List<GameObject> nums = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Templates>().numbers;
        foreach(GameObject num in nums) {
            int rand = UnityEngine.Random.Range(0, roomClones.Count-1);
            Vector2 coords = roomClones[rand].transform.position;
            Instantiate(num, coords, Quaternion.identity);
            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }

    private IEnumerator LogRoomCoordinates(float delay) {
        Debug.Log("Room coordinates:");
        
        foreach(GameObject room in roomClones) {
            Debug.Log(System.String.Format("({0},{1})", room.transform.position.x, room.transform.position.y));
        }
        yield return new WaitForSeconds(delay);

        MapUtils map = GameObject.Find("GenerationScripts").GetComponent<MapUtils>();
        StartCoroutine(map.DelaunayTriangulation(roomClones, delay));
    }
}
