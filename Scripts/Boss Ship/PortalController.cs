using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject[] aliens;
    public GameObject player;
    public int spawnInterval;
    public AudioSource[] sfx;
    public Light spawnGlow;
    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnInterval == counter)
        {
            SpawnAlien();
            counter = 0;
            spawnGlow.intensity = 0;
        }
        else
        {
            counter++;
            spawnGlow.intensity += 0.00417f;
        }            
    }

    private void OnTriggerEnter(Collider other)
    {
        sfx[1].Play();
        if (other.gameObject.tag == "pLaser")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);            
        }
        else if (other.gameObject.tag != "Portal" && other.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }

    private void SpawnAlien()
    {
        GameObject spawnAlien = aliens[(int)Random.Range(0, aliens.Length)];
        spawnAlien.GetComponent<EnemyScript>().player = player;
        Instantiate(spawnAlien, this.transform.position, this.transform.rotation);
        sfx[0].Play();
    }
}
