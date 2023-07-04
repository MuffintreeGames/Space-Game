using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAfterDelay : MonoBehaviourPun
{
    public GameObject spawnedObject;
    public float timeToSpawn = 1f;
    public bool maintainRotation = false;   //if true, spawned object will have the same rotation as this one

    private float timeElapsed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !PhotonView.Get(this).AmOwner)
        {
            return;
        }

        timeElapsed += Time.deltaTime;
        if (timeElapsed > timeToSpawn)
        {
            Quaternion targetRotation = Quaternion.identity;
            if (maintainRotation)
            {
                targetRotation = transform.rotation;
            }
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Instantiate(spawnedObject.name, transform.position, targetRotation);
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Instantiate(spawnedObject, transform.position, targetRotation);
                Destroy(gameObject);
            }
        }
    }
}
