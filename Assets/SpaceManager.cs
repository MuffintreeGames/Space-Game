using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*private class Sector : MonoBehaviour
{
    public GameObject contents;

}*/

public class SpaceManager : MonoBehaviour
{

    public GameObject SmallPlanet;

    private float chunkSize = 10f; //dimensions of 1 chunk
    private int sectorDimensions = 10; //dimensions of 1 sector in terms of the number of chunks

    private CircleCollider2D smallPlanetCollider;



    // Start is called before the first frame update
    void Start()    //divide space up into several sectors, which are then broken up into smaller chunks which can each contain a max of 1 object
    {
        smallPlanetCollider = SmallPlanet.GetComponent<CircleCollider2D>();
        Debug.Log("building sector 1");
        BuildChunk1();
    }

    void BuildChunk1()  //area the goliath spawns in, mostly small planets
    {
        for (int x = 0; x < sectorDimensions; x++)
        {
            for (int y = 0; y < sectorDimensions; y++)
            {
                float leftChunkLimit = (x  * chunkSize) + smallPlanetCollider.radius;   //get coords for furthest left possible place we could put small planet in chunk
                float rightChunkLimit = ((x + 1f) * chunkSize) - smallPlanetCollider.radius;
                float bottomChunkLimit = (y * chunkSize) + smallPlanetCollider.radius;
                float topChunkLimit = ((y + 1f) * chunkSize) - smallPlanetCollider.radius;
                float planetXCoords = Random.Range(leftChunkLimit, rightChunkLimit);
                float planetYCoords = Random.Range(bottomChunkLimit, topChunkLimit);
                Vector3 planetCoords = new Vector3(planetXCoords, planetYCoords, 0);
                Debug.Log("spawning planet at " + planetCoords);
                Instantiate(SmallPlanet, planetCoords, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
