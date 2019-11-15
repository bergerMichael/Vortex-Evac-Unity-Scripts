using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLaserController : MonoBehaviour
{
    public GameObject Portal;
    public GameObject Explosion;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * 3000f);
        transform.Rotate(new Vector3(90f, 0, 0));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ("Portal" != collision.gameObject.tag && "Player" != collision.gameObject.tag && "CapitalShip" != collision.gameObject.tag && "pLaser" != collision.gameObject.tag)
        {
            var portal = Instantiate(Portal, collision.gameObject.transform.position, Quaternion.identity);
            portal.tag = "Portal";
            var pBounds = VisualBounds.GetMaxBounds(portal);
            portal.transform.position += Vector3.up * pBounds.extents.y * 0.75f;
        }
        if ("CapitalShip" != collision.gameObject.tag)
        {
            Instantiate(Explosion, collision.gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if("Portal" == other.gameObject.tag)
        {
            Instantiate(Explosion, other.gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
