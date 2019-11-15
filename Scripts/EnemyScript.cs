using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public GameObject bloodExplosion;
    public AudioSource[] deathRattles;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
            GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        CheckDistanceToPlayer();
    }

    private void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= 2f)
            GetComponent<Animator>().SetBool("IsAttacking", true);
        else
            GetComponent<Animator>().SetBool("IsAttacking", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile" || other.gameObject.tag == "Laser")
        {
            Instantiate(bloodExplosion, transform.position, Quaternion.identity);
            bloodExplosion.transform.parent = transform.parent;
            InitiateDeath();
        }
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerScript>().OnHitByEnemy();
        }
    }

    private void InitiateDeath()
    {
        isDead = true;
        deathRattles[(int)Random.Range(0, deathRattles.Length)].Play();
        GetComponent<NavMeshAgent>().isStopped = true;
        Destroy(GetComponent<CapsuleCollider>());
        GetComponent<Animator>().SetBool("IsDead", true);
        Destroy(this.gameObject, 2);
    }
}
