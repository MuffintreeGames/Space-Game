using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public GoliathController playerGoliath;

    private Transform expBar;
    private Transform hpBar;
    private GameObject abilityBar;
    private Image abilityIcon1;
    private Text abilityTimer1;
    private Image abilityIcon2;
    private Text abilityTimer2;
    private Image abilityIcon3;
    private Text abilityTimer3;
    private Image abilityIcon4;
    private Text abilityTimer4;
    private Image newIcon;
    private GameObject abilitySelectionDisplay;


    private float timeSinceExpChange = 0f;  //how much time has passed since player exp has changed
    private float timeToChangeExp = 0.2f;   //speed that exp bar should fill
    private float lastReadExpCount = 0f;    //last exp value gotten from player; used to check if it has changed
    private float oldExpScale = 0f;         //last value that exp bar stopped at
    private bool levelUpProcedure = false;  //currently putting normal exp bar behaviour on hold to level up properly

    private float timeSinceHpChange = 0f;  
    private float timeToChangeHp = 0.2f;   
    private float lastReadHpCount = 0f;
    private float lastReadMaxHpCount = 0f;
    private float oldHpScale = 0f;         
    // Start is called before the first frame update
    void Start()
    {
        expBar = GameObject.Find("ExpBarFillContainer").transform;
        if (!expBar)
        {
            Debug.LogError("Couldn't find exp bar!");
        }

        hpBar = GameObject.Find("HpBarFillContainer").transform;
        if (!hpBar)
        {
            Debug.LogError("Couldn't find hp bar!");
        }

        abilityBar = GameObject.Find("AbilityBar");
        if (!abilityBar)
        {
            Debug.LogError("Couldn't find ability bar!");
        }

        abilityIcon1 = GameObject.Find("Ability1Icon").GetComponent<Image>();
        if (!abilityIcon1)
        {
            Debug.LogError("Couldn't find ability icon 1!");
        }

        abilityTimer1 = GameObject.Find("Ability1Timer").GetComponent<Text>();
        if (!abilityTimer1)
        {
            Debug.LogError("Couldn't find ability timer 1!");
        }

        abilityIcon2 = GameObject.Find("Ability2Icon").GetComponent<Image>();
        if (!abilityIcon2)
        {
            Debug.LogError("Couldn't find ability icon 2!");
        }

        abilityTimer2 = GameObject.Find("Ability2Timer").GetComponent<Text>();
        if (!abilityTimer2)
        {
            Debug.LogError("Couldn't find ability timer 2!");
        }

        abilityIcon3 = GameObject.Find("Ability3Icon").GetComponent<Image>();
        if (!abilityIcon3)
        {
            Debug.LogError("Couldn't find ability icon 3!");
        }

        abilityTimer3 = GameObject.Find("Ability3Timer").GetComponent<Text>();
        if (!abilityTimer3)
        {
            Debug.LogError("Couldn't find ability timer 3!");
        }

        abilityIcon4 = GameObject.Find("Ability4Icon").GetComponent<Image>();
        if (!abilityIcon4)
        {
            Debug.LogError("Couldn't find ability icon 4!");
        }

        abilityTimer4 = GameObject.Find("Ability4Timer").GetComponent<Text>();
        if (!abilityTimer4)
        {
            Debug.LogError("Couldn't find ability timer 4!");
        }

        newIcon = GameObject.Find("NewIcon").GetComponent<Image>();
        if (!newIcon)
        {
            Debug.LogError("Couldn't find new icon!");
        }

        abilitySelectionDisplay = GameObject.Find("AbilitySelection");
        if (!abilitySelectionDisplay)
        {
            Debug.LogError("Couldn't find ability display!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateExpBar();
        UpdateHpBar();
        UpdateAbilityBar();
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

    void UpdateHpBar()
    {
        float currentHp;
        float maxHp;

        if (playerGoliath != null)
        {
            currentHp = playerGoliath.GetHealth();
            maxHp = playerGoliath.GetMaxHealth();
        } else
        {
            currentHp = 0;
            maxHp = lastReadMaxHpCount;
        }

        if (currentHp != lastReadHpCount || maxHp != lastReadMaxHpCount) //goliath health changed
        {
            timeSinceHpChange = 0f;
            oldHpScale = hpBar.localScale.y;
            lastReadHpCount = currentHp;
            lastReadMaxHpCount = maxHp;
        }

        if (timeSinceHpChange < timeToChangeHp)
        {
            float hpPercentage = currentHp / maxHp;
            
            if (hpPercentage < 0)
            {
                hpPercentage = 0;
            }
            float hpScale = Mathf.Lerp(oldHpScale, hpPercentage, timeSinceHpChange / timeToChangeHp);
            hpBar.localScale = new Vector3(hpBar.localScale.x, hpScale, hpBar.localScale.z);
            timeSinceHpChange += Time.deltaTime;
        }
    }

    void UpdateAbilityBar() //set ability icons, gray them out if they're on cooldown and set timer
    {
        if (playerGoliath.Action1 != null)
        {
            abilityIcon1.enabled = true;
            abilityTimer1.enabled = true;
            abilityIcon1.sprite = playerGoliath.Action1.icon;

            if (!playerGoliath.Action1.IsOffCooldown())
            {
                abilityIcon1.color = Color.grey;
                abilityTimer1.text = playerGoliath.Action1.GetCooldown().ToString();
            }
            else
            {
                abilityIcon1.color = Color.white;
                abilityTimer1.text = "";
            }
        } else
        {
            abilityIcon1.enabled = false;
            abilityTimer1.enabled = false;
        }

        if (playerGoliath.Action2 != null)
        {
            abilityIcon2.enabled = true;
            abilityTimer2.enabled = true;
            abilityIcon2.sprite = playerGoliath.Action2.icon;
            if (!playerGoliath.Action2.IsOffCooldown())
            {
                abilityIcon2.color = Color.grey;
                abilityTimer2.text = playerGoliath.Action2.GetCooldown().ToString();
            }
            else
            {
                abilityIcon2.color = Color.white;
                abilityTimer2.text = "";
            }
        }
        else
        {
            abilityIcon2.enabled = false;
            abilityTimer2.enabled = false;
        }

        if (playerGoliath.Action3 != null)
        {
            abilityIcon3.enabled = true;
            abilityTimer3.enabled = true;
            abilityIcon3.sprite = playerGoliath.Action3.icon;
            if (!playerGoliath.Action3.IsOffCooldown())
            {
                abilityIcon3.color = Color.grey;
                abilityTimer3.text = playerGoliath.Action3.GetCooldown().ToString();
            }
            else
            {
                abilityIcon3.color = Color.white;
                abilityTimer3.text = "";
            }
        } else
        {
            abilityIcon3.enabled = false;
            abilityTimer3.enabled = false;
        }

        if (playerGoliath.Action4 != null)
        {
            abilityIcon4.enabled = true;
            abilityTimer4.enabled = true;
            abilityIcon4.sprite = playerGoliath.Action4.icon;
            if (!playerGoliath.Action4.IsOffCooldown())
            {
                abilityIcon4.color = Color.grey;
                abilityTimer4.text = playerGoliath.Action4.GetCooldown().ToString();
            }
            else
            {
                abilityIcon4.color = Color.white;
                abilityTimer4.text = "";
            }
        } else
        {
            abilityIcon4.enabled = false;
            abilityTimer4.enabled = false;
        }

        if (playerGoliath.InAbilitySelection()) {
            AbilityTemplate newAbility = playerGoliath.GetSelectionOption();
            if (newAbility == null)
            {
                Debug.LogError("goliath is in selection mode, but the ability is null!");
            } else
            {
                newIcon.sprite = newAbility.icon;
            }

            abilitySelectionDisplay.SetActive(true);
        } else
        {
            abilitySelectionDisplay.SetActive(false);
        }
    }
}
