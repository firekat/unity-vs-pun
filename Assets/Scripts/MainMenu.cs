using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
  private string connectIP = "127.0.0.1";

  void OnGUI()
  {
    if (GUILayout.Button("Host"))
    {
      Network.InitializeServer(8, 25005, true);

      Application.LoadLevel("GameScene");
    }
    connectIP = GUILayout.TextField(connectIP);

    if (GUILayout.Button("Connect"))
    {
      Network.Connect(connectIP, 25005);
    }
  }

  void OnConnectedToServer()
  {
    Network.isMessageQueueRunning = false;

    Application.LoadLevel("GameScene");
  }
}
