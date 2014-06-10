using UnityEngine;
using System.Collections;

public class PhotonNetworkedPlayer : Photon.MonoBehaviour
{
  public float lerpFactor = 10f;

  public float interpolationBackTime = 0.1f;

  // private Vector3 lastReceievedPosition;
  private NetworkState[] stateBuffer = new NetworkState[20];
  private int stateCount;

  //  void Start()
  //  {
  //    lastReceievedPosition = transform.position;
  //  }

  void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    Vector3 position = Vector3.zero;
    if (stream.isWriting)
    {
      position = transform.position;
      stream.SendNext(position);
    }
    else if (stream.isReading)
    {
      position = (Vector3) stream.ReceiveNext();
      // Debug.Log("Okay, reading network object: " + position);
      // transform.position = position;
      //lastReceievedPosition = position;
      SaveBuffer(new NetworkState(position, info.timestamp));
    }
  }

  void Update()
  {
    if (photonView.isMine || stateCount == 0)
    {
      return;
    }

    double currentTime = PhotonNetwork.time;
    double interpolationTime = currentTime - interpolationBackTime;

    if (stateBuffer[0].timestamp > interpolationTime)
    {
      for (int i = 0; i < stateCount; i++)
      {
        if (stateBuffer[i].timestamp <= interpolationTime || i == stateCount - 1)
        {
          NetworkState lhs = stateBuffer[i];

          NetworkState rhs = stateBuffer[Mathf.Max(i - 1, 0)];

          double length = rhs.timestamp - lhs.timestamp;
          float t = 0f;
          if (length > 0.0001)
          {
            t = (float)((interpolationTime - lhs.timestamp) / length);
          }

          transform.position = Vector3.Lerp(lhs.position, rhs.position, t);
          break;
        }
      }
    }
  }

  void SaveBuffer(NetworkState state)
  {
    for (int i = stateBuffer.Length - 1; i > 0; i--)
    {
      stateBuffer[i] = stateBuffer[i - 1];
    }

    // state[0] is the most current??
    stateBuffer[0] = state;

    stateCount = Mathf.Min(stateCount + 1, stateBuffer.Length);
  }

  private struct NetworkState
  {
    public Vector3 position;
    public double timestamp;

    public NetworkState(Vector3 pos, double time)
    {
      this.position = pos;
      this.timestamp = time;
    }
  }
}

