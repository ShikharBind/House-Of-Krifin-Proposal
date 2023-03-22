using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Ludo.Core;
using Ludo.Utilities;

public class MultiplayerController : MonoBehaviourPunCallbacks
{
    // [SerializeField] private string VersionName = "0.1";
    public static string joinCode = null;
    public static int maxPlayers = -1, entryFee = -1;
    public static bool is2v2 = false, withFriend = false;
    [SerializeField] private GameObject UsernameMenu, ConnectPanel, loadingPanel, playerPrefab;
    [SerializeField] private Button startGame;
    [SerializeField] private TextMeshProUGUI playerN, joinCodText;
    private bool isJoined = false, isGameStarted = false;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isJoined)
        {
            playerN.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                startGame.interactable = true;
            }
            else
            {
                startGame.interactable = false;
            }

            if (withFriend) loadingPanel.SetActive(false);
        }

        if (PhotonNetwork.InRoom && !isGameStarted)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers && !withFriend)
            {
                StartGame();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        Debug.Log("Connected Master");
    }



    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        StartJoining();
        // JoinRoom("abc");
    }


    private void StartJoining()
    {
        if (is2v2)
        {
            GameManager.is2v2 = true;
        }
        if (joinCode == null)
        {
            if (!withFriend)
            {
                JoinRoom();
            }
            else if (withFriend)
            {
                CreateRoom();
            }
        }
        else
        {
            JoinRoom(joinCode);
            joinCode = null;
        }
    }

    private RoomOptions GetRoomOptions(int maxPlayers)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayers;
        return roomOptions;
    }

    public void CreateRoom()
    {
        string roomName = Ludo.Utilities.Utilities.CreateRandomName();
        PhotonNetwork.CreateRoom(roomName, GetRoomOptions(maxPlayers));
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        // PhotonNetwork.JoinOrCreateRoom("abc", GetRoomOptions(), TypedLobby.Default);
    }

    public void JoinRoom()
    {
        Hashtable roomProperties
            = new Hashtable() { { "fee", entryFee } };
        RoomOptions opt = GetRoomOptions(maxPlayers);
        PhotonNetwork.JoinRandomOrCreateRoom(null, (byte)maxPlayers, MatchmakingMode.FillRoom, null, null, null, opt, null);
    }

    public static void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
            
        GameManager.isMultiplayer = false;
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room Joined");
        SpawnPlayer();
        startGame.interactable = true;
        isJoined = true;
        // PhotonNetwork.LoadLevel(1);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(1);
            isGameStarted = true;
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        joinCodText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Room Created");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Room Left");
    }

    private void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public static void Disconnect()
    {
        GameManager.isMultiplayer = false;
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        print("Disconnected from server for reason " + cause.ToString());
    }


}
