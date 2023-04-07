using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotAimingArrow : MonoBehaviour
{
    private GodController god;
    //private Vector2 startingPos;    //position of arrow at creation
    public float launchForce = 10f;   //force applied to object
    public CircleCollider2D aimedObject = null;
    public GodAbilityTemplate parentAbility;

    private bool appliedScale = false;
    // Start is called before the first frame update
    void Start()
    {
        //startingPos = transform.position;
        god = GameObject.Find("GodController").GetComponent<GodController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!aimedObject)
        {
            Destroy(gameObject);
        }

        if (!appliedScale)
        {
            appliedScale = true;
            transform.localScale = new Vector2(transform.localScale.x * aimedObject.transform.localScale.x, transform.localScale.y * aimedObject.transform.localScale.y)/1.5f;
        }

        Vector3 mouseCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetDirection = mouseCoords - aimedObject.transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        float distanceFromCenter = aimedObject.radius * aimedObject.transform.localScale.x;
        transform.position = aimedObject.transform.position + (targetDirection.normalized * distanceFromCenter);
        transform.localRotation = targetRotation;

        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody2D aimedRigid = aimedObject.gameObject.GetComponent<Rigidbody2D>();
            aimedRigid.AddForce(targetDirection * launchForce, ForceMode2D.Impulse);
            SpeedAttackObject aimedAttack = aimedObject.GetComponent<SpeedAttackObject>();
            aimedAttack.enabled = true;
            god.SpendMP(parentAbility.manaCost);
            god.SetAbilityUsage(true);
            Destroy(gameObject);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            god.SetAbilityUsage(true);
            Destroy(gameObject);
        }
    }
}
