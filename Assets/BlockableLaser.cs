using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserBlockedEvent : UnityEvent<Vector2>
{

}

public class BlockableLaser : AttackObject
{
    public LayerMask blockingLayers;
    public static LaserBlockedEvent LaserBlocked;
    private void Awake()
    {
        if (LaserBlocked == null)
        {
            LaserBlocked = new LaserBlockedEvent();
        }
        transform.parent.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    new protected void OnTriggerStay2D(Collider2D collision)
    {
        GameObject hitGameObject = collision.gameObject;
        if ((blockingLayers & (1 << hitGameObject.layer)) != 0)
        {
            Debug.Log("hit something that should block laser!");
            Vector2 contactPoint = collision.GetComponent<Rigidbody2D>().ClosestPoint(transform.position);
            LaserBlocked.Invoke(contactPoint);
        }
        base.OnTriggerStay2D(collision);
    }

    new protected void OnCollisionStay2D(Collision2D collision)
    {
        GameObject hitGameObject = collision.gameObject;
        if ((blockingLayers & (1 << hitGameObject.layer)) != 0)
        {
            Debug.Log("hit something that should block laser!");
            LaserBlocked.Invoke(collision.GetContact(0).point);
        }
        base.OnCollisionStay2D(collision);
    }
}
