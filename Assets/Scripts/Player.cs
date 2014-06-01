using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
  public float MoveSpeed = 5f;
  void Start()
  {
    if (networkView == null || networkView.isMine)
    {
      gameObject.name = "Player - Me";
    }
    else
    {
      gameObject.name = "Player " + networkView.owner;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (networkView == null || networkView.isMine)
    {
      transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
    }
  }
}
