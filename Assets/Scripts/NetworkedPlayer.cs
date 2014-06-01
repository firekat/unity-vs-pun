using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour
{

  public float lerpFactor = 10f;
  private Vector3 lastReceievedPosition;

  void Start()
  {
    lastReceievedPosition = transform.position;
  }

  void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
  {
    Vector3 position = Vector3.zero;
    if (stream.isWriting)
    {
      position = transform.position;
      stream.Serialize(ref position);
    }
    else if (stream.isReading)
    {
      stream.Serialize(ref position);
      // Debug.Log("Okay, reading network object: " + position);
      // transform.position = position;
      lastReceievedPosition = position;
    }
  }

  void Update()
  {
    if (!networkView.isMine)
    {
      transform.position = Vector3.Lerp(transform.position, lastReceievedPosition, Time.deltaTime * lerpFactor);
    }
  }
}
