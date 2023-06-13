using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Online;
using Photon.Pun;

public class UpdateGodAbilityHelpTextEvent : UnityEvent<string>
{

}

public class HUDManager : MonoBehaviourPun
{
    public GoliathController playerGoliath;
    public GodController playerGod;

    public static UpdateGodAbilityHelpTextEvent UpdateGodAbilityHelpText;

    private Transform expBar;
    private Transform hpBar;
    private Transform mpBar;

    private Image abilityIcon1;
    private Text abilityTimer1;
    private Text abilityName1;
    private Image abilityIcon2;
    private Text abilityTimer2;
    private Text abilityName2;
    private Image abilityIcon3;
    private Text abilityTimer3;
    private Text abilityName3;
    private Image abilityIcon4;
    private Text abilityTimer4;
    private Text abilityName4;

    private Image godAbilityIcon1;
    private Text godAbilityTimer1;
    private Text godAbilityName1;
    private Text godAbilityCost1;
    private Image godAbilityIcon2;
    private Text godAbilityTimer2;
    private Text godAbilityName2;
    private Text godAbilityCost2;
    private Image godAbilityIcon3;
    private Text godAbilityTimer3;
    private Text godAbilityName3;
    private Text godAbilityCost3;
    private Image godAbilityIcon4;
    private Text godAbilityTimer4;
    private Text godAbilityName4;
    private Text godAbilityCost4;
    private Image godAbilityIcon5;
    private Text godAbilityTimer5;
    private Text godAbilityName5;
    private Text godAbilityCost5;
    private Image godAbilityIcon6;
    private Text godAbilityTimer6;
    private Text godAbilityName6;
    private Text godAbilityCost6;
    private Image godAbilityIcon7;
    private Text godAbilityTimer7;
    private Text godAbilityName7;
    private Text godAbilityCost7;

    private Text godAbilityHelp;
    private GameObject godAbilityHelpBackground;

    private Image newIcon;
    private GameObject abilitySelectionDisplay;


    private float timeSinceExpChange = 0f;  //how much time has passed since player exp has changed
    private float timeToChangeExp = 0.2f;   //speed that exp bar should fill
    private float lastReadExpCount = 0f;    //last exp value gotten from player; used to check if it has changed
    private float oldExpScale = 0f;         //last value that exp bar stopped at
    private bool levelUpProcedure = false;  //currently putting normal exp bar behaviour on hold to level up properly
    private Text expMaxText;
    private Text expCurrentText;

    private float timeSinceHpChange = 0f;  
    private float timeToChangeHp = 0.2f;
    private float currentHp = 0f;
    private float lastReadHpCount = 0f;
    private float lastReadMaxHpCount = 0f;
    private float oldHpScale = 0f;
    private Text hpMaxText;
    private Text hpCurrentText;

    private Text mpText;
    private float timeSinceMpChange = 0f;
    private float timeToChangeMp = 0.2f;
    private float lastReadMpCount = 0f;
    private float mpCountBeforeSpending = 0f;   //mp value before it was spent on an ability
    private float oldMpScale = 1f;
    // Start is called before the first frame update

    private void Awake()
    {
        UpdateGodAbilityHelpText = new UpdateGodAbilityHelpTextEvent();
        expBar = GameObject.Find("ExpBarFillContainer").transform;
        if (!expBar)
        {
            Debug.LogError("Couldn't find exp bar!");
        }

        expMaxText = GameObject.Find("ExpMax").GetComponent<Text>();
        if (!expMaxText)
        {
            Debug.LogError("Couldn't find exp max!");
        }

        expCurrentText = GameObject.Find("ExpCurrent").GetComponent<Text>();
        if (!expCurrentText)
        {
            Debug.LogError("Couldn't find exp current!");
        }

        hpBar = GameObject.Find("HpBarFillContainer").transform;
        if (!hpBar)
        {
            Debug.LogError("Couldn't find hp bar!");
        }

        hpMaxText = GameObject.Find("HpMax").GetComponent<Text>();
        if (!hpMaxText)
        {
            Debug.LogError("Couldn't find hp max!");
        }

        hpCurrentText = GameObject.Find("HpCurrent").GetComponent<Text>();
        if (!hpCurrentText)
        {
            Debug.LogError("Couldn't find hp current!");
        }


        mpBar = GameObject.Find("MpBarFillContainer").transform;
        if (!mpBar)
        {
            Debug.LogError("Couldn't find mp bar!");
        }

        mpText = GameObject.Find("MpText").GetComponent<Text>();
        if (!mpText)
        {
            Debug.LogError("Couldn't find mp text!");
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

        abilityName1 = GameObject.Find("Ability1Name").GetComponent<Text>();
        if (!abilityName1)
        {
            Debug.LogError("Couldn't find ability name 1!");
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

        abilityName2 = GameObject.Find("Ability2Name").GetComponent<Text>();
        if (!abilityName2)
        {
            Debug.LogError("Couldn't find ability name 2!");
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

        abilityName3 = GameObject.Find("Ability3Name").GetComponent<Text>();
        if (!abilityName3)
        {
            Debug.LogError("Couldn't find ability name 3!");
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

        abilityName4 = GameObject.Find("Ability4Name").GetComponent<Text>();
        if (!abilityName4)
        {
            Debug.LogError("Couldn't find ability name 4!");
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

        godAbilityIcon1 = GameObject.Find("GodAbility1Icon").GetComponent<Image>();
        if (!godAbilityIcon1)
        {
            Debug.LogError("Couldn't find god ability icon 1!");
        }

        godAbilityTimer1 = GameObject.Find("GodAbility1Timer").GetComponent<Text>();
        if (!godAbilityTimer1)
        {
            Debug.LogError("Couldn't find god ability timer 1!");
        }

        godAbilityName1 = GameObject.Find("GodAbility1Name").GetComponent<Text>();
        if (!godAbilityName1)
        {
            Debug.LogError("Couldn't find god ability name 1!");
        }

        godAbilityCost1 = GameObject.Find("GodAbility1Cost").GetComponent<Text>();
        if (!godAbilityCost1)
        {
            Debug.LogError("Couldn't find god ability cost 1!");
        }

        godAbilityIcon2 = GameObject.Find("GodAbility2Icon").GetComponent<Image>();
        if (!godAbilityIcon2)
        {
            Debug.LogError("Couldn't find god ability icon 2!");
        }

        godAbilityTimer2 = GameObject.Find("GodAbility2Timer").GetComponent<Text>();
        if (!godAbilityTimer2)
        {
            Debug.LogError("Couldn't find god ability timer 2!");
        }

        godAbilityName2 = GameObject.Find("GodAbility2Name").GetComponent<Text>();
        if (!godAbilityName2)
        {
            Debug.LogError("Couldn't find god ability name 2!");
        }

        godAbilityCost2 = GameObject.Find("GodAbility2Cost").GetComponent<Text>();
        if (!godAbilityCost2)
        {
            Debug.LogError("Couldn't find god ability cost 2!");
        }

        godAbilityIcon3 = GameObject.Find("GodAbility3Icon").GetComponent<Image>();
        if (!godAbilityIcon3)
        {
            Debug.LogError("Couldn't find god ability icon 3!");
        }

        godAbilityTimer3 = GameObject.Find("GodAbility3Timer").GetComponent<Text>();
        if (!godAbilityTimer3)
        {
            Debug.LogError("Couldn't find god ability timer 3!");
        }

        godAbilityName3 = GameObject.Find("GodAbility3Name").GetComponent<Text>();
        if (!godAbilityName3)
        {
            Debug.LogError("Couldn't find god ability name 3!");
        }

        godAbilityCost3 = GameObject.Find("GodAbility3Cost").GetComponent<Text>();
        if (!godAbilityCost3)
        {
            Debug.LogError("Couldn't find god ability cost 3!");
        }

        godAbilityIcon4 = GameObject.Find("GodAbility4Icon").GetComponent<Image>();
        if (!godAbilityIcon4)
        {
            Debug.LogError("Couldn't find god ability icon 4!");
        }

        godAbilityTimer4 = GameObject.Find("GodAbility4Timer").GetComponent<Text>();
        if (!godAbilityTimer4)
        {
            Debug.LogError("Couldn't find god ability timer 4!");
        }

        godAbilityName4 = GameObject.Find("GodAbility4Name").GetComponent<Text>();
        if (!godAbilityName4)
        {
            Debug.LogError("Couldn't find god ability name 4!");
        }

        godAbilityCost4 = GameObject.Find("GodAbility4Cost").GetComponent<Text>();
        if (!godAbilityCost4)
        {
            Debug.LogError("Couldn't find god ability cost 4!");
        }

        godAbilityIcon5 = GameObject.Find("GodAbility5Icon").GetComponent<Image>();
        if (!godAbilityIcon5)
        {
            Debug.LogError("Couldn't find god ability icon 5!");
        }

        godAbilityTimer5 = GameObject.Find("GodAbility5Timer").GetComponent<Text>();
        if (!godAbilityTimer5)
        {
            Debug.LogError("Couldn't find god ability timer 5!");
        }

        godAbilityName5 = GameObject.Find("GodAbility5Name").GetComponent<Text>();
        if (!godAbilityName5)
        {
            Debug.LogError("Couldn't find god ability name 5!");
        }

        godAbilityCost5 = GameObject.Find("GodAbility5Cost").GetComponent<Text>();
        if (!godAbilityCost5)
        {
            Debug.LogError("Couldn't find god ability cost 5!");
        }

        godAbilityIcon6 = GameObject.Find("GodAbility6Icon").GetComponent<Image>();
        if (!godAbilityIcon6)
        {
            Debug.LogError("Couldn't find god ability icon 6!");
        }

        godAbilityTimer6 = GameObject.Find("GodAbility6Timer").GetComponent<Text>();
        if (!godAbilityTimer6)
        {
            Debug.LogError("Couldn't find god ability timer 6!");
        }

        godAbilityName6 = GameObject.Find("GodAbility6Name").GetComponent<Text>();
        if (!godAbilityName6)
        {
            Debug.LogError("Couldn't find god ability name 6!");
        }

        godAbilityCost6 = GameObject.Find("GodAbility6Cost").GetComponent<Text>();
        if (!godAbilityCost6)
        {
            Debug.LogError("Couldn't find god ability cost 6!");
        }

        godAbilityIcon7 = GameObject.Find("GodAbility7Icon").GetComponent<Image>();
        if (!godAbilityIcon7)
        {
            Debug.LogError("Couldn't find god ability icon 7!");
        }

        godAbilityTimer7 = GameObject.Find("GodAbility7Timer").GetComponent<Text>();
        if (!godAbilityTimer7)
        {
            Debug.LogError("Couldn't find god ability timer 7!");
        }

        godAbilityName7 = GameObject.Find("GodAbility7Name").GetComponent<Text>();
        if (!godAbilityName7)
        {
            Debug.LogError("Couldn't find god ability name 7!");
        }

        godAbilityCost7 = GameObject.Find("GodAbility7Cost").GetComponent<Text>();
        if (!godAbilityCost7)
        {
            Debug.LogError("Couldn't find god ability cost 7!");
        }

        godAbilityHelpBackground = GameObject.Find("GodAbilityHelp");
        if (!godAbilityHelpBackground)
        {
            Debug.LogError("Couldn't find god ability help background!");
        }

        godAbilityHelp = GameObject.Find("GodAbilityHelpText").GetComponent<Text>();
        if (!godAbilityHelp)
        {
            Debug.LogError("Couldn't find god ability help!");
        }

        UpdateGodAbilityHelpText.AddListener(SetGodHelpText);

        SetGodHelpText("");
    }
    void Start()
    {
        if (RoleManager.isGoliath)
        {
            GameObject.Find("MpBar").SetActive(false);
            GameObject.Find("GodAbilityBar").SetActive(false);
            GameObject.Find("FreeCameraButton").SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateExpBar();
        UpdateHpBar();
        UpdateMpBar();
        UpdateAbilityBar();
        UpdateGodAbilityBar();
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
            if (expScale < 0f)
            {
                expScale = 1f;
            }
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
        if (currentExp == -1)   //special value used to mark max level
        {
            expCurrentText.text = "Max";
            expMaxText.text = "Max";
        }
        else
        {
            expCurrentText.text = currentExp.ToString("F0");
            expMaxText.text = neededExp.ToString("F0");
        }
    }

    void UpdateHpBar()
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
        {
            float currentHp;

            if (playerGoliath != null)
            {
                currentHp = playerGoliath.GetHealth();
            }
            else
            {
                currentHp = 0;
            }
            gameObject.GetComponent<PhotonView>().RPC("ReceiveHpBarUpdate", RpcTarget.All, currentHp);
        }

        ScaleHpBar();
    }

    void ScaleHpBar()
    {
        float maxHp;
        if (playerGoliath != null)
        {
            maxHp = playerGoliath.GetMaxHealth();
        }
        else
        {
            maxHp = lastReadMaxHpCount;
        }

        if (currentHp != lastReadHpCount || maxHp != lastReadMaxHpCount) //goliath health changed
        {
            timeSinceHpChange = 0f;
            oldHpScale = hpBar.localScale.y;
            lastReadHpCount = currentHp;
            lastReadMaxHpCount = maxHp;
        }
        float hpPercentage = currentHp / maxHp;
        if (hpPercentage < 0)
        {
            hpPercentage = 0;
        }
        float hpScale;

        if (timeSinceHpChange < timeToChangeHp)
        {
            hpScale = Mathf.Lerp(oldHpScale, hpPercentage, timeSinceHpChange / timeToChangeHp);
            timeSinceHpChange += Time.deltaTime;
        }
        else
        {
            hpScale = hpPercentage;
        }

        hpBar.localScale = new Vector3(hpBar.localScale.x, hpScale, hpBar.localScale.z);
        hpCurrentText.text = currentHp.ToString("F0");
        hpMaxText.text = maxHp.ToString("F0");
    }

    [PunRPC]
    public void ReceiveHpBarUpdate(float providedCurrentHp) //used so that master client can inform non-master client of what hp should be
    {
        currentHp = providedCurrentHp;
    }

    void UpdateMpBar()
    {
        if (PhotonNetwork.IsConnected && RoleManager.isGoliath)
        {
            return;
        }

        float currentMp;
        float maxMp;

        if (playerGod != null)
        {
            currentMp = playerGod.GetMP();
            maxMp = playerGod.GetMaxMP();
        }
        else
        {
            currentMp = 0;
            maxMp = 1;
        }

        if (currentMp < lastReadMpCount) //god spent mp on ability
        {
            timeSinceMpChange = 0f;
            oldMpScale = mpBar.localScale.y;
            mpCountBeforeSpending = lastReadMpCount;
        }

        lastReadMpCount = currentMp;

        float mpPercentage = currentMp / maxMp;

        if (mpPercentage < 0)
        {
            mpPercentage = 0;
        }

        float mpScale;
        if (timeSinceMpChange < timeToChangeMp)
        {
            mpScale = Mathf.Lerp(oldMpScale, mpPercentage, timeSinceMpChange / timeToChangeMp);
            timeSinceMpChange += Time.deltaTime;
        } else
        {
            mpScale = mpPercentage;
        }
        mpBar.localScale = new Vector3(mpBar.localScale.x, mpScale, mpBar.localScale.z);
        mpText.text = currentMp.ToString("F0") + "/" + maxMp.ToString("F0");
    }

    void UpdateAbilityBar() //set ability icons, gray them out if they're on cooldown and set timer
    {
        if (playerGoliath.Action1 != null)
        {
            abilityIcon1.enabled = true;
            abilityTimer1.enabled = true;
            abilityIcon1.sprite = playerGoliath.Action1.icon;
            abilityName1.enabled = true;
            abilityName1.text = playerGoliath.Action1.displayName;

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
            abilityName1.enabled = false;
        }

        if (playerGoliath.Action2 != null)
        {
            abilityIcon2.enabled = true;
            abilityTimer2.enabled = true;
            abilityIcon2.sprite = playerGoliath.Action2.icon;
            abilityName2.enabled = true;
            abilityName2.text = playerGoliath.Action2.displayName;

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
            abilityName2.enabled = false;
        }

        if (playerGoliath.Action3 != null)
        {
            abilityIcon3.enabled = true;
            abilityTimer3.enabled = true;
            abilityIcon3.sprite = playerGoliath.Action3.icon;
            abilityName3.enabled = true;
            abilityName3.text = playerGoliath.Action3.displayName;

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
            abilityName3.enabled = false;
        }

        if (playerGoliath.Action4 != null)
        {
            abilityIcon4.enabled = true;
            abilityTimer4.enabled = true;
            abilityIcon4.sprite = playerGoliath.Action4.icon;
            abilityName4.enabled = true;
            abilityName4.text = playerGoliath.Action4.displayName;

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
            abilityName4.enabled = false;
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

    void UpdateGodAbilityBar()
    {
        if (PhotonNetwork.IsConnected && RoleManager.isGoliath)
        {
            return;
        }

        if (playerGod.Action1 != null)
        {
            godAbilityIcon1.enabled = true;
            godAbilityTimer1.enabled = true;
            godAbilityIcon1.sprite = playerGod.Action1.icon;
            godAbilityCost1.text = playerGod.Action1.manaCost.ToString();
            godAbilityCost1.enabled = true;
            godAbilityName1.enabled = true;
            godAbilityName1.text = playerGod.Action1.displayName;

            if (!playerGod.CanUseAbility1())
            {
                godAbilityIcon1.color = Color.grey;
                if (!playerGod.Action1.IsOffCooldown())
                {
                    godAbilityTimer1.text = playerGod.Action1.GetCooldown().ToString("F0");
                } else
                {
                    godAbilityTimer1.text = "";
                }
                
            }
            else
            {
                godAbilityIcon1.color = Color.white;
                godAbilityTimer1.text = "";
            }
        }
        else
        {
            godAbilityIcon1.enabled = false;
            godAbilityTimer1.enabled = false;
            godAbilityCost1.enabled = false;
            godAbilityName1.enabled = false;
        }

        if (playerGod.Action2 != null)
        {
            godAbilityIcon2.enabled = true;
            godAbilityTimer2.enabled = true;
            godAbilityIcon2.sprite = playerGod.Action2.icon;
            godAbilityCost2.text = playerGod.Action2.manaCost.ToString("F0");
            godAbilityCost2.enabled = true;
            godAbilityName2.enabled = true;
            godAbilityName2.text = playerGod.Action2.displayName;

            if (!playerGod.CanUseAbility2())
            {
                godAbilityIcon2.color = Color.grey;
                if (!playerGod.Action2.IsOffCooldown())
                {
                    godAbilityTimer2.text = playerGod.Action2.GetCooldown().ToString("F0");
                }
                else
                {
                    godAbilityTimer2.text = "";
                }
            }
            else
            {
                godAbilityIcon2.color = Color.white;
                godAbilityTimer2.text = "";
            }
        }
        else
        {
            godAbilityIcon2.enabled = false;
            godAbilityTimer2.enabled = false;
            godAbilityCost2.enabled = false;
            godAbilityName2.enabled = false;
        }

        if (playerGod.Action3 != null)
        {
            godAbilityIcon3.enabled = true;
            godAbilityTimer3.enabled = true;
            godAbilityIcon3.sprite = playerGod.Action3.icon;
            godAbilityCost3.text = playerGod.Action3.manaCost.ToString();
            godAbilityCost3.enabled = true;
            godAbilityName3.enabled = true;
            godAbilityName3.text = playerGod.Action3.displayName;

            if (!playerGod.CanUseAbility3())
            {
                godAbilityIcon3.color = Color.grey;
                if (playerGod.Action3UnlockTime > TimeManager.GetElapsedTime())
                {
                    godAbilityTimer3.color = Color.blue;
                    godAbilityTimer3.text = (playerGod.Action3UnlockTime - TimeManager.GetElapsedTime()).ToString("F0");
                }
                else if (!playerGod.Action3.IsOffCooldown())
                {
                    godAbilityTimer3.color = Color.red;
                    godAbilityTimer3.text = playerGod.Action3.GetCooldown().ToString("F0");
                }
                else
                {
                    godAbilityTimer3.text = "";
                }
            }
            else
            {
                godAbilityIcon3.color = Color.white;
                godAbilityTimer3.text = "";
            }
        }
        else
        {
            godAbilityIcon3.enabled = false;
            godAbilityTimer3.enabled = false;
            godAbilityCost3.enabled = false;
            godAbilityName3.enabled = false;
        }

        if (playerGod.Action4 != null)
        {
            godAbilityIcon4.enabled = true;
            godAbilityTimer4.enabled = true;
            godAbilityIcon4.sprite = playerGod.Action4.icon;
            godAbilityCost4.text = playerGod.Action4.manaCost.ToString();
            godAbilityCost4.enabled = true;
            godAbilityName4.enabled = true;
            godAbilityName4.text = playerGod.Action4.displayName;

            if (!playerGod.CanUseAbility4())
            {
                godAbilityIcon4.color = Color.grey;
                if (playerGod.Action4UnlockTime > TimeManager.GetElapsedTime())
                {
                    godAbilityTimer4.color = Color.blue;
                    godAbilityTimer4.text = (playerGod.Action4UnlockTime - TimeManager.GetElapsedTime()).ToString("F0");
                }
                else if (!playerGod.Action4.IsOffCooldown())
                {
                    godAbilityTimer4.color = Color.red;
                    godAbilityTimer4.text = playerGod.Action4.GetCooldown().ToString("F0");
                }
                else
                {
                    godAbilityTimer4.text = "";
                }
            }
            else
            {
                godAbilityIcon4.color = Color.white;
                godAbilityTimer4.text = "";
            }
        }
        else
        {
            godAbilityIcon4.enabled = false;
            godAbilityTimer4.enabled = false;
            godAbilityCost4.enabled = false;
            godAbilityName4.enabled = false;
        }

        if (playerGod.Action5 != null)
        {
            godAbilityIcon5.enabled = true;
            godAbilityTimer5.enabled = true;
            godAbilityIcon5.sprite = playerGod.Action5.icon;
            godAbilityCost5.text = playerGod.Action5.manaCost.ToString();
            godAbilityCost5.enabled = true;
            godAbilityName5.enabled = true;
            godAbilityName5.text = playerGod.Action5.displayName;

            if (!playerGod.CanUseAbility5())
            {
                godAbilityIcon5.color = Color.grey;
                if (playerGod.Action5UnlockTime > TimeManager.GetElapsedTime())
                {
                    godAbilityTimer5.color = Color.blue;
                    godAbilityTimer5.text = (playerGod.Action5UnlockTime - TimeManager.GetElapsedTime()).ToString("F0");
                }
                else if (!playerGod.Action5.IsOffCooldown())
                {
                    godAbilityTimer5.color = Color.red;
                    godAbilityTimer5.text = playerGod.Action5.GetCooldown().ToString("F0");
                }
                else
                {
                    godAbilityTimer5.text = "";
                }
            }
            else
            {
                godAbilityIcon5.color = Color.white;
                godAbilityTimer5.text = "";
            }
        }
        else
        {
            godAbilityIcon5.enabled = false;
            godAbilityTimer5.enabled = false;
            godAbilityCost5.enabled = false;
            godAbilityName5.enabled = false;
        }

        if (playerGod.Action6 != null)
        {
            godAbilityIcon6.enabled = true;
            godAbilityTimer6.enabled = true;
            godAbilityIcon6.sprite = playerGod.Action6.icon;
            godAbilityCost6.text = playerGod.Action6.manaCost.ToString();
            godAbilityCost6.enabled = true;
            godAbilityName6.enabled = true;
            godAbilityName6.text = playerGod.Action6.displayName;

            if (!playerGod.Action6.IsOffCooldown())
            {
                godAbilityIcon6.color = Color.grey;
                godAbilityTimer6.text = playerGod.Action6.GetCooldown().ToString();
            }
            else
            {
                godAbilityIcon6.color = Color.white;
                godAbilityTimer6.text = "";
            }
        }
        else
        {
            godAbilityIcon6.enabled = false;
            godAbilityTimer6.enabled = false;
            godAbilityCost6.enabled = false;
            godAbilityName6.enabled = false;
        }

        if (playerGod.Action7 != null)
        {
            godAbilityIcon7.enabled = true;
            godAbilityTimer7.enabled = true;
            godAbilityIcon7.sprite = playerGod.Action7.icon;
            godAbilityCost7.text = playerGod.Action7.manaCost.ToString();
            godAbilityCost7.enabled = true;
            godAbilityName7.enabled = true;
            godAbilityName7.text = playerGod.Action7.displayName;

            if (!playerGod.Action7.IsOffCooldown())
            {
                godAbilityIcon7.color = Color.grey;
                godAbilityTimer7.text = playerGod.Action7.GetCooldown().ToString();
            }
            else
            {
                godAbilityIcon7.color = Color.white;
                godAbilityTimer7.text = "";
            }
        }
        else
        {
            godAbilityIcon7.enabled = false;
            godAbilityTimer7.enabled = false;
            godAbilityCost7.enabled = false;
            godAbilityName7.enabled = false;
        }
    }

    void SetGodHelpText(string text)
    {
        Debug.Log("changing help text to " + text);
        godAbilityHelp.text = text;

        if (text == "")
        {
            godAbilityHelpBackground.SetActive(false);
        } else
        {
            godAbilityHelpBackground.SetActive(true);
        }
    }
}
