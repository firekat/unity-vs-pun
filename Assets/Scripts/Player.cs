using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
  public float MoveSpeed = 5f;

  [System.NonSerialized]
  public float horizAxis = 0f;

  [System.NonSerialized]
  public float vertAxis = 0f;

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
    if (networkView.isMine)
    {
      horizAxis = Input.GetAxis("Horizontal");
      vertAxis = Input.GetAxis("Vertical");
    }
  }

  public void Simulate()
  {
    transform.Translate(new Vector3(horizAxis, 0, vertAxis) * MoveSpeed * Time.fixedDeltaTime);
  }
}
