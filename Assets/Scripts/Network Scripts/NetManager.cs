using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetManager : NetworkManager {

    private bool firstPlayerJoined;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject playerObj = Instantiate(playerPrefab,Vector3.zero,Quaternion.identity);
        List<Transform> spawnPos = NetworkManager.singleton.startPositions;
        if (!firstPlayerJoined)
        {
            firstPlayerJoined = true;
            playerObj.transform.position = spawnPos[0].position;

        }
        else
        {
            playerObj.transform.position = spawnPos[1].position;
        }
        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
    }
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 777;
        NetworkManager.singleton.networkAddress = "localhost";

    }
    public void HostGame()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }
    public void Join()
    {
        SetPort();
        NetworkManager.singleton.StartClient();
    }
}
