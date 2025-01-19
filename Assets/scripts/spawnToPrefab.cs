using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnToPrefab : MonoBehaviour
{
    public GameObject[] objectsToSpawn;  // Prefabs to spawn
    private Transform playgroundArea;   // Area for spawning objects
    public float spawnHeight = 5f;      // Height at which objects spawn

    void Start()
    {
        SpawnObjects();
    }
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("spawnedObject").Length == 0)
        {
            SpawnObjects();
        }
    }


    void SpawnObjects()
    {
        if (objectsToSpawn == null || objectsToSpawn.Length == 0)
        {
            UnityEngine.Debug.LogError("objectsToSpawn listesi boþ! Lütfen Inspector'da prefab'lar ekleyin.");
            return;
        }

        foreach (GameObject prefab in objectsToSpawn)
        {
            if (prefab == null)
            {
                UnityEngine.Debug.LogError("Listede null bir prefab bulundu! Lütfen kontrol edin.");
                continue;
            }

            // Generate a single unique ID for both spawned objects
            int uniqueID = UnityEngine.Random.Range(0, 10000);

            // Spawn first object
            Vector3 spawnPosition1 = GetRandomSpawnPosition();
            GameObject spawnedObject1 = Instantiate(prefab, spawnPosition1, Quaternion.identity);
            ConfigureRigidbody(spawnedObject1);
            AssignUniqueID(spawnedObject1, uniqueID);

            // Spawn second object
            Vector3 spawnPosition2 = GetRandomSpawnPosition();
            GameObject spawnedObject2 = Instantiate(prefab, spawnPosition2, Quaternion.identity);
            ConfigureRigidbody(spawnedObject2);
            AssignUniqueID(spawnedObject2, uniqueID);
        }
    }

    // Function to assign a unique ID to the object
    void AssignUniqueID(GameObject obj, int uniqueID)
    {
        obj.name = obj.name + "_" + uniqueID;    // Assign the same unique ID to both objects
        obj.AddComponent<ObjectID>().id = uniqueID;  // Create a new script component to store the ID
    }

    // Function to get random spawn position within bounds
    Vector3 GetRandomSpawnPosition()
    {
        float xPos = UnityEngine.Random.Range(-5f, 5f);
        float zPos = UnityEngine.Random.Range(-3f, 5f);
        return new Vector3(xPos, spawnHeight, zPos);
    }

    // Function to configure the Rigidbody of the spawned object
    void ConfigureRigidbody(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.isKinematic = false;
    }
}
