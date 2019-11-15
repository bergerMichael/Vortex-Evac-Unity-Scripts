using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCombatController : MonoBehaviour
{
    [Range(500f, 1000f)]
    public float MaxHealth;
    private float CurrentHealth;

    public Image HealthBar;

    public GameObject Laser;
    public GameObject PortalLaser;
    public GameObject[] PortalSpawnPoints;
    public AudioSource[] sfx;

    private GameObject player;
    private Bounds PlayerBounds;

    private GameObject FiringPos;

    private const float MaxWeaponCooldown = 10f;

    private const float MaxPortalCooldown = 6f;
    private const float MinPortalCooldown = 3f;

    [SerializeField, Range(0f, 1f)]
    private float Accuracy;
    private const float AccuracyRange = 5f; 

    [SerializeField, Range(0f, MaxWeaponCooldown)]
    private float WeaponCooldown;

    [SerializeField, Range(0f, MaxWeaponCooldown)]
    private float PortalCooldown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerBounds = VisualBounds.GetMaxBounds(player);
        WeaponCooldown = Random.Range(0f, MaxWeaponCooldown);
        PortalCooldown = Random.Range(MinPortalCooldown, MaxPortalCooldown);
        FiringPos = GameObject.Find("CaptialShipFiringLocation");
        CurrentHealth = MaxHealth;
        HealthBar.fillAmount = 1;
    }
    void LateUpdate()
    {
        WeaponCheck();
        PortalCheck();
        HealthCheck();
    }

    void WeaponCheck()
    {
        WeaponCooldown -= Time.deltaTime;
        if (WeaponCooldown <= 0)
        {
            WeaponCooldown = Random.Range(0f, MaxWeaponCooldown);
            Accuracy = Random.Range(0f, 1f);
            var targetX = Random.Range(-AccuracyRange, AccuracyRange) * Accuracy + player.transform.position.x;
            var targetZ = Random.Range(-AccuracyRange, AccuracyRange) * Accuracy + player.transform.position.z;
            var target = new Vector3(targetX, player.transform.position.y + PlayerBounds.extents.y, targetZ);

            sfx[0].Play();
            var laser = Instantiate(Laser, FiringPos.transform.position, Quaternion.identity);
            laser.transform.LookAt(target, Vector3.up);
        }
    }
    void PortalCheck()
    {
        PortalCooldown -= Time.deltaTime;
        if (PortalCooldown <= 0)
        {
            PortalCooldown = Random.Range(MinPortalCooldown, MaxPortalCooldown);
            if (PortalSpawnPoints.Length > 0)
            {
                var target = PortalSpawnPoints[Random.Range(0, PortalSpawnPoints.Length)].transform.position;

                sfx[2].Play();
                var pLaser = Instantiate(PortalLaser, FiringPos.transform.position, Quaternion.identity);
                pLaser.transform.LookAt(target, Vector3.up);
            }
        }
    }
    void HealthCheck()
    {
        if (CurrentHealth <= 0)
        {
            GameObject.FindGameObjectWithTag("Ship Explosion").GetComponent<AudioSource>().Play();            
            Destroy(gameObject, 6f);
            GameObject.Find("---Game Manager---").GetComponent<MainMenu>().LoadWinScene();
        }
    }
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        HealthBar.fillAmount = CurrentHealth / MaxHealth;
    }
}
