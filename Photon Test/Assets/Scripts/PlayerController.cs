using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPun
{
    private const float NORMAL_MOVE_SPEED = 5f;
    private const float JUMP_FORCE = 500f;
    public float zMoveSpeed = 0;
    public float xMoveSpeed = 0;
    public int jumpCount = 1;

    private Vector3 playerxzPos;
    private Vector3 rayPos;
    private RaycastHit hit;
    private PlayerInput playerInput;
    private Animator playerAni;
    private Rigidbody playerRigidbody;
    private GameObject mainCam;
    private CapsuleCollider playerCollider;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAni = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        rayPos = new Vector3(0, 4f, 0) + mainCam.transform.forward.normalized * 0.2f;
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        playerxzPos = new Vector3(transform.position.x, 0, transform.position.z);
        Physics.Raycast(rayPos + playerxzPos, mainCam.transform.forward.normalized, out hit);
        Debug.Log(hit.collider.tag);
        if (hit.collider != null)
        {
            Debug.DrawRay(rayPos + playerxzPos, mainCam.transform.forward.normalized * hit.distance, Color.yellow);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //MoveZ();
        //MoveX();
        RotateByCam();

        playerAni.SetBool("IsWalk", false);

        if (playerAni.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            if (playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.92f)
            {
                CheckJumpEnd();
            }
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            playerAni.SetBool("IsWalk", true);
            //Move();
            MoveZ();
            MoveX();
            Rotate();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount > 0)
            {
                Jump();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                Change();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if(playerCollider.enabled == false)
            {
                Return();
            }
        }
    }

    private void MoveZ()
    {
        if (playerInput.moveZ < 0)
        {
            zMoveSpeed = NORMAL_MOVE_SPEED * 0.8f;
        }
        else
        {
            zMoveSpeed = NORMAL_MOVE_SPEED;
        }

        Vector3 moveDistance =
            playerInput.moveZ * transform.forward * zMoveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    private void MoveX()
    {
        xMoveSpeed = NORMAL_MOVE_SPEED;

        Vector3 moveDistance =
            playerInput.moveX * transform.right * xMoveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    private void CheckJumpEnd()
    {
        if (playerRigidbody.velocity.y < 0 && !playerAni.GetBool("IsGround"))
        {
            playerAni.SetBool("IsJump", false);
        }
    }

    private void Move()
    {
        Vector3 moveDistance =
            transform.forward * Time.deltaTime * NORMAL_MOVE_SPEED;
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    private void RotateByCam()
    {
        var offset = mainCam.transform.forward;
        offset.y = 0;
        transform.LookAt(transform.position + offset);
    }

    private void Rotate()
    {
        float rotateY;
        Vector3 turn;

        if (playerInput.moveZ > 0)
        {
            if (playerInput.moveX > 0)
            {
                rotateY = 45;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }

            if (playerInput.moveX == 0)
            {
                rotateY = 0;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }

            if (playerInput.moveX < 0)
            {
                rotateY = -45;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }
        }

        if (playerInput.moveZ == 0)
        {
            if (playerInput.moveX > 0)
            {
                rotateY = 90;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }

            if (playerInput.moveX == 0)
            {
                // pass
                return;
            }

            if (playerInput.moveX < 0)
            {
                rotateY = -90;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }
        }

        if (playerInput.moveZ < 0)
        {
            if (playerInput.moveX > 0)
            {
                rotateY = 135;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }

            if (playerInput.moveX == 0)
            {
                rotateY = 180;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }

            if (playerInput.moveX < 0)
            {
                rotateY = -135;
                turn = new Vector3(0, rotateY, 0);
                transform.eulerAngles += turn;
                return;
            }
        }
    }

    private void Jump()
    {
        jumpCount -= 1;
        playerRigidbody.AddForce(new Vector3(0, JUMP_FORCE, 0));
        playerAni.SetBool("IsJump", true);
    }

    private void Change()
    {
        photonView.RPC("ChangeOnServer", RpcTarget.MasterClient, hit.collider.gameObject, playerCollider, playerAni, gameObject);
    }


    private void Return()
    { 
        photonView.RPC("ReturnOnServer", RpcTarget.MasterClient, gameObject, playerCollider, playerAni);
    }

    [PunRPC]
    private void ChangeOnServer(GameObject tempObject, CapsuleCollider playerCollider, Animator playerAni, GameObject playerGameObject)
    {
        if (hit.collider.CompareTag("Changable"))
        {
            GameObject child = new GameObject();
            for (int i = 0; i < playerGameObject.transform.childCount; i++)
            {
                if (playerGameObject.transform.GetChild(i).gameObject.CompareTag("Changable"))
                {
                    playerCollider.enabled = true;
                    child = playerGameObject.transform.GetChild(i).gameObject;
                }
            }

            Instantiate(tempObject, playerGameObject.transform.position, playerGameObject.transform.rotation).transform.parent = playerGameObject.transform;
            Destroy(child);

            playerAni.enabled = false;
            //playerRigidbody.constraints = RigidbodyConstraints.None;

            for (int i = 0; i < transform.childCount - 1; i++)
            {
                if (playerGameObject.transform.GetChild(i).gameObject.activeSelf == true)
                {
                    playerGameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            playerCollider.enabled = false;
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ChangeOnServer", RpcTarget.Others, tempObject, playerCollider, playerAni, playerGameObject);
            }
        }
    }

    [PunRPC]
    private void ReturnOnServer(GameObject playerGameObject, CapsuleCollider playerCollider, Animator playerAni)
    {
        for (int i = 0; i < playerGameObject.transform.childCount; i++)
        {
            if (playerGameObject.transform.GetChild(i).gameObject.CompareTag("Changable"))
            {
                playerCollider.enabled = true;
                GameObject child = playerGameObject.transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        for (int i = 0; i < playerGameObject.transform.childCount; i++)
        {
            if (playerGameObject.transform.GetChild(i).gameObject.activeSelf == false)
            {
                playerGameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        playerAni.enabled = true;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ReturnOnServer", RpcTarget.Others, playerGameObject, playerCollider, playerAni);
        }
        //playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        //playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            playerAni.SetBool("IsGround", true);
            playerAni.SetBool("IsJump", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            jumpCount = 1;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            playerAni.SetBool("IsGround", false);
        }
    }
}