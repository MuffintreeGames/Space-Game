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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (childSprite && GrantedAbility != null && !spriteSet)
        {
            childSprite.sprite = GrantedAbility.icon;
            spriteSet = true;
        }
    }

    public void KilledByGoliath()
    {
        GoliathGainAbility.Invoke(GrantedAbility);
    }
}
