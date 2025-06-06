using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Linq;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static BasicSpawner Instance;
    [SerializeField] private GameObject fieldPrefab;
    private bool Iwin = false;
    private Canvas canvas;
    #region Network properties 
    private NetworkRunner runner;

   [SerializeField] private NetworkPrefabRef playerPrefab;
   [SerializeField] private NetworkPrefabRef networkedGameManager;
   private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    #endregion 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }

   public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
   {
       if (runner.IsServer)
       {
           // Create a unique position for the player
           Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
           NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
           // Keep track of the player avatars for easy access
           _spawnedCharacters.Add(player, networkPlayerObject);
       }
   }
   
    public void InitializeField()
    {
        GameObject field = Instantiate(fieldPrefab, canvas.transform);
    }

   public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
   {
       if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
       {
           runner.Despawn(networkObject);
           _spawnedCharacters.Remove(player);
       }
   }
    
   public void OnInput(NetworkRunner runner, NetworkInput input) {}

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
   public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (Iwin)
        {
            SceneManager.LoadScene("WinnersScene");
        }
        else
        {
            SceneManager.LoadScene("LosersScene");
        }
    }

   async void StartGame(GameMode mode)
   {
       // Create the Fusion runner and let it know that we will be providing user input
       runner = gameObject.AddComponent<NetworkRunner>();
       runner.ProvideInput = true;

       // Create the NetworkSceneInfo from the current scene
       var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);//Todos en la misma escena
       var sceneInfo = new NetworkSceneInfo();
       if (scene.IsValid)
       {
           sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
       }

        // Start or join (depends on gamemode) a session with a specific name
        await runner.StartGame(new StartGameArgs()
       {
           GameMode = mode,
           SessionName = "TestRoom",
           Scene = scene,
           SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
           PlayerCount = 2
        });

        if (runner.IsServer)
        {
            runner.Spawn(
                networkedGameManager,
                Vector3.zero,
                Quaternion.identity,
                inputAuthority: PlayerRef.None // Objeto sin dueño específico
            );
        }
    }

   private void OnGUI()
   {
       if (runner == null)
       {
           if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
           {
                InitializeField();
                StartGame(GameMode.Host);
           }
           if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
           {
                InitializeField();
                StartGame(GameMode.Client);
           }
       }
   }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) 
    {
        if (runner.SessionInfo.PlayerCount >= 2)
        {
            Debug.Log("Partida llena. Conexión rechazada.");
        }
        else
        {
            request.Accept(); // Permite la conexión
        }
    }

    public void CleanupRunner()
    {
        _spawnedCharacters.Clear();
        runner = null;
    }

    public void ReturnToLobby(bool IsHostWinner)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.RPC_EndGame(IsHostWinner);
        }
    }

    public void ReturnToLobby2(bool IsHostWinner)
    {
        if (runner.IsServer) 
        {
            StartCoroutine(EndGameSequence(IsHostWinner));
        }
        else
        {
            if (!IsHostWinner)
            {
                Iwin = true;
            }
            runner.Shutdown();
        }

        Debug.Log(Iwin);
    }

    private IEnumerator EndGameSequence(bool IsHostWinner)
    {
        yield return new WaitForSeconds(2f);


        if (IsHostWinner)
        {
            Iwin = true;
        }
        runner.Shutdown();
    }

    private IEnumerator ShutdownHost()
    {
        yield return new WaitForSeconds(1f); 
        if (runner != null)
        {
            runner.Shutdown(destroyGameObject: false);
            Destroy(runner);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
