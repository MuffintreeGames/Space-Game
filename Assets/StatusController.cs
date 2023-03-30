using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*struct StatusStruct
{
    public Sprite icon
}*/

public class StatusController : MonoBehaviour
{
    private float[] statusTimers;
    private float[] maxTimers;
    private Transform goliathTransform;
    private GameObject[] iconArray;
    private bool[] iconInUse;
    // Start is called before the first frame update
    void Start()
    {
        goliathTransform = GameObject.Find("Goliath").transform;
        iconArray = new GameObject[6];
        iconInUse = new bool[iconArray.Length];
        statusTimers = new float[iconArray.Length];
        maxTimers = new float[iconArray.Length];
        for (int i = 0; i < iconArray.Length; i++)
        {
            iconArray[i] = transform.GetChild(i).gameObject;
            iconInUse[i] = false;
            statusTimers[i] = 0f;
            maxTimers[i] = 0f;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one + Vector3.one * (goliathTransform.localScale.y-1)/2;
        transform.position = new Vector3(goliathTransform.position.x, goliathTransform.position.y + (1.1f * goliathTransform.localScale.y), goliathTransform.position.z);  //reposition to be directly over goliath

        for (int i = 0;i < iconArray.Length;i++)
        {
            if (iconInUse[i])
            {
                statusTimers[i] -= Time.deltaTime;
                if (statusTimers[i] < 0f)
                {
                    iconArray[i].GetComponent<Image>().enabled = false;
                    iconInUse[i] = false;
                    Transform iconOverlay = iconArray[i].transform.GetChild(0);
                    iconOverlay.localScale = new Vector3(iconOverlay.localScale.x, 0f, iconOverlay.localScale.z);
                } else
                {
                    Transform iconOverlay = iconArray[i].transform.GetChild(0);
                    float percentageLeft = statusTimers[i] / maxTimers[i];
                    iconOverlay.localScale = new Vector3(iconOverlay.localScale.x, 1 - percentageLeft, iconOverlay.localScale.z);
                }
            }
        }
    }

    public void AddStatus(Sprite icon, float duration)
    {
        for (int i = 0; i < iconArray.Length; i++) { 
            if (!iconInUse[i]) {
                iconInUse[i] = true;
                Image iconSprite = iconArray[i].GetComponent<Image>();
                iconSprite.sprite = icon;
                iconSprite.enabled = true;
                statusTimers[i] = duration;
                maxTimers[i] = duration;
                return;
            }
        }
        Debug.LogError("Too many status effects to display at once!");
    }
}
