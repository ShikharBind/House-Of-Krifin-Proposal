using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using agora_gaming_rtc;

public class AgoraVideoChat : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private string appID = ""; // *NOTE* Add your own appID from console.agora.io
    [SerializeField]
    private string channel = "Testing";
    private string originalChannel;
    private IRtcEngine mRtcEngine;
    private uint myUID = 0;

    [Header("Player Video Panel Properties")]
    [SerializeField]
    private GameObject userVideoPrefab;


    private int Offset = 100;



    void Start()
    {
        if (!photonView.isMine)
            return;
        // Setup Agora Engine and Callbacks.
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
        }
        originalChannel = channel;
        mRtcEngine = IRtcEngine.GetEngine(appID);
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccessHandler;
        mRtcEngine.OnUserJoined = OnUserJoinedHandler;
        mRtcEngine.OnLeaveChannel = OnLeaveChannelHandler;
        mRtcEngine.OnUserOffline = OnUserOfflineHandler;
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        mRtcEngine.JoinChannel(channel, null, 0);
    }

    // Local Client Joins Channel.
    private void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        if (!photonView.isMine)
            return;
        myUID = uid;
        Debug.LogFormat("I: {0} joined channel: {1}.", uid.ToString(), channelName);
        CreateUserVideoSurface(uid, true);
    }

    // Remote Client Joins Channel.
    private void OnUserJoinedHandler(uint uid, int elapsed)
    {
        if (!photonView.isMine)
            return;
        CreateUserVideoSurface(uid, false);
    }

    // Local user leaves channel.
    private void OnLeaveChannelHandler(RtcStats stats)
    {
        if (!photonView.isMine)
            return;
    }

    // Remote User Leaves the Channel.
    private void OnUserOfflineHandler(uint uid, USER_OFFLINE_REASON reason)
    {
        if (!photonView.isMine)
            return;
    }

    void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine = null;
            IRtcEngine.Destroy();
        }
    }

    private void CreateUserVideoSurface(uint uid, bool isLocalUser)
    {
        // Create Gameobject holding video surface and update properties
        GameObject newUserVideo = Instantiate(userVideoPrefab);
        if (newUserVideo == null)
        {
            Debug.LogError("CreateUserVideoSurface() - newUserVideoIsNull");
            return;
        }
        newUserVideo.name = uid.ToString();

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            newUserVideo.transform.parent = canvas.transform;
        }
        // set up transform for new VideoSurface
        newUserVideo.transform.Rotate(0f, 0.0f, 180.0f);
        float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        newUserVideo.transform.localPosition = new Vector3(xPos, yPos, 0f);
        newUserVideo.transform.localScale = new Vector3(3f, 4f, 1f);
        newUserVideo.transform.rotation = Quaternion.Euler(Vector3.right * -180);

        // Update our VideoSurface to reflect new users
        VideoSurface newVideoSurface = newUserVideo.GetComponent<VideoSurface>();
        if (newVideoSurface == null)
        {
            Debug.LogError("CreateUserVideoSurface() - VideoSurface component is null on newly joined user");
        }

        if (isLocalUser == false)
        {
            newVideoSurface.SetForUser(uid);
        }
        newVideoSurface.SetGameFps(30);
    }

    public void JoinRemoteChannel(string remoteChannelName)
    {
        if (!photonView.isMine)
            return;
        mRtcEngine.LeaveChannel();
        mRtcEngine.JoinChannel(remoteChannelName, null, myUID);
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        channel = remoteChannelName;
    }

    public string GetCurrentChannel() => channel;

    void SetupLocalVideoView()
    {
        VideoCanvas localCanvas = new VideoCanvas();
        localCanvas.uid = 0;
        localCanvas.renderMode = VideoRenderMode.Hidden;
        localCanvas.view = localVideoView.GetComponent<RawImage>();
        mRtcEngine.SetupLocalVideo(localCanvas);
    }

    void SetupRemoteVideoView()
    {
        VideoCanvas remoteCanvas = new VideoCanvas();
        remoteCanvas.uid = uid;
        remoteCanvas.renderMode = VideoRenderMode.Fit;
        remoteCanvas.view = remoteVideoView.GetComponent<RawImage>();
        mRtcEngine.SetupRemoteVideo(remoteCanvas);
    }


}