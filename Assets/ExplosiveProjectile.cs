using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile {     //like a normal projectile, but explodes when it despawns
    

    public GameObject Explosion; //explosion object to spawn

    protected override void ProjectileExpire()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonNetwork.IsConnected) PhotonNetwork.Instantiate(Explosion.name, transform.position, Quaternion.identity);
        else Instantiate(Explosion, transform.position, Quaternion.identity);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void TriggerExplosion()
    {
        Debug.LogError("triggering explosion!");
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonNetwork.IsConnected) PhotonNetwork.Instantiate(Explosion.name, transform.position, Quaternion.identity);
        else Instantiate(Explosion, transform.position, Quaternion.identity);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
