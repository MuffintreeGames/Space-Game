using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAfterDelay : MonoBehaviour
{
    public GameObject spawnedObject;
    public float timeToSpawn = 1f;

    private float timeElapsed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > timeToSpawn)
        {
            Instantiate(spawnedObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
