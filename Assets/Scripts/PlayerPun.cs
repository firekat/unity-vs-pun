using UnityEngine;
using System.Collections;

public class PlayerPun : Photon.MonoBehaviour
{
  public float MoveSpeed = 5f;
  void Start()
  {
    if (photonView == null || photonView.isMine)
    {
      gameObject.name = "PhotonPlayer - Me";
    }
    else
    {
      gameObject.name = "PhotonPlayer " + photonView.owner;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (photonView == null || photonView.isMine)
    {
      transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
    }
  }
}

