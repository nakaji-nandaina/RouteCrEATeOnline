using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MoveCam : MonoBehaviour
{
    float sensitiveMove = 1.2f;
    Camera cam;
    float prestarttime = 2;
    float movetime = 3;
    float movecount = 0;
    Vector3 startPos;Vector3 endPos;
    Vector3 startRotate; Vector3 endRotate;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }
    public void IniCam()
    {
        GameObject unitychan = GameManager.Instance.UnityChanObj;
        cam.transform.position = new Vector3(unitychan.transform.position.x - 10, unitychan.transform.position.y + 15, unitychan.transform.position.z - 10);
        cam.transform.rotation = Quaternion.Euler(50, 30, 0);
        GameManager.Instance.prestartTime = 2;
        movetime = 3;
        movecount = 0;
        startPos = cam.transform.position;
        startRotate = cam.transform.rotation.eulerAngles;
        endPos = new Vector3(0, 60, -10);
        endRotate = new Vector3(80, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Goaled!=-1) return;
        if (PhotonNetwork.PlayerList.Length < 2) return;
        if (GameManager.Instance.prestartTime > 0)
        {
            GameManager.Instance.prestartTime -= Time.deltaTime;
            return;
        }
        if (movecount < movetime)
        {
            movecount += Time.deltaTime;
            if (movecount > movetime) movecount = movetime;
            float p = movecount / movetime;
            this.gameObject.transform.position = startPos * (1 - p) + endPos * p;
            this.gameObject.transform.rotation = Quaternion.Euler( startRotate * (1 - p) + endRotate * p);
            return;
        }
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
