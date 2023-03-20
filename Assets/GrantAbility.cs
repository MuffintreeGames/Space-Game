using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityGainEvent : UnityEvent<AbilityTemplate>
{

}

public class GrantAbility : MonoBehaviour
{
    public static AbilityGainEvent GoliathGainAbility;

    public AbilityTemplate GrantedAbility;

    private SpriteRenderer childSprite;
    private bool spriteSet = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GoliathGainAbility == null)
        {
            GoliathGainAbility = new AbilityGainEvent();
        }

        GameObject childImage = transform.GetChild(0).gameObject;
        if (childImage)
        {
            childSprite = childImage.GetComponent<SpriteRenderer>();
            Debug.Log(childSprite);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GrantedAbility != null);
        //Debug.Log(childSprite);
        if (childSprite && GrantedAbility != null && !spriteSet)
        {
            Debug.Log("updating sprite!");
            childSprite.sprite = GrantedAbility.icon;
            spriteSet = true;
        }
    }

    public void KilledByGoliath()
    {
        GoliathGainAbility.Invoke(GrantedAbility);
    }
}
