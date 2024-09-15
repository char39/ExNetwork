using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";
    public Text connectionInfoText;
    public Button joinButton;

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        joinButton.interactable = false;
        connectionInfoText.text = "Connect to Master Server...";
    }

    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "Online : Connected to Master Server";
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = "Offline : Disconnected to Master Server";
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "빈 방이 없습니다. 새로운 방을 생성합니다.";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 });
    }
    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "방 접속 성공";
        PhotonNetwork.LoadLevel("Main");
    }

    public void Connect()
    {
        joinButton.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "방을 찾고 있습니다...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "Offline : Disconnected to Master Server";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
