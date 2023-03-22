using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleController : MonoBehaviour
{

    public LayerMask TargetLayers;

    private float suckRange = 30f;
    private float suckStrengthMin = 0.5f;
    private float suckStrengthRampUp = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        suckRange = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if ((TargetLayers & (1 << col.gameObject.layer)) != 0) {
            ApplySuck(col.gameObject);
        }
    }

    void ApplySuck(GameObject suckedObject)
    {
        Transform suckedTransform = suckedObject.transform;
        Vector3 distanceFromCenter = new Vector3(suckedTransform.position.x - transform.position.x, suckedTransform.position.y - transform.position.y, 0);
        float distanceFloat = Mathf.Sqrt((distanceFromCenter.x * distanceFromCenter.x) + (distanceFromCenter.y * distanceFromCenter.y));
        float angle = Mathf.Atan2(distanceFromCenter.y, distanceFromCenter.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        suckedTransform.position -= (distanceFromCenter.normalized * (suckStrengthMin + (suckStrengthRampUp * (1 - (distanceFloat / suckRange))))) * Time.deltaTime;
    }
}
