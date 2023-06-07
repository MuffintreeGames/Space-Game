using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoliathRadar : MonoBehaviourPun
{
    public GameObject ArrowTemplate;

    private bool radarOn = false;

    private SectorCoordinates sectorCoords;
    private SectorCoordinates oldSectorCoords;

    private List<GameObject> radarArrows;
    private List<GameObject> nearbyAbilityPlanets;

    private float distanceFromCenter = 2f;

    private GameObject SectorAbilityOverlay;
    private Image SectorAbilityIcon;

    // Start is called before the first frame update
    void Start()
    {
        radarArrows = new List<GameObject>();
        nearbyAbilityPlanets = new List<GameObject>();
        oldSectorCoords = new SectorCoordinates(-100, -100);
        SectorAbilityOverlay = GameObject.Find("SectorAbility");
        SectorAbilityIcon = GameObject.Find("SectorAbilityIcon").GetComponent<Image>();
        SectorAbilityOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected && !RoleManager.isGoliath)
        {
            return;
        }

        sectorCoords = SpaceManager.GetSectorAtCoords(transform.position.x, transform.position.y);
        AbilityTemplate currentSectorAbility = SpaceManager.GetAbilityTemplateInSector(sectorCoords.x, sectorCoords.y);
        if (currentSectorAbility != null)
        {
            SectorAbilityOverlay.SetActive(true);
            SectorAbilityIcon.sprite = currentSectorAbility.icon;
        }
        else
        {
            SectorAbilityOverlay.SetActive(false);
        }

        if (Input.GetButtonDown("Radar"))
        {
            ToggleRadar();
        }

        if (radarOn)
        {
            if (oldSectorCoords.x != sectorCoords.x || oldSectorCoords.y != sectorCoords.y)
            {
                oldSectorCoords = sectorCoords;
                for (int n = 0; n < radarArrows.Count; n++)
                {
                    Destroy(radarArrows[n]);
                }
                radarArrows.Clear();
                Debug.Log("in a new sector, checking adjacent sectors for abilities");
                BuildArrowArray();
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

    void BuildArrowArray()
    {
        nearbyAbilityPlanets = SpaceManager.GetAbilitiesInAdjacentSectors(sectorCoords.x, sectorCoords.y);
        for (int n = 0; n < nearbyAbilityPlanets.Count; n++)
        {
            GameObject targetAbilityPlanet = nearbyAbilityPlanets[n];
            Debug.Log("a nearby sector has " + targetAbilityPlanet.GetComponent<GrantAbility>().GrantedAbility + " at coordinates " + targetAbilityPlanet.transform.position.x + ", " + targetAbilityPlanet.transform.position.y);
            Vector3 targetDirection = targetAbilityPlanet.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject instantiatedArrow;
            /*if (PhotonNetwork.IsConnected) instantiatedArrow = PhotonNetwork.Instantiate(ArrowTemplate.name, transform.position + (targetDirection.normalized * distanceFromCenter), targetRotation);
            else */
            instantiatedArrow = Instantiate(ArrowTemplate, transform.position + (targetDirection.normalized * distanceFromCenter), targetRotation);
            radarArrows.Add(instantiatedArrow);
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
            radarArrows.Clear();
        } else
        {
            radarOn = true;
            BuildArrowArray();
        }
    }
}
