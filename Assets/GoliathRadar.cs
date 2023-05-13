using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoliathRadar : MonoBehaviourPun
{
    public GameObject ArrowTemplate;

    private bool radarOn = false;

    private SectorCoordinates sectorCoords;
    private SectorCoordinates oldSectorCoords;

    private List<GameObject> radarArrows;
    private List<GameObject> nearbyAbilityPlanets;

    private float distanceFromCenter = 2f;

    // Start is called before the first frame update
    void Start()
    {
        radarArrows = new List<GameObject>();
        nearbyAbilityPlanets = new List<GameObject>();
        oldSectorCoords = new SectorCoordinates(-100, -100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Radar"))
        {
            ToggleRadar();
        }

        if (radarOn)
        {
            sectorCoords = SpaceManager.GetSectorAtCoords(transform.position.x, transform.position.y);
            if (oldSectorCoords.x != sectorCoords.x || oldSectorCoords.y != sectorCoords.y)
            {
                oldSectorCoords = sectorCoords;
                for (int n = 0; n < radarArrows.Count; n++)
                {
                    Destroy(radarArrows[n]);
                }
                radarArrows.Clear();

                Debug.Log("in a new sector, checking adjacent sectors for abilities");
                nearbyAbilityPlanets = SpaceManager.GetAbilitiesInAdjacentSectors(sectorCoords.x, sectorCoords.y);
                for (int n = 0; n < nearbyAbilityPlanets.Count; n++)
                {
                    GameObject targetAbilityPlanet = nearbyAbilityPlanets[n];
                    Debug.Log("a nearby sector has " + targetAbilityPlanet.GetComponent<GrantAbility>().GrantedAbility + " at coordinates " + targetAbilityPlanet.transform.position.x + ", " + targetAbilityPlanet.transform.position.y);
                    Vector3 targetDirection = targetAbilityPlanet.transform.position - transform.position;
                    float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    radarArrows.Add(PhotonNetwork.Instantiate(ArrowTemplate.name, transform.position + (targetDirection.normalized * distanceFromCenter), targetRotation));
                }
            } else
            {
                for (int n = 0; n < nearbyAbilityPlanets.Count; n++) {
                    GameObject targetAbilityPlanet = nearbyAbilityPlanets[n];
                    if (targetAbilityPlanet != null)
                    {
                        Vector3 targetDirection = targetAbilityPlanet.transform.position - transform.position;
                        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                        radarArrows[n].transform.position = transform.position + (targetDirection.normalized * distanceFromCenter);
                        radarArrows[n].transform.localRotation = targetRotation;
                    }
                }
            }
        }
    }

    void ToggleRadar()
    {
        if (radarOn)
        {
            radarOn = false;
            for (int n = 0; n < radarArrows.Count; n++)
            {
                Destroy(radarArrows[n]);
            }
        } else
        {
            radarOn = true;
        }
    }
}
