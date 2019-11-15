using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    public GameObject TargetingLight;
    private new Camera camera;
    void Start()
    {
        Cursor.visible = false;
        camera = Camera.main;
    }
    private void Update()
    {
        if (Cursor.visible)
            Cursor.visible = false;
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 600f))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(hit.collider.gameObject.transform.up, Vector3.up);
        }
        else
        {
            transform.position = camera.ScreenPointToRay(Input.mousePosition).GetPoint(20f);
        }
    }
}
