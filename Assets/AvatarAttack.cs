using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAttack : MonoBehaviour   //add to attack object to end avatar attack mode when it is destroyed
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        AvatarController.AvatarFinishAttack.Invoke(true);
    }
}
