using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhotonNetworkedPlayer : Photon.MonoBehaviour
{
  public float lerpFactor = 10f;

  public float interpolationBackTime = 0.1f;

  // private Vector3 lastReceievedPosition;
  private NetworkState[] stateBuffer = new NetworkState[20];
  private int stateCount;

  /// <summary>
  /// The history of move states send from client to server
  /// </summary>
  private List<MoveStruct> moveHistory = new List<MoveStruct>();

  /// <summary>
  /// Cached player object
  /// </summary>
  private PlayerPun playerObject;

  private float updateTimer = 0f;

  void Start()
  {
    playerObject = GetComponent<PlayerPun>();
  }

  /*
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
  */

  void FixedUpdate()
  {
    if (photonView.isMine)
    {
      MoveStruct moveState = new MoveStruct(playerObject.horizAxis, playerObject.vertAxis, PhotonNetwork.time);

      // buffer move state
      moveHistory.Insert(0, moveState);

      // TODO: parameterise this
      if (moveHistory.Count > 200)
      {
        moveHistory.RemoveAt(moveHistory.Count - 1);
      }

      playerObject.Simulate();

      // send state to server?
      photonView.RPC("ProcessInput", PhotonTargets.MasterClient, moveState.HorizontalAxis, moveState.VecticalAxis, transform.position);
    }
  }

  /// <summary>
  /// Receives input from the client
  /// </summary>
  /// <param name="horizAxis">Horiz axis.</param>
  /// <param name="vertAxis">Vert axis.</param>
  /// <param name="possie">Possie.</param>
  /// <param name="info">Info.</param>
  [RPC]
  void ProcessInput(float horizAxis, float vertAxis, Vector3 possie, PhotonMessageInfo info)
  {
    if (photonView.isMine || !PhotonNetwork.isMasterClient)
    {
      return;
    }

    // execute input
    playerObject.horizAxis = horizAxis;
    playerObject.vertAxis = vertAxis;
    playerObject.Simulate();

    // compare the results
    // TODO: make this configurable, consider using sqrt dist?
    if (Vector3.Distance(transform.position, possie) > 0.1f)
    {
      Debug.Log("Correcting input: " + possie);
      photonView.RPC("CorrectState", info.sender, transform.position);
    }
  }

  /// <summary>
  /// Sent from the master client to slave clients when they are out of sync
  /// </summary>
  /// <param name="correctPossie">Correct possie.</param>
  /// <param name="info">Info.</param>
  [RPC]
  void CorrectState(Vector3 correctPossie, PhotonMessageInfo info)
  {
    int pastState = 0;
    // TODO: convert to LINQ?
    for (int i = 0; i < moveHistory.Count; i++)
    {
      if (moveHistory[i].Timestamp <= info.timestamp)
      {
        pastState = i;
        break;
      }
    }

    // rewind
    transform.position = correctPossie;

    // replay
    for (int i = pastState; i>= 0; i--)
    {
      playerObject.horizAxis = moveHistory[i].HorizontalAxis;
      playerObject.vertAxis = moveHistory[i].VecticalAxis;
      playerObject.Simulate();
    }

    // clear the move history??
    moveHistory.Clear();
  }

  [RPC]
  void NetUpdate(Vector3 possie, PhotonMessageInfo info)
  {
    if (!photonView.isMine)
    {
      SaveBuffer(new NetworkState(possie, info.timestamp));
    }
  }

  void Update()
  {
    // if this is the master client, send out position updates every nth of a second
    if (PhotonNetwork.isMasterClient)
    {
      updateTimer += Time.deltaTime;
      // TODO: again, make this configurable??
      if (updateTimer >= 0.1f)
      {
        updateTimer = 0f;
        photonView.RPC("NetUpdate", PhotonTargets.Others, transform.position);
      }
      return;
    }

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

  private struct MoveStruct
  {
    public float HorizontalAxis;
    public float VecticalAxis;
    public double Timestamp;

    public MoveStruct(float horiz, float vert, double timestamp)
    {
      this.HorizontalAxis = horiz;
      this.VecticalAxis = vert;
      this.Timestamp = timestamp;
    }
  }

}

