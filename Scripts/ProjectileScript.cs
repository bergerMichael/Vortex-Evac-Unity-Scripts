using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private bool isActive;  // tracks if the projectile has been fired.
    private Vector3 returnForce;
    private Vector3 initialDir;
    private float returnForceScalar;
    public GameObject player;
    public GameObject playerParent;
    public GameObject crosshair;
    public AudioSource[] sfx;
    private float forceTime;
    private float elapsedTime;
    private float emergencyRetrieval;
    private float superEmergencyRetrieval;

    private new Rigidbody rigidbody;

    public GameObject explosion;

    private bool hitCapitalShip;

    // Start is called before the first frame update
    void Start()
    {
        returnForceScalar = 50f;
        forceTime = 0;
        elapsedTime = 0;
        emergencyRetrieval = 180;
        superEmergencyRetrieval = 360;

        rigidbody = GetComponent<Rigidbody>();
        hitCapitalShip = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitCapitalShip)
        {
            if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("CapitalShip").transform.position) <= 20f)
            {
                transform.LookAt(GameObject.FindGameObjectWithTag("CapitalShip").transform);
                rigidbody.AddForce(transform.forward * 40, ForceMode.Impulse);
                hitCapitalShip = true;
            }
        }
        transform.LookAt(rigidbody.velocity + transform.position);
        transform.Rotate(Vector3.right, 90f);
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            ReturnToPlayer();
        }
    }

    void ReturnToPlayer()
    {
        if (elapsedTime == superEmergencyRetrieval)
        {
            playerParent.GetComponent<PlayerScript>().PlayReload();
            DeactiviateProjectile();
        }
        if (elapsedTime == emergencyRetrieval)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().AddForce(returnForce, ForceMode.Impulse);
            returnForce = (new Vector3(player.transform.position.x, player.transform.position.y + 2f, player.transform.position.z)
                - new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z))
                .normalized * returnForceScalar;
        }
        if (elapsedTime >= forceTime && elapsedTime < emergencyRetrieval)
        {
            this.GetComponent<Rigidbody>().AddForce(returnForce, ForceMode.Acceleration);
            returnForce = (new Vector3(player.transform.position.x, player.transform.position.y + 2f, player.transform.position.z)
                - new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z))
                .normalized * returnForceScalar;
        }
        elapsedTime++;
    }

    public void PropellProjectile(Vector3 force, float time)
    {
        /// Add an impulse force to the projectile
        /// Initiate the countdown before the projectile starts it's return mission
        forceTime = time * 5;

        returnForce = (new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z)            
            - new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z))
            .normalized * returnForceScalar;
        returnForce.y = 0f;

        this.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        initialDir = force;
        var dir = initialDir;
        dir.x = 90;

        transform.Rotate(dir);

        ActivateProjectile();
    }

    public void CollectProjectile()
    {
        playerParent.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerParent.GetComponent<PlayerScript>().PlayReload();
        DeactiviateProjectile();
    }

    public void ActivateProjectile()
    {
        isActive = true;
        this.transform.eulerAngles.Set(player.transform.eulerAngles.x, player.transform.eulerAngles.y, 90);        
        /// find the direction player is facing and start the projectile offset from the player in that direction
        Vector3 playerPos = player.transform.position;
        Vector3 target = crosshair.transform.position;
        Vector3 direction = (target - playerPos).normalized * 3;
        direction.y = 0;
        playerPos.y = 3.229f;

        this.transform.position = playerPos + direction;
    }

    private void DeactiviateProjectile()
    {        
        Destroy(this.gameObject, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if("CapitalShip" == other.gameObject.tag)
        {
            sfx[0].Play();
            other.gameObject.GetComponentInParent<ShipCombatController>().TakeDamage(30f);
            Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 5f);
            transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
            rigidbody.velocity = Vector3.zero;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ("CapitalShip" == other.gameObject.tag)
        {
            sfx[0].Play();
            other.gameObject.GetComponentInParent<ShipCombatController>().TakeDamage(30f);
            Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 5f);
        }
    }
}
