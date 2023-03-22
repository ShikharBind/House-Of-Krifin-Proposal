public Transform playerPrefab;

void Start()
{
    if (PhotonNetwork.IsConnected)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f)), Quaternion.identity, 0);
    }
}