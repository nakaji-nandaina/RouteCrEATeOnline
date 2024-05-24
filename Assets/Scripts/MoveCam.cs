using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    float sensitiveMove = 1.2f;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Goaled!=-1) return;
        if (Input.GetMouseButton(1))
        {
            float moveX = Input.GetAxis("Mouse X") * sensitiveMove;
            float moveY = Input.GetAxis("Mouse Y") * sensitiveMove;
            cam.transform.localPosition -= new Vector3(moveX,0, moveY);
        }
        float exY=Input.GetAxis("Mouse ScrollWheel")*10;
        cam.transform.position += cam.transform.forward * exY;
        float limy = 10 > cam.transform.position.y ? 10 : cam.transform.position.y;
        limy = 70 < limy ? 70 : limy;
        float limx = -60 > cam.transform.position.x ? -60 : cam.transform.position.x;
        limx = 60 < limx ? 60 : limx;
        float limz = -60 > cam.transform.position.z ? -60 : cam.transform.position.z;
        limz = 60 < limz ? 60 : limz;
        cam.transform.position = new Vector3(limx, limy, limz);
    }
    
}
