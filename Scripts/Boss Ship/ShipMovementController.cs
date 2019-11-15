using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovementController : MonoBehaviour
{
    [Range(0f, 100f)]
    public float radius;

    [Range(0f, 20f)]
    public float height;

    [Range(0f, 50f)]
    public float speed;

    [SerializeField, Range(0f, 360f)]
    private float AngleOfRotation;

    private float CircleRadius { get { return radius + Mathf.Max(EnvironmentBounds.extents.x, EnvironmentBounds.extents.z) + ShipBounds.extents.x; } }

    private GameObject Environment;
    private Bounds EnvironmentBounds;

    private Bounds ShipBounds;

    void Start()
    {
        Environment = GameObject.Find("---Environment---");
        if (null != Environment)
        {
            EnvironmentBounds = VisualBounds.GetMaxBounds(Environment);
        }
        else
            Debug.LogError("Environment Not Found");

        ShipBounds = VisualBounds.GetMaxBounds(gameObject);

        transform.position = new Vector3(CircleRadius, height, 0); 
    }
    void Update()
    {
        AngleOfRotation += Time.deltaTime * speed;

        if (AngleOfRotation >= 360)
            AngleOfRotation = 0;

        var x = CircleRadius * Mathf.Cos(Mathf.Deg2Rad * AngleOfRotation);
        var z = CircleRadius * Mathf.Sin(Mathf.Deg2Rad * AngleOfRotation);
        transform.LookAt(new Vector3(x, transform.position.y, z), Vector3.up);
        transform.position = new Vector3(x, height, z);
    }
}
