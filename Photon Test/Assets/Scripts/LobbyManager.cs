using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text loadingText;
    public Button joinButton;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;

        loadingText.text = "Try to Connect Server...";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        loadingText.text = "Online: Connected to Server";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        loadingText.text = "Offline: Connection Failed\nRetry to Connect Server...";
    }

    public void Connect()
    {
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            loadingText.text = "Try to Connect Room...";

            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            loadingText.text = "Offline: Connection Failed\nRetry to Connect Server...";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        loadingText.text = "No Empty Room Exist, Making New Room...";

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        loadingText.text = "Join Room Succeed";

        PhotonNetwork.LoadLevel("MainScene");
    }
}
