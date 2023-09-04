using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    private const float NORMAL_MOVE_SPEED = 5f;
    private const float JUMP_FORCE = 500f;
    public float zMoveSpeed = 0;
    public float xMoveSpeed = 0;
    public int jumpCount = 1;

    private RaycastHit hit;
    private PlayerInput playerInput;
    private Animator playerAni;
    private Rigidbody playerRigidbody;
    private GameObject mainCam;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAni = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, transform.forward, out hit);
        Debug.Log(hit.collider.tag);
        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
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
            if(playerAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.92f)
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
        if(playerRigidbody.velocity.y < 0 && !playerAni.GetBool("IsGround"))
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
        playerAni.SetBool("IsGround", false);
        playerAni.SetBool("IsJump", true);
    }

    private void Change()
    {
        if(hit.collider.CompareTag("Changable"))
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).gameObject.CompareTag("Changable"))
                {
                    Destroy(transform.GetChild(i));
                }
            }

            GameObject tempObject = hit.collider.gameObject;
            Instantiate(tempObject, transform.position, transform.rotation).transform.parent = transform;

            playerAni.enabled = false;
            playerRigidbody.constraints = RigidbodyConstraints.None;

            for (int i = 0; i < transform.childCount - 1; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf == true)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    private void Return()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.CompareTag("Changable"))
            {
                Destroy(transform.GetChild(i));
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf == false)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        playerAni.enabled = true;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            playerAni.SetBool("IsGround", true);
            jumpCount = 1;
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Ground"))
    //    {
    //        playerAni.SetBool("IsGround", false);
    //    }
    //}
}