using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public GoliathController playerGoliath;

    private Transform expBar;

    private float timeSinceExpChange = 0f;  //how much time has passed since player exp has changed
    private float timeToChangeExp = 0.2f;   //speed that exp bar should fill
    private float lastReadExpCount = 0f;    //last exp value gotten from player; used to check if it has changed
    private float oldExpScale = 0f;         //last value that exp bar stopped at
    private bool levelUpProcedure = false;  //currently putting normal exp bar behaviour on hold to level up properly
    // Start is called before the first frame update
    void Start()
    {
        expBar = GameObject.Find("ExpBarFillContainer").transform;
        if (!expBar)
        {
            Debug.LogError("Couldn't find exp bar!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateExpBar();
    }

    void UpdateExpBar()
    {
        float currentExp = playerGoliath.GetExp();
        float neededExp = playerGoliath.GetNeededExp();
        if (currentExp != lastReadExpCount && !levelUpProcedure) //goliath gained exp
        {
            timeSinceExpChange = 0f;
            oldExpScale = expBar.localScale.y;
            lastReadExpCount = currentExp;
            int playerLevel = playerGoliath.GetLevel();
            if (currentExp >= neededExp) //leveled up, want bar to reach cap before falling
            {
                levelUpProcedure = true;
            }
        }

        if (timeSinceExpChange < timeToChangeExp)
        {
            float expPercentage;
            if (!levelUpProcedure)
            {
                expPercentage = currentExp / neededExp;
            } else
            {
                expPercentage = 1;
            }
            float expScale = Mathf.Lerp(oldExpScale, expPercentage, timeSinceExpChange / timeToChangeExp);
            expBar.localScale = new Vector3(expBar.localScale.x, expScale, expBar.localScale.z);
            timeSinceExpChange += Time.deltaTime;
        } else if (levelUpProcedure)
        {
            playerGoliath.LevelUp();    //level up goliath from here to sync it up with the exp bar
            levelUpProcedure = false;
            expBar.localScale = new Vector3(expBar.localScale.x, 0f, expBar.localScale.z);
            oldExpScale = 0f;
            lastReadExpCount = 0f;
        }
    }
}
