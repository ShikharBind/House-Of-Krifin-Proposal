using UnityEngine.UI;

public class AgoraPartyJoiner : MonoBehaviourPunCallbacks
{
    [Header("Local Player Stats")]
    [SerializeField]
    private Button inviteButton;
    [SerializeField]
    private GameObject joinButton;
    [SerializeField]
    private GameObject leaveButton;


    [Header("Remote Player Stats")]
    [SerializeField]
    private int remotePlayerViewID;
    [SerializeField]
    private string remoteInviteChannelName = null;

    private AgoraVideoChat agoraVideo;


    private void Awake()
    {
        agoraVideo = GetComponent<AgoraVideoChat>();
    }

    private void Start()
    {
        if (!photonView.isMine)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        inviteButton.interactable = false;
        joinButton.SetActive(false);
        leaveButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.isMine || !other.CompareTag("Player"))
        {
            return;
        }
        // Used for calling RPC events on other players.
        PhotonView otherPlayerPhotonView = other.GetComponent<PhotonView>();
        if (otherPlayerPhotonView != null)
        {
            remotePlayerViewID = otherPlayerPhotonView.viewID;
            inviteButton.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.isMine || !other.CompareTag("Player"))
        {
            return;
        }
        remoteInviteChannelName = null;
        inviteButton.interactable = false;
        joinButton.SetActive(false);
    }

    public void OnInviteButtonPress()
    {

        PhotonView.Find(remotePlayerViewID).RPC("InvitePlayerToPartyChannel", PhotonTargets.All, remotePlayerViewID, agoraVideo.GetCurrentChannel());
    }

    public void OnJoinButtonPress()
    {
        if (photonView.isMine && remoteInviteChannelName != null)
        {
            agoraVideo.JoinRemoteChannel(remoteInviteChannelName);
            joinButton.SetActive(false);
            leaveButton.SetActive(true);
        }
    }

    public void OnLeaveButtonPress()
    {
        if (!photonView.isMine)
            return;
    }

    [PunRPC]
    public void InvitePlayerToPartyChannel(int invitedID, string channelName)
    {
        if (photonView.isMine && invitedID == photonView.viewID)
        {
            joinButton.SetActive(true);
            remoteInviteChannelName = channelName;
        }
    }
}