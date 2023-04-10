using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LaunchingKick : MonoBehaviour
{
    public GameObject legContainer;
    public GameObject leg;
    public float waitTime = 0.5f;
    public float swingTime = 0.1f;
    public Vector2 targetDirection = Vector2.zero;

    private float elapsedTime = 0f;
    private float angle;
    private bool swinging = false;
    //private AttackObject legAttack;

    // Start is called before the first frame update
    void Start()
    {
        angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        legContainer.transform.rotation = targetRotation;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (!swinging) {
            if (elapsedTime > waitTime)
            {
                swinging = true;
                leg.GetComponent<BoxCollider2D>().isTrigger = true;
                leg.GetComponent<AttackObject>().enabled = true;
                elapsedTime = 0f;
            }
        } else
        {
            if (elapsedTime > waitTime)
            {
                Destroy(gameObject);
            } else
            {
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f + (180f * elapsedTime / swingTime)));
                legContainer.transform.rotation = targetRotation;
            }
        }
    }
}
