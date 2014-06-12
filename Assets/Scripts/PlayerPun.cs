using UnityEngine;
using System.Collections;

public class PlayerPun : Photon.MonoBehaviour
{
  public float MoveSpeed = 5f;

  [System.NonSerialized]
  public float horizAxis = 0f;

  [System.NonSerialized]
  public float vertAxis = 0f;

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
    if (photonView.isMine)
    {
      horizAxis = Input.GetAxis("Horizontal");
      vertAxis = Input.GetAxis("Vertical");
    }
  }

  public void Simulate()
  {
    // NOTE: this is called from a fixed update, hence fixedDeltaTime??
    transform.Translate(new Vector3(horizAxis, 0, vertAxis) * MoveSpeed * Time.fixedDeltaTime);
  }
}

