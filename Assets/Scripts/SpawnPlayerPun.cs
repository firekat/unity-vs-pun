using UnityEngine;
using System.Collections;

public class SpawnPlayerPun : MonoBehaviour
{
  public string PlayerPrefabName;

  void Awake()
  {
    PhotonNetwork.automaticallySyncScene = true;
  }

  void Start()
  {
    PhotonNetwork.Instantiate(PlayerPrefabName, Vector3.zero, Quaternion.identity, 0);
  }
}

