using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleShieldController : MonoBehaviour
{
    public Killable protectedObject;

    private SpriteRenderer shieldSprite;

    private float lifeTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        shieldSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime > 0f)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0f)
            {
                protectedObject.blockableHits = 0;
                shieldSprite.enabled = false;
                return;
            }

            int blockLeft = protectedObject.CheckBlock();
            if (blockLeft == 0)
            {
                shieldSprite.enabled = false;
                return;
            }

            shieldSprite.enabled = true;
        }
    }

    public void ActivateShield(float lifetime, int numHits)
    {
        this.lifeTime = lifetime;
        protectedObject.blockableHits = numHits;
    }
}
