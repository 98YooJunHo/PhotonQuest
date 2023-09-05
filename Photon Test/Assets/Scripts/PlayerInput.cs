using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviourPun
{
    public string moveZAxisName = "Vertical";
    public string moveXAxisName = "Horizontal";

    public float moveZ { get; private set; }
    public float moveX { get; private set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        moveZ = Input.GetAxis(moveZAxisName);
        moveX = Input.GetAxis(moveXAxisName);
    }
}
