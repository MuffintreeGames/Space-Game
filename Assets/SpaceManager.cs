using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SectorCoordinates
{
    public int x;
    public int y;

    public SectorCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class SpaceManager : MonoBehaviour
{

    public GameObject SmallPlanet;
    public GameObject MediumPlanet;
    public GameObject LargePlanet;
    public GameObject MassivePlanet;
    public GameObject AbilityPlanet;

    public GameObject Level1Barrier;
    public GameObject Level2Barrier;
    public GameObject Level3Barrier;

    public GameObject BigBang;
    public float bigBangTime = 60f; //time at which big bang should begin

    public List<AbilityTemplate> AbilityPool;
    private List<AbilityTemplate> ModifiedAbilityPool;

    private static float chunkSize = 10f; //dimensions of 1 chunk
    private static int sectorDimensions = 10; //dimensions of 1 sector in terms of the number of chunks
    private static float sectorSize;
    private static int worldSize = 8; //number of sectors in x and y directions

    private static CircleCollider2D smallPlanetCollider;

    private static GameObject[][][][] WorldMap;   //map of all the sectors
    private static GameObject[][] AbilityMap;  //map of all the ability planets contained in the sectors

    private static float timeElapsed = 0f;
    private static bool bigBangSpawned = false;

    // Start is called before the first frame update
    void Start()    //divide space up into several sectors, which are then broken up into smaller chunks which can each contain a max of 1 object
    {
        ModifiedAbilityPool = new List<AbilityTemplate>(AbilityPool);
        sectorSize = chunkSize * sectorDimensions;
        smallPlanetCollider = SmallPlanet.GetComponent<CircleCollider2D>();

        WorldMap = new GameObject[worldSize][][][];
        for (int x = 0; x < worldSize; x++)
        {
            WorldMap[x] = new GameObject[worldSize][][];
        }

        AbilityMap = new GameObject[worldSize][];
        for (int x = 0; x < worldSize; x++)
        {
            AbilityMap[x] = new GameObject[worldSize];
        }

        int centerIndex = worldSize / 2;

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                int sectorX = x - centerIndex;  //convert x and y into sector coordinates by basing them around the center of the universe; negative = left/below center, positive = right/above center
                int sectorY = y - centerIndex;
                int distanceX;  //calculate the distance away from world center we are at to figure out contents of sector; have to add 1 to positive numbers to keep balanced with negative
                int distanceY;
                if (sectorX >= 0)
                {
                    distanceX = sectorX + 1;
                } else
                {
                    distanceX = Mathf.Abs(sectorX);
                }

                if (sectorY >= 0)
                {
                    distanceY = sectorY + 1;
                }
                else
                {
                    distanceY = Mathf.Abs(sectorY);
                }

                int totalDistance = distanceX + distanceY;
                if (totalDistance <= 2) //starting sectors
                {
                    WorldMap[x][y] = BuildSector(10, 15, 7, 10, 0, 0, sectorX, sectorY, x, y, null);
                } else if (totalDistance <= 4)
                {
                    WorldMap[x][y] = BuildSector(10, 15, 7, 10, 0, 0, sectorX, sectorY, x, y, Level1Barrier);
                } else if (totalDistance <= 6)
                {
                    WorldMap[x][y] = BuildSector(10, 15, 7, 10, 0, 0, sectorX, sectorY, x, y, Level2Barrier);
                } else
                {
                    WorldMap[x][y] = BuildSector(10, 15, 7, 10, 0, 0, sectorX, sectorY, x, y, Level3Barrier);
                }
            }
        }
        SectorWall.UnlockSector.AddListener(ActivateSector);
        
    }

    GameObject[][] BuildSector(int MediumMin, int MediumMax, int LargeMin, int LargeMax, int MassiveMin, int MassiveMax, int sectorX, int sectorY, int arrayX, int arrayY, GameObject barrier)
    {
        GameObject[][] sectorMap = new GameObject[sectorDimensions][];
        for (int x = 0; x < sectorDimensions; x++)
        {
            sectorMap[x] = new GameObject[sectorDimensions];
        }

        AbilityMap[arrayX][arrayY] = SpawnAbilityPlanet(sectorMap, sectorX, sectorY);

        SpawnRandomPlanets(MediumMin, MediumMax, MediumPlanet, sectorMap, sectorX, sectorY);
        SpawnRandomPlanets(LargeMin, LargeMax, LargePlanet, sectorMap, sectorX, sectorY);
        SpawnRandomPlanets(MassiveMin, MassiveMax, MassivePlanet, sectorMap, sectorX, sectorY);

        for (int x = 0; x < sectorDimensions; x++)
        {
            for (int y = 0; y < sectorDimensions; y++)
            {
                if (sectorMap[x][y] == null)
                {
                    PlacePlanetInChunk(x, y, SmallPlanet, smallPlanetCollider, sectorMap, sectorX, sectorY);
                }
                if (barrier != null && sectorMap[x][y] != AbilityMap[arrayX][arrayY])
                {
                    sectorMap[x][y].SetActive(false);   //disable any object that spawns behind a barrier
                }
            }
        }

        if (barrier != null)
        {
            SpawnBarrier(barrier, sectorX, sectorY);
        }
        return sectorMap;
    }

    public void ActivateSector(int sectorX, int sectorY)    //used after a barrier gets destroyed to activate everything in the sector
    {
        int centerIndex = worldSize / 2;
        int arrayX = sectorX + centerIndex;
        int arrayY = sectorY + centerIndex;

        GameObject[][] sectorMap = WorldMap[arrayX][arrayY];
        for (int x = 0; x < sectorDimensions; x++)
        {
            for (int y = 0; y < sectorDimensions; y++)
            {
                GameObject chunkContents = sectorMap[x][y];
                chunkContents.SetActive(true);
            }
        }
    }

    GameObject SpawnAbilityPlanet(GameObject[][] sectorMap, int sectorX, int sectorY) //always place in the middle of a chunk in the middle of a sector
    {
        int randX = Random.Range(Mathf.RoundToInt((float)(sectorDimensions * 0.4)), Mathf.RoundToInt((float)(sectorDimensions * 0.7)));
        int randY = Random.Range(Mathf.RoundToInt((float)(sectorDimensions * 0.4)), Mathf.RoundToInt((float)(sectorDimensions * 0.7)));
        Vector3 planetCoords = new Vector3(randX * chunkSize + chunkSize/2 + (sectorSize * sectorX), randY * chunkSize + chunkSize/2 + (sectorSize * sectorY), 0);
        GameObject newPlanet = Instantiate(AbilityPlanet, planetCoords, Quaternion.identity);
        newPlanet.GetComponent<GrantAbility>().GrantedAbility = SelectAbility();
        sectorMap[randX][randY] = newPlanet;
        return newPlanet;
    }

    AbilityTemplate SelectAbility()
    {
        int abilityIndex = Random.Range(0, ModifiedAbilityPool.Count - 1);
        AbilityTemplate chosenAbility = ModifiedAbilityPool[abilityIndex];
        ModifiedAbilityPool.RemoveAt(abilityIndex);
        return chosenAbility;
    }

    void SpawnRandomPlanets(int min, int max, GameObject planetTemplate, GameObject[][] sectorMap, int sectorX, int sectorY)
    {
        int finalNumber = Random.Range(min, max);
        CircleCollider2D templateCollider = planetTemplate.GetComponent<CircleCollider2D>();
        for (int n = 0; n < finalNumber; n++)
        {
            bool planetPlaced = false;
            int attempts = 0;
            while (!planetPlaced) {
                int randX = Random.Range(0, sectorDimensions - 1);
                int randY = Random.Range(0, sectorDimensions - 1);
                if (sectorMap[randX][randY] == null)
                {
                    PlacePlanetInChunk(randX, randY, planetTemplate, templateCollider, sectorMap, sectorX, sectorY);
                    planetPlaced = true;
                } else {
                    attempts += 1;
                    if (attempts >= 3)
                    {
                        while (!planetPlaced)   //just increment randX and randY until we find an empty spot. Not a great first choice for RNG, but provides a guaranteed escape
                        {
                            randY += 1;
                            if (randY >= sectorDimensions)
                            {
                                randY = 0;
                                randX += 1;
                                if (randX >= sectorDimensions)
                                {
                                    randX = 0;
                                }
                            }
                            if (sectorMap[randX][randY] == null)
                            {
                                PlacePlanetInChunk(randX, randY, planetTemplate, templateCollider, sectorMap, sectorX, sectorY);
                                planetPlaced = true;
                            }
                        }
                    }
                }
            }
        }
    }

    void PlacePlanetInChunk(int chunkX, int chunkY, GameObject planetTemplate, CircleCollider2D templateCollider, GameObject[][] sectorMap, int sectorX, int sectorY)
    {
            float planetRadius = templateCollider.radius * planetTemplate.transform.localScale.x;
            float leftChunkLimit = (chunkX * chunkSize) + planetRadius;   //get coords for furthest left possible place we could put planet in chunk
            float rightChunkLimit = ((chunkX + 1f) * chunkSize) - planetRadius;
            float bottomChunkLimit = (chunkY * chunkSize) + planetRadius;
            float topChunkLimit = ((chunkY + 1f) * chunkSize) - planetRadius;
            float planetXCoords = Random.Range(leftChunkLimit, rightChunkLimit);
            float planetYCoords = Random.Range(bottomChunkLimit, topChunkLimit);
            Vector3 planetCoords = new Vector3(planetXCoords + (sectorSize*sectorX), planetYCoords + (sectorSize*sectorY), 0);
            GameObject newPlanet = Instantiate(planetTemplate, planetCoords, Quaternion.identity);
            newPlanet.GetComponent<SpriteRenderer>().color = PickRandomPlanetColor();
            sectorMap[chunkX][chunkY] = newPlanet;
    }

    void SpawnBarrier(GameObject barrier, int sectorX, int sectorY)
    {
        Vector3 barrierCoords = new Vector3(sectorSize / 2 + (sectorSize * sectorX), sectorSize / 2 + (sectorSize * sectorY), 0);
        GameObject newBarrier = Instantiate(barrier, barrierCoords, Quaternion.identity);
        newBarrier.GetComponent<SectorWall>().SetParameters(sectorX, sectorY);
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

    public static SectorCoordinates GetSectorAtCoords(float xCoords, float yCoords)  //used to convert x,y coords into sector values
    {
        int sectorX = (int) Mathf.Floor(xCoords / sectorSize) + (worldSize / 2);
        int sectorY = (int) Mathf.Floor(yCoords / sectorSize) + (worldSize / 2);
        return new SectorCoordinates(sectorX, sectorY);
    }

    public static List<GameObject> GetAbilitiesInAdjacentSectors(int sectorX, int sectorY)  //used by goliath radar to get all the ability planets in adjacent sectors
    {
        List<GameObject> returnList = new List<GameObject>();
        GameObject targetPlanet;
        if (sectorX > 0)
        {
            targetPlanet = AbilityMap[sectorX - 1][sectorY];
            if (targetPlanet != null)
            {
                returnList.Add(targetPlanet);
            } else
            {
                Debug.Log("sector " + sectorX + ", " + sectorY + " has nothing?");
            }
        }

        if (sectorX < (worldSize - 1))
        {
            targetPlanet = AbilityMap[sectorX + 1][sectorY];
            if (targetPlanet != null)
            {
                returnList.Add(targetPlanet);
            }
            else
            {
                Debug.Log("sector " + sectorX + ", " + sectorY + " has nothing?");
            }
        }

        if (sectorY > 0)
        {
            targetPlanet = AbilityMap[sectorX][sectorY - 1];
            if (targetPlanet != null)
            {
                returnList.Add(targetPlanet);
            }
            else
            {
                Debug.Log("sector " + sectorX + ", " + sectorY + " has nothing?");
            }
        }

        if (sectorY < (worldSize - 1))
        {
            targetPlanet = AbilityMap[sectorX][sectorY + 1];
            if (targetPlanet != null)
            {
                returnList.Add(targetPlanet);
            }
            else
            {
                Debug.Log("sector " + sectorX + ", " + sectorY + " has nothing?");
            }
        }

        return returnList;
    }

    public static float GetElapsedTime()
    {
        return timeElapsed;
    }
}
