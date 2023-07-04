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
        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        Debug.Log("raycast rotation: " + transform.parent.rotation.eulerAngles);
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.parent.position, transform.parent.rotation.eulerAngles, 5000f);
        Debug.DrawRay(transform.parent.position, transform.parent.rotation.eulerAngles * 5000f, Color.red);
        Debug.Log("hit " + hit.Length);
        for (int x = 0; x < hit.Length; x++) {
            //if (hit.Length > 0 && hit[0].collider != null)
            //{
            //if (avatarBlockingLayers == (avatarBlockingLayers | (1 << hit[x].collider.gameObject.layer)))
            //{
                Debug.LogError("raycast hit " + hit[x].collider.gameObject.name + " on layer " + LayerMask.LayerToName(hit[x].collider.gameObject.layer) + " at " + hit[x].point);
            //}
            //}
        }
        /*else
        {
            Debug.Log("raycast failed to hit anything");
            transform.parent.localScale = new Vector2(transform.localScale.x, 5000f);
        }*/
    }
    
    new protected void OnTriggerStay2D(Collider2D collision)
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
    }
}
