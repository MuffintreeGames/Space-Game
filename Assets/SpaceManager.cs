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
    public GameObject MediumPlanet;
    public GameObject LargePlanet;
    public GameObject MassivePlanet;

    private float chunkSize = 10f; //dimensions of 1 chunk
    private int sectorDimensions = 10; //dimensions of 1 sector in terms of the number of chunks

    private CircleCollider2D smallPlanetCollider;

    private GameObject[][] sector1Map;   //all the contents of the chunks in sector 1

    // Start is called before the first frame update
    void Start()    //divide space up into several sectors, which are then broken up into smaller chunks which can each contain a max of 1 object
    {
        smallPlanetCollider = SmallPlanet.GetComponent<CircleCollider2D>();
        Debug.Log("building sector 1");
        BuildSector1();
    }

    void BuildSector1()  //area the goliath spawns in, mostly small planets
    {
        sector1Map = new GameObject[sectorDimensions][];
        for (int x = 0; x < sectorDimensions; x++)
        {
            sector1Map[x] = new GameObject[sectorDimensions];
        }

        SpawnRandomPlanets(10, 15, MediumPlanet, sector1Map);
        SpawnRandomPlanets(7, 10, LargePlanet, sector1Map);
        for (int x = 0; x < sectorDimensions; x++)
        {
            for (int y = 0; y < sectorDimensions; y++)
            {
                if (sector1Map[x][y] == null)
                {
                    PlacePlanetInChunk(x, y, SmallPlanet, smallPlanetCollider, sector1Map);
                }
            }
        }
    }

    void SpawnRandomPlanets(int min, int max, GameObject planetTemplate, GameObject[][] sectorMap)
    {
        int finalNumber = Random.Range(min, max);
        CircleCollider2D templateCollider = planetTemplate.GetComponent<CircleCollider2D>();
        for (int n = 0; n < finalNumber; n++)
        {
            bool planetPlaced = false;
            while (!planetPlaced) {
                int randX = Random.Range(0, sectorDimensions);
                int randY = Random.Range(0, sectorDimensions);
                if (sectorMap[randX][randY] == null)
                {
                    PlacePlanetInChunk(randX, randY, planetTemplate, templateCollider, sectorMap);
                    planetPlaced = true;
                }
            }
        }
    }

    void PlacePlanetInChunk(int chunkX, int chunkY, GameObject planetTemplate, CircleCollider2D templateCollider, GameObject[][] sectorMap)
    {
            float leftChunkLimit = (chunkX * chunkSize) + templateCollider.radius;   //get coords for furthest left possible place we could put planet in chunk
            float rightChunkLimit = ((chunkX + 1f) * chunkSize) - templateCollider.radius;
            float bottomChunkLimit = (chunkY * chunkSize) + templateCollider.radius;
            float topChunkLimit = ((chunkY + 1f) * chunkSize) - templateCollider.radius;
            float planetXCoords = Random.Range(leftChunkLimit, rightChunkLimit);
            float planetYCoords = Random.Range(bottomChunkLimit, topChunkLimit);
            Vector3 planetCoords = new Vector3(planetXCoords, planetYCoords, 0);
            sectorMap[chunkX][chunkY] = Instantiate(planetTemplate, planetCoords, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
