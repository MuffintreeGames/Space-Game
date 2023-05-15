using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostilePlanetController : MonoBehaviourPun
{
    public GameObject PlanetShot;
    public float AttackSpeed = 10f;
    public float AttackDuration = 5f;

    private float attackFrequency = 2f;
    private float timeSinceAttack = 0f;
    private bool attackReady = true;

    private float attackRange = 30f;
    private float distanceFromOrigin = 3f;

    private Transform goliathTransform;

    // Start is called before the first frame update
    void Start()
    {
        goliathTransform = GameObject.Find("Goliath").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (goliathTransform == null)
        {
            return;
        }

        if (!attackReady)
        {
            timeSinceAttack += Time.deltaTime;
            if (timeSinceAttack >= attackFrequency)
            {
                attackReady = true;
            }
        }

        float distanceFromGoliath = Vector3.Distance(goliathTransform.position, transform.position);
        if (distanceFromGoliath <= attackRange && attackReady)
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        Vector3 targetDirection = goliathTransform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0,0,angle));
        GameObject firedShot;
        if (PhotonNetwork.IsConnected) firedShot = PhotonNetwork.Instantiate(PlanetShot.name, transform.position + (targetDirection.normalized * distanceFromOrigin), Quaternion.identity);
        else firedShot = PhotonNetwork.Instantiate(PlanetShot.name, transform.position + (targetDirection.normalized * distanceFromOrigin), Quaternion.identity);
        firedShot.GetComponent<Projectile>().SetProjectileParameters(AttackSpeed, targetRotation.eulerAngles.z, AttackDuration);
        attackReady = false;
        timeSinceAttack = 0f;
    }
}
