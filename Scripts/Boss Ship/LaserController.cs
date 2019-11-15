using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public GameObject Explosion;
    public AudioSource ExplosionAudio;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 800f);
        transform.Rotate(new Vector3(90f, 0, 0));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ("CapitalShip" != collision.gameObject.tag)
        {
            GameObject.FindGameObjectWithTag("Laser Explosion").GetComponent<AudioSource>().Play();
            Destroy(Instantiate(Explosion, transform.position, Quaternion.identity), 5f);            
            Destroy(gameObject);            
        }
    }
}
