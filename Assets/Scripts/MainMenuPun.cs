using UnityEngine;
using System.Collections;

public class MainMenuPun : UnityEngine.MonoBehaviour
{
  bool joined = false;

  void Start()
  {
    PhotonNetwork.ConnectUsingSettings("v1.0");
  }

  void OnJoinedLobby()
  {
    // we joined Photon and are ready to get a list of rooms
    joined = true;
  }

  void OnFailedToConnectToPhoton(DisconnectCause cause)
  {
    Debug.LogError(cause);
  }

  void OnGUI()
  {
    if (GUILayout.Button("CreateRoom") && joined)
    {
      if (PhotonNetwork.room == null) 
      {
        PhotonNetwork.CreateRoom("TestRoom", true, true, 8 );
      }
    }

    foreach(RoomInfo room in PhotonNetwork.GetRoomList())
    {
      if (GUILayout.Button(room.name) && joined)
      {
        PhotonNetwork.JoinRoom(room.name);
      }
    }

    if (GUILayout.Button("Leave") && joined && PhotonNetwork.room != null)
    {
      PhotonNetwork.LeaveRoom();
    }
  }

  // I think OnCreatedRoom is called when creating a room,
  // but OnJoinedRoom is called both when creating a room and joining a room??
//  void OnCreatedRoom()
//  {
//    PhotonNetwork.LoadLevel("GameScene");
//  }

  void OnJoinedRoom()
  {
    PhotonNetwork.LoadLevel("GameScene");
  }
}

