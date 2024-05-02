using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{

    public AudioSource hit;

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("hit " + collision.gameObject.tag);
        if (collision.gameObject.tag == "shootable") {
            hit.Play();
            GameObject.FindGameObjectWithTag("GameController").GetComponent<SpawnProjectiles>().projectileHit(collision.gameObject);
        }
    }
}
