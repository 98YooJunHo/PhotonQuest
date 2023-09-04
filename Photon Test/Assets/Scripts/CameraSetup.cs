using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class CameraSetup : MonoBehaviourPun
{
    public float xMove = 0;
    public float sensitivity = 1;
    private float yDistance = -3;
    private float zDistance = 10;
    private GameObject playerCam;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            playerCam = GameObject.FindWithTag("MainCamera");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        xMove += Input.GetAxis("Mouse X");
        playerCam.transform.rotation = Quaternion.Euler(0, xMove * sensitivity, 0);
        Vector3 reverseDistance = new Vector3 (0, yDistance, zDistance);

        playerCam.transform.position = transform.position - playerCam.transform.rotation * reverseDistance;
    }
}
