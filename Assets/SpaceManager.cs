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

    public GameObject BigBang;
    public float bigBangTime = 60f; //time at which big bang should begin

    private float chunkSize = 10f; //dimensions of 1 chunk
    private int sectorDimensions = 10; //dimensions of 1 sector in terms of the number of chunks

    private CircleCollider2D smallPlanetCollider;

    private GameObject[][] sector1Map;   //all the contents of the chunks in sector 1

    private float timeElapsed = 0f;
    private bool bigBangSpawned = false;

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
            float planetRadius = templateCollider.radius * planetTemplate.transform.localScale.x;
            float leftChunkLimit = (chunkX * chunkSize) + planetRadius;   //get coords for furthest left possible place we could put planet in chunk
            float rightChunkLimit = ((chunkX + 1f) * chunkSize) - planetRadius;
            float bottomChunkLimit = (chunkY * chunkSize) + planetRadius;
            float topChunkLimit = ((chunkY + 1f) * chunkSize) - planetRadius;
            float planetXCoords = Random.Range(leftChunkLimit, rightChunkLimit);
            float planetYCoords = Random.Range(bottomChunkLimit, topChunkLimit);
            Vector3 planetCoords = new Vector3(planetXCoords, planetYCoords, 0);
            GameObject newPlanet = Instantiate(planetTemplate, planetCoords, Quaternion.identity);
        newPlanet.GetComponent<SpriteRenderer>().color = PickRandomPlanetColor();
            sectorMap[chunkX][chunkY] = newPlanet;
    }

    Color PickRandomPlanetColor()
    {
        int randomNumber = Random.Range(1, 8);
        switch (randomNumber) {
            case 1:
                return Color.red;
            case 2:
                return Color.blue;
            case 3:
                return Color.green;
            case 4:
                return Color.yellow;
            case 5:
                return new Color(156, 0, 255, 255);
            case 6:
                return new Color(255, 133, 0, 255);
            case 7:
                return Color.cyan;
            case 8:
                return Color.magenta;
        }
        return Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if ((timeElapsed >= bigBangTime) && !bigBangSpawned)
        {
            Debug.Log("spawning big bang");
            Instantiate(BigBang, new Vector3(0, 0, 0), Quaternion.identity);
            bigBangSpawned = true;
        }

        timeElapsed += Time.deltaTime;
        //Debug.Log("time elapsed: " + timeElapsed);
    }
}
