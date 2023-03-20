using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePlanet : MonoBehaviour
{
    public GameObject Explosion;
    public Color blinkColor = Color.yellow;

    private float blinkTimer = 0.3f;
    private float timeSinceBlink = 0f;
    private float timeToExplode = 3f;
    private float tickingTime = 0f;
    private bool detonationStarted = false;
    private bool inBlink = false;
    private Color originalColor;
    private SpriteRenderer planetSprite;

    private Killable healthScript;

    // Start is called before the first frame update
    void Start()
    {
        healthScript = GetComponent<Killable>();
        planetSprite = GetComponent<SpriteRenderer>();
        originalColor = planetSprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!detonationStarted && healthScript.GetHealth() < healthScript.MaxHealth)
        {
            StartDetonation();
        }

        if (detonationStarted)
        {
            timeSinceBlink += Time.deltaTime;
            if (timeSinceBlink >= blinkTimer)
            {
                if (!inBlink)
                {
                    planetSprite.color = blinkColor;
                    inBlink = true;
                }
                else
                {
                    planetSprite.color = originalColor;
                    inBlink = false;
                }
                timeSinceBlink = 0f;
                blinkTimer -= 0.014f;
            }

            tickingTime += Time.deltaTime;
            if (tickingTime >= timeToExplode)
            {
                Explode();
            }
        }
    }

    void StartDetonation()
    {
        tickingTime = 0f;
        detonationStarted = true;
    }

    void Explode()
    {
        Debug.Log("exploding!");
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
