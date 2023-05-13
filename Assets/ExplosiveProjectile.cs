using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile {     //like a normal projectile, but explodes when it despawns
    

    public GameObject Explosion; //explosion object to spawn

    protected override void ProjectileExpire()
    {
        PhotonNetwork.Instantiate(Explosion.ToString(), transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
