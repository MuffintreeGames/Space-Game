using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class AvatarLaserBlockedEvent : UnityEvent<Vector2, Transform>
{

}

public class BlockableAvatarLaser : AttackObject
{
    public LayerMask avatarBlockingLayers;
    public static AvatarLaserBlockedEvent AvatarLaserBlocked;

    public GameObject testObject;
    bool skipFrame = false;
    float length = 0f;
    float speed = 100f;
    private void Awake()
    {
        if (AvatarLaserBlocked == null)
        {
            AvatarLaserBlocked = new AvatarLaserBlockedEvent();
        }
        //transform.parent.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Quaternion.AngleAxis(transform.parent.rotation.eulerAngles.z, Vector3.forward) * Vector3.up;
        //Debug.Log("line should end at " + transform.parent.position);
        //Debug.Log("direction vector " + end);
        //Debug.DrawLine(transform.parent.position, transform.parent.position + (end * 5000f), Color.blue);
        //Debug.DrawRay(transform.parent.position, transform.forward * 5000f, Color.blue);
        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, dir, length + (speed * Time.deltaTime), avatarBlockingLayers);
        //for (int x = 0; x < hit.Length; x++)
        //{
        if (hit.collider != null)
        {
            //if (avatarBlockingLayers == (avatarBlockingLayers | (1 << hit[x].collider.gameObject.layer)))
            //{
            Debug.LogError("raycast hit " + hit.collider.gameObject.name + " on layer " + LayerMask.LayerToName(hit.collider.gameObject.layer) + " at " + hit.point);
            //transform.parent.position 
            transform.parent.localScale = new Vector2(transform.parent.localScale.x, hit.distance);
            //break;
            //}
        }
        else
        {
            length += speed * Time.deltaTime;
            transform.parent.localScale = new Vector2(transform.parent.localScale.x, length);
        }
        /*else
        {
            Debug.Log("raycast failed to hit anything");
            transform.parent.localScale = new Vector2(transform.localScale.x, 5000f);
        }*/
    }
    
    /*new protected void OnTriggerStay2D(Collider2D collision)
    {
        GameObject hitGameObject = collision.gameObject;
        if ((avatarBlockingLayers & (1 << hitGameObject.layer)) != 0)
        {
            Debug.Log("hit something that should block laser!");
            skipFrame = true;
            Vector2 contactPoint = collision.ClosestPoint(transform.position);//collision.GetComponent<Rigidbody2D>().ClosestPoint(transform.position);
            AvatarLaserBlocked.Invoke(contactPoint, transform.parent);
        }
        base.OnTriggerStay2D(collision);
    }

    new protected void OnCollisionStay2D(Collision2D collision)
    {
        GameObject hitGameObject = collision.gameObject;
        if ((avatarBlockingLayers & (1 << hitGameObject.layer)) != 0)
        {
            Debug.Log("hit something that should block laser!");
            skipFrame = true;
            AvatarLaserBlocked.Invoke(collision.GetContact(0).point, transform.parent);
        }
        base.OnCollisionStay2D(collision);
    }*/
}
