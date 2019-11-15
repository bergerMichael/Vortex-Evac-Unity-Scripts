using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public GameObject playerProjectile;    // reference to the single projectile
    public GameObject crosshair;
    public GameObject playerModel;
    public Camera mainCamera;
    public float maxProjectileForce;
    public AudioSource[] playerSFX;
    public AudioSource[] painSFX;

    private int invulnPeriod;
    private int invulnCounter;
    private bool isInvuln;
    private float currentForce;
    private bool isCharging;
    private float cameraDistance;
    private Color defaultLightColor;

    [SerializeField, Range(0f, 600f)]
    private float RotationSpeed = 600f;

    public GameObject crosshairLight;
    public GameObject chargeLight;

    private const float PlayerMaxHealth = 100f;
    private float CurrentPlayerHealth;
    public Image HealthBar;

    private GameObject shot;
    public Image AmmoCounter;

    public GameObject CullingPlane;

    void Start()
    {
        currentForce = 0;
        isCharging = false;
        cameraDistance = Vector3.Distance(playerModel.transform.position, mainCamera.transform.position);
        maxProjectileForce = 50;
        CurrentPlayerHealth = PlayerMaxHealth;
        HealthBar.fillAmount = 1f;
        shot = null;
        defaultLightColor = crosshairLight.GetComponent<Light>().color;
        invulnPeriod = 60;
        isInvuln = false;
        invulnCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        CheckHealth();

        var color = AmmoCounter.color;
        if (null == shot)
            color.a = 1f;
        else
            color.a = 0.1f;
        AmmoCounter.color = color;

        if (isInvuln && invulnCounter < invulnPeriod)
        {
            invulnCounter++;
        }
        if (isInvuln && invulnCounter >= invulnPeriod)
        {
            invulnCounter = 0;
            isInvuln = false;
        }
    }

    private void FixedUpdate()
    {
        transform.position.Set(transform.position.x, 1.81f, transform.position.z);      // hacky way to stop the player from sinking
    }

    private void LateUpdate()
    {
        if(transform.position.y <= CullingPlane.transform.position.y)
        {
            TakeDamage(1f);
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isCharging = true;
            crosshairLight.GetComponent<Light>().intensity = 5;
            playerSFX[1].Play();
        }

        if (isCharging && currentForce <= maxProjectileForce)    // increments current force by a set value each frame mouse0 is held down
        {
            currentForce += 0.84f;     // this value reaches a full charge in approximately 1 second
        }

        if (isCharging && currentForce >= maxProjectileForce)
        {
            chargeLight.GetComponent<Light>().color = Color.green;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            playerSFX[1].Stop();
            if (null == shot)
                FireWeapon(currentForce);
            currentForce = 0f; 
            isCharging = false;
            crosshairLight.GetComponent<Light>().intensity = 0;
            chargeLight.GetComponent<Light>().color = defaultLightColor;
        }
        /// Lightup the crosshair as force gets stronger
        chargeLight.GetComponent<Light>().intensity = currentForce * 1.5f;
        transform.LookAt(new Vector3(-crosshair.transform.position.x, transform.position.y, -crosshair.transform.position.z));
    }

    void CheckCameraMovement()
    {
        if (cameraDistance > Vector3.Distance(playerModel.transform.position, mainCamera.transform.position))
            mainCamera.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void FireWeapon(float forceScaler)
    {
        /// <summary>
        /// Reposition the PlayerProjectile to the barrel of the player model's weapon
        /// Find the direction vector from Player's current position to the target
        /// Calculate the force vector with direction and player input
        /// Apply force to the projectile in the derived direction
        /// Apply opposite force to player (this force will be some order of magnitue smaller than the projectile's)
        /// The projectile will need it's own behavior to facilitate returning to the player
        /// </summary>

        Vector3 playerPos = transform.position;
        Vector3 target = crosshair.transform.position;
        Vector3 forceVector = (target - playerPos).normalized * forceScaler;

        

        GameObject newProj = Instantiate(playerProjectile, (forceVector.normalized * 3), Quaternion.Euler(0, -transform.eulerAngles.y, transform.eulerAngles.z));

        newProj.GetComponent<ProjectileScript>().playerParent = this.gameObject;
        newProj.GetComponent<ProjectileScript>().player = playerModel;
        newProj.GetComponent<ProjectileScript>().crosshair = crosshair;
        newProj.GetComponent<ProjectileScript>().PropellProjectile(new Vector3(forceVector.x, 0f, forceVector.z), forceScaler);
        shot = newProj;
        this.GetComponent<Rigidbody>().AddForce((-0.25f) * (new Vector3(forceVector.x, 0f, forceVector.z)), ForceMode.Impulse);     // currently sufficient for moving the player. Acceleration/decelleration preferrable
        // Camera movement is disproportional to player since they are at different angles
        //mainCamera.GetComponent<Rigidbody>().AddForce(-(new Vector3(forceVector.x, 0f, forceVector.z)), ForceMode.Impulse);
        this.GetComponentInChildren<Animator>().Play("Shoot");

        playerSFX[0].Play();
    }
    public void TakeDamage(float damage)
    {
        if (isInvuln)
            return;
        CurrentPlayerHealth -= damage;
        HealthBar.fillAmount = CurrentPlayerHealth / PlayerMaxHealth;
        painSFX[(int)Random.Range(0, painSFX.Length)].Play();
    }
    private void CheckHealth()
    {
        if(CurrentPlayerHealth <= 0f)
        {
            Destroy(gameObject);
            GameObject.Find("---Game Manager---").GetComponent<MainMenu>().LoadLoseScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Laser")
        {
            if (isInvuln)
                return;
            StartCoroutine(Flash(1f, 0.05f));
            TakeDamage(20);
            isInvuln = true;
        }
    }

    public void OnHitByEnemy()  // enemies colliders are triggers, so this function must be called from the enemy object
    {
        if (isInvuln)
            return;
        StartCoroutine(Flash(1f, 0.05f));
        TakeDamage(3);
        isInvuln = true;
    }

    IEnumerator Flash(float time, float intervalTime)
    {
        float elapsedTime = 0f;
        int index = 0;
        while (elapsedTime < time)
        {
            if (GetComponent<Light>().intensity > 0)
                GetComponent<Light>().intensity = 0;
            else
                GetComponent<Light>().intensity = 2;

            elapsedTime += Time.deltaTime;
            index++;
            yield return new WaitForSeconds(intervalTime);
        }
    }

    public void PlayReload()
    {
        playerSFX[2].Play();
    }
}
