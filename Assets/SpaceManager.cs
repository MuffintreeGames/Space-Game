using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Runtime.CompilerServices;

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

public class SpaceManager : MonoBehaviourPunCallbacks   //script to generate space map
{

    public GameObject SmallPlanet;
    public GameObject MediumPlanet;
    public GameObject LargePlanet;
    public GameObject MassivePlanet;
    public GameObject AbilityPlanet;
    public GameObject Sun;
    public GameObject BlackHole;
    public GameObject ExplosivePlanet;
    public GameObject MonsterEgg;

    public GameObject SectorDivision;
    public GameObject Level1Barrier;
    public GameObject Level2Barrier;
    public GameObject Level3Barrier;
    public GameObject EdgeOfWorld;

    public GameObject BigBang;
    public float bigBangTime = 60f; //time at which big bang should begin

    public List<AbilityTemplate> AbilityPool;
    private List<AbilityTemplate> ModifiedAbilityPool;

    private static float chunkSize = 10f; //dimensions of 1 chunk
    private static int sectorDimensions = 10; //dimensions of 1 sector in terms of the number of chunks
    private static float sectorSize;
    private static int worldSize = 8; //number of sectors in x and y directions

    private static CircleCollider2D smallPlanetCollider;

    public static GameObject[][][][] WorldMap;   //map of all the sectors
    private static GameObject[][] AbilityMap;  //map of all the ability planets contained in the sectors

    
    private static bool bigBangSpawned = false;

    private void Awake()
    {
        Debug.Log("awake");
        if (PhotonNetwork.IsConnected)
        {
            RoleManager.isGoliath = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isGoliath"];
            //Debug.Log("setting isGoliath to " + RoleManager.isGoliath);
            Random.InitState((int)PhotonNetwork.LocalPlayer.CustomProperties["Seed"]);
        }
    }

    // Start is called before the first frame update
    void Start()    //divide space up into several sectors, which are then broken up into smaller chunks which can each contain a max of 1 object
    {
        TimeManager.ResetTime();
        bigBangSpawned = false;
        SectorWall.UnlockSector.AddListener(ActivateSector);
        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                WorldMap = new GameObject[worldSize][][][];
                for (int x = 0; x < worldSize; x++)
                {
                    WorldMap[x] = new GameObject[worldSize][][];
                    for (int y = 0; y < worldSize; y++)
                    {
                        WorldMap[x][y] = new GameObject[sectorDimensions][];
                        for (int z = 0; z < sectorDimensions; z++)
                        {
                            WorldMap[x][y][z] = new GameObject[sectorDimensions];
                        }
                    }
                }
                return;
            }
        }

        for (int x = 0; x < AbilityPool.Count; x++)
        {
            AbilityPool[x].ID = x;  //assign ID here so that we can easily sync up client end
        }
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
                }
                else
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
                    WorldMap[x][y] = BuildSector(10, 15, 7, 10, 0, 0, sectorX, sectorY, x, y, null, 0, 1, 0, 1, 0, 2, 0, 0);
                }
                else if (totalDistance <= 3)
                {
                    WorldMap[x][y] = BuildSector(15, 20, 10, 13, 1, 2, sectorX, sectorY, x, y, Level1Barrier, 0, 2, 0, 2, 0, 2, 0, 1);
                }
                else if (totalDistance <= 5)
                {
                    WorldMap[x][y] = BuildSector(20, 25, 13, 16, 2, 3, sectorX, sectorY, x, y, Level2Barrier, 1, 2, 1, 2, 1, 2, 0, 2);
                }
                else
                {
                    WorldMap[x][y] = BuildSector(25, 28, 16, 19, 3, 5, sectorX, sectorY, x, y, Level3Barrier, 1, 3, 1, 3, 1, 3, 1, 2);
                }
            }
        }
        
        //set up edge of world
        for (int y = -1; y < worldSize + 1; y++)
        {
            CreateEdgeTeleporter(-1 - centerIndex, y - centerIndex);
        }

        for (int y = -1; y < worldSize + 1; y++)
        {
            CreateEdgeTeleporter(centerIndex, y - centerIndex);
        }

        for (int x = 0; x < worldSize; x++)
        {
            CreateEdgeTeleporter(x - centerIndex, -1 - centerIndex);
        }

        for (int x = 0; x < worldSize; x++)
        {
            CreateEdgeTeleporter(x - centerIndex, centerIndex);
        }


        if (PhotonNetwork.IsConnected)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();

            string[][][][] stringMap = new string[worldSize][][][];
            for (int x = 0; x < worldSize; x++)
            {
                stringMap[x] = new string[worldSize][][];
                for (int y = 0; y < worldSize; y++)
                {
                    stringMap[x][y] = new string[sectorDimensions][];
                    for (int z = 0; z < sectorDimensions; z++)
                    {
                        stringMap[x][y][z] = new string[sectorDimensions];
                        for (int a = 0; a < sectorDimensions; a++)
                        {
                            stringMap[x][y][z][a] = WorldMap[x][y][z][a].name;
                        }
                    }
                }
            }

            roomProperties.Add("Map", stringMap);
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            Debug.Log("host sent message!");
            PhotonNetwork.Instantiate("FinishedLoadingObject", Vector3.zero, Quaternion.identity);  //once this is loaded, game starts
        }
    }

    void CreateEdgeTeleporter(int sectorX, int sectorY)
    {
        Vector3 teleCoords = new Vector3(sectorSize / 2 + (sectorSize * sectorX), sectorSize / 2 + (sectorSize * sectorY), 0);
        if (PhotonNetwork.IsConnected) { PhotonNetwork.Instantiate(EdgeOfWorld.name, teleCoords, Quaternion.identity); Debug.Log("photon check"); }
        else Instantiate(EdgeOfWorld, teleCoords, Quaternion.identity);
    }

    GameObject[][] BuildSector(int MediumMin, int MediumMax, int LargeMin, int LargeMax, int MassiveMin, int MassiveMax, int sectorX, int sectorY, int arrayX, int arrayY, GameObject barrier, int SunMin, int SunMax, int BlackMin, int BlackMax, int ExplodeMin, int ExplodeMax, int EggMin, int EggMax)
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
        SpawnRandomPlanets(SunMin, SunMax, Sun, sectorMap, sectorX, sectorY, true);
        SpawnRandomPlanets(BlackMin, BlackMax, BlackHole, sectorMap, sectorX, sectorY, true);
        SpawnRandomPlanets(ExplodeMin, ExplodeMax, ExplosivePlanet, sectorMap, sectorX, sectorY);
        SpawnRandomPlanets(EggMin, EggMax, MonsterEgg, sectorMap, sectorX, sectorY);

        for (int x = 0; x < sectorDimensions; x++)
        {
            for (int y = 0; y < sectorDimensions; y++)
            {
                if (sectorMap[x][y] == null)
                {
                    PlacePlanetInChunk(x, y, SmallPlanet, smallPlanetCollider, sectorMap, sectorX, sectorY);
                }
                if (barrier == null/* && sectorMap[x][y] != AbilityMap[arrayX][arrayY]*/)
                {
                    sectorMap[x][y].SetActive(true);   //enable any object not behind a barrier
                }
            }
        }

        if (barrier != null)
        {
            SpawnBarrier(barrier, sectorX, sectorY);
        }

        SpawnSectorDivider(sectorX, sectorY);

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
                
                if (chunkContents != null)
                {
                    chunkContents.SetActive(true);   //enable any object not behind a barrier
                }
            }
        }
    }

    GameObject SpawnAbilityPlanet(GameObject[][] sectorMap, int sectorX, int sectorY) //always place in the middle of a chunk in the middle of a sector
    {
        int randX = Random.Range(Mathf.RoundToInt((float)(sectorDimensions * 0.4)), Mathf.RoundToInt((float)(sectorDimensions * 0.7)));
        int randY = Random.Range(Mathf.RoundToInt((float)(sectorDimensions * 0.4)), Mathf.RoundToInt((float)(sectorDimensions * 0.7)));
        Vector3 planetCoords = new Vector3(randX * chunkSize + chunkSize/2 + (sectorSize * sectorX), randY * chunkSize + chunkSize/2 + (sectorSize * sectorY), 0);
        GameObject newPlanet;
        if (PhotonNetwork.IsConnected)
        {
            object[] instantiateParameters = new object[5];
            AbilityTemplate chosenAbility = SelectAbility();
            instantiateParameters[0] = chosenAbility.ID;
            instantiateParameters[1] = sectorX + (worldSize / 2);
            instantiateParameters[2] = sectorY + (worldSize / 2);
            instantiateParameters[3] = randX;
            instantiateParameters[4] = randY;
            newPlanet = PhotonNetwork.Instantiate(AbilityPlanet.name, planetCoords, Quaternion.identity, 0, instantiateParameters);
        }
        else
        {
            newPlanet = Instantiate(AbilityPlanet, planetCoords, Quaternion.identity);
            newPlanet.GetComponent<GrantAbility>().GrantedAbility = SelectAbility();
        }
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

    void SpawnRandomPlanets(int min, int max, GameObject planetTemplate, GameObject[][] sectorMap, int sectorX, int sectorY, bool special = false)
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
                    PlacePlanetInChunk(randX, randY, planetTemplate, templateCollider, sectorMap, sectorX, sectorY, special);
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
                                PlacePlanetInChunk(randX, randY, planetTemplate, templateCollider, sectorMap, sectorX, sectorY, special);
                                planetPlaced = true;
                            }
                        }
                    }
                }
            }
        }
    }
    void PlacePlanetInChunk(int chunkX, int chunkY, GameObject planetTemplate, CircleCollider2D templateCollider, GameObject[][] sectorMap, int sectorX, int sectorY, bool special = false)
    {
        float planetRadius = templateCollider.radius * planetTemplate.transform.localScale.x;
        float leftChunkLimit = (chunkX * chunkSize) + planetRadius;   //get coords for furthest left possible place we could put planet in chunk
        float rightChunkLimit = ((chunkX + 1f) * chunkSize) - planetRadius;
        float bottomChunkLimit = (chunkY * chunkSize) + planetRadius;
        float topChunkLimit = ((chunkY + 1f) * chunkSize) - planetRadius;
        float planetXCoords = Random.Range(leftChunkLimit, rightChunkLimit);
        float planetYCoords = Random.Range(bottomChunkLimit, topChunkLimit);
        Vector3 planetCoords = new Vector3(planetXCoords + (sectorSize * sectorX), planetYCoords + (sectorSize * sectorY), 0);
        GameObject newPlanet;
        int colorNumber = 0;
        Color newPlanetColor = PickPlanetColor(ref colorNumber); ;
        if (PhotonNetwork.IsConnected)
        {
            object[] instantiateParameters;
            if (!special)
            {
                instantiateParameters = new object[5];
            }
            else
            {
                instantiateParameters = new object[4];
            }
            instantiateParameters[0] = sectorX + (worldSize / 2);
            instantiateParameters[1] = sectorY + (worldSize / 2);
            instantiateParameters[2] = chunkX;
            instantiateParameters[3] = chunkY;
            if (!special)
            {
                instantiateParameters[4] = colorNumber;
            }
            newPlanet = PhotonNetwork.Instantiate(planetTemplate.name, planetCoords, Quaternion.identity, 0, instantiateParameters);
        }
        else newPlanet = Instantiate(planetTemplate, planetCoords, Quaternion.identity);
        if (!special)
        {
            newPlanet.GetComponent<SpriteRenderer>().color = newPlanetColor;
        }
        sectorMap[chunkX][chunkY] = newPlanet;
    }

    void SpawnBarrier(GameObject barrier, int sectorX, int sectorY)
    {
        Vector3 barrierCoords = new Vector3(sectorSize / 2 + (sectorSize * sectorX), sectorSize / 2 + (sectorSize * sectorY), -2f);
        GameObject newBarrier;
        if (PhotonNetwork.IsConnected) {
            object[] instantiationData = new object[2];
            instantiationData[0] = sectorX;
            instantiationData[1] = sectorY;
            newBarrier = PhotonNetwork.Instantiate(barrier.name, barrierCoords, Quaternion.identity, 0, instantiationData); 
        } else newBarrier = Instantiate(barrier, barrierCoords, Quaternion.identity);
        newBarrier.GetComponent<SectorWall>().SetParameters(sectorX, sectorY);
    }

    void SpawnSectorDivider(int sectorX, int sectorY)
    {
        Vector3 dividerCoords = new Vector3(sectorSize / 2 + (sectorSize * sectorX), sectorSize / 2 + (sectorSize * sectorY), 0);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(SectorDivision.name, dividerCoords, Quaternion.identity);
        }
        else Instantiate(SectorDivision, dividerCoords, Quaternion.identity);
    }

    Color PickPlanetColor(ref int colorNumber)
    {
        colorNumber = Random.Range(1, 6);
        switch (colorNumber) {
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
            case 4:
                return new Color(156, 0, 255, 255);
            case 5:
                return Color.cyan;
            case 6:
                return Color.magenta;
        }
        return Color.white;
    }

    public static Color GetColor(int colorNumber) //user provides number to get color
    {
        switch (colorNumber)
        {
            case 1:
                return Color.blue;
            case 2:
                return Color.green;
            case 3:
                return Color.yellow;
            case 4:
                return new Color(156, 0, 255, 255);
            case 5:
                return Color.cyan;
            case 6:
                return Color.magenta;
        }
        return Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if ((TimeManager.GetElapsedTime() >= bigBangTime) && !bigBangSpawned)
        {
            Debug.Log("spawning big bang");
            /*if (PhotonNetwork.IsConnected) PhotonNetwork.Instantiate(BigBang.name, new Vector3(0, 0, 0), Quaternion.identity);
            else */Instantiate(BigBang, new Vector3(0, 0, 0), Quaternion.identity);
            bigBangSpawned = true;
        }

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

    public static AbilityTemplate GetAbilityTemplateInSector(int sectorX, int sectorY)
    {
        GameObject abilityPlanet = AbilityMap[sectorX][sectorY];
        if (abilityPlanet != null)
        {
            return abilityPlanet.GetComponent<GrantAbility>().GrantedAbility;
        }
        return null;
    }

    /*void DeactivateWorld()  //disable everything outside of the initial sectors
    {
        int distanceToCenter = (worldSize / 2) - 1;
        for (int a = 0; a < worldSize; a++)
        {
            for (int b = 0; b < worldSize; b++)
            {
                if ((a == distanceToCenter || a == distanceToCenter - 1) && (b == distanceToCenter || b == distanceToCenter - 1))
                {
                    continue;
                }
                for (int c = 0; c < sectorDimensions; c++)
                {
                    for (int  d = 0; d < sectorDimensions; d++)
                    {
                        WorldMap[a][b][c][d].SetActive(false);
                    }
                }
            }
        }
    }*/

    void ActivateWorld()  //enable everything in initial sectors
    {
        int distanceToCenter = (worldSize / 2) - 1;
        for (int a = 0; a < worldSize; a++)
        {
            for (int b = 0; b < worldSize; b++)
            {
                if ((a == distanceToCenter || a == distanceToCenter - 1) && (b == distanceToCenter || b == distanceToCenter - 1))
                {
                    for (int c = 0; c < sectorDimensions; c++)
                    {
                        for (int d = 0; d < sectorDimensions; d++)
                        {

                            WorldMap[a][b][c][d].SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public static bool CoordsInStartingSector(int sectorX, int sectorY)
    {
        int distanceToCenter = (worldSize / 2) - 1;
        return ((sectorX == distanceToCenter || sectorX == distanceToCenter + 1) && (sectorY == distanceToCenter || sectorY == distanceToCenter + 1));
    }

    /*public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable propertiesThatChanged)    //for non-host player, need to receive world map and then perform any local changes that require it
    {
        Debug.Log("player update received!");
        if (targetPlayer != PhotonNetwork.LocalPlayer)
        {
            return;
        }
        if (propertiesThatChanged.ContainsKey("Map"))
        {
            string[][][][] stringMap = (string[][][][])propertiesThatChanged["Map"];
            for (int a = 0; a < worldSize; a++)
            {
                for (int b = 0; b < worldSize; b++)
                {
                    for (int c = 0; c < sectorDimensions; c++)
                    {
                        for (int d = 0; d < sectorDimensions; d++)
                        {
                            WorldMap[a][b][c][d] = GameObject.Find(stringMap[a][b][c][d]);
                        }
                    }
                }
            }
            ActivateWorld();
        }
    }*/

    /*public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)    //for non-host player, need to receive world map and then perform any local changes that require it
    {
        Debug.Log("room update received: " + propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey("Map"))
        {
            string[][][][] stringMap = (string[][][][])propertiesThatChanged["Map"];
            for (int a = 0; a < worldSize; a++)
            {
                for (int b = 0; b < worldSize; b++)
                {
                    for (int c = 0; c < sectorDimensions; c++)
                    {
                        for (int d = 0; d < sectorDimensions; d++)
                        {
                            Debug.Log("looking for " + stringMap[a][b][c][d]);
                            WorldMap[a][b][c][d] = GameObject.Find(stringMap[a][b][c][d]);
                        }
                    }
                }
            }
            ActivateWorld();
        }
    }*/
}
