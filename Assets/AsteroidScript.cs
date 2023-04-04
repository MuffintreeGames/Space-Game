using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class AsteroidScript : MonoBehaviour
{
    private GameObject parentObject;
    public float asteroidSpeed = .02f;
    // Start is called before the first frame update
    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rotationAxis = new Vector3(0, 0, 1);
        //transform.RotateAround(parentObject.transform.position, rotationAxis, asteroidSpeed);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Quaternion q = Quaternion.AngleAxis(asteroidSpeed, rotationAxis);
        rb.MovePosition(q * (rb.transform.position - parentObject.transform.position) + parentObject.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        enabled = false;
    }
}
