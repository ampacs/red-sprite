using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// The NetworkSceneManager class is used by the server to load, unload, and change the scenes players are currently playing through. 
/// The calls are networked, so when you load a scene it will load on all other clients. You can listen to events to know when the actions
/// have been completed on the clients.
/// </summary>
public class NetworkSceneManager : NetworkBehaviour
{
    public delegate void SceneLoadedHandler(string sceneName);
    /// <summary>
    /// Event fired when all clients have finished loading a scene asynchronously.
    /// </summary>
    public event SceneLoadedHandler SceneLoaded;

    public delegate void SceneUnloadedHandler(string sceneName);
    /// <summary>
    /// Event fired when all clients have finished unloading a scene asynchronously.
    /// </summary>
    public event SceneUnloadedHandler SceneUnloaded;

    public delegate void ActiveSceneChangedHandler(string activeSceneName);
    /// <summary>
    /// Event fired when all clients have changed their active scene.
    /// </summary>
    public event ActiveSceneChangedHandler ActiveSceneChanged;

    /// <summary>
    /// Singleton instance of this manager
    /// </summary>
    public static NetworkSceneManager instance;

    private Dictionary<string, int> sceneLoadedStatus;
    private Dictionary<string, int> sceneUnloadedStatus;
    private Dictionary<string, int> sceneSetActiveStatus;
    
    private NetworkClient m_Client;
    private const short ClientLoadeSceneMsg = 1001;

    private void Awake()
    {
        sceneLoadedStatus = new Dictionary<string, int>();
        sceneUnloadedStatus = new Dictionary<string, int>();
        sceneSetActiveStatus = new Dictionary<string, int>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }
    
    private void Start()
    {
        // register custom network message handlers
        NetworkServer.RegisterHandler(NetworkMessages.CLIENT_SCENE_LOADED, MsgClientLoadedScene);
        NetworkServer.RegisterHandler(NetworkMessages.CLIENT_SCENE_UNLOADED, MsgClientUnloadedScene);
        NetworkServer.RegisterHandler(NetworkMessages.CLIENT_ACTIVE_SCENE_CHANGED, MsgClientActiveSceneChanged);
    }

    private void OnDestroy()
    {
        // unregister custom network message handlers
        NetworkServer.UnregisterHandler(NetworkMessages.CLIENT_SCENE_LOADED);
        NetworkServer.UnregisterHandler(NetworkMessages.CLIENT_SCENE_UNLOADED);
        NetworkServer.UnregisterHandler(NetworkMessages.CLIENT_ACTIVE_SCENE_CHANGED);
    }

    /// <summary>
    /// Additively loads a scene on all clients. This method can only be called from the server.
    /// </summary>
    /// <param name="sceneName">Name of scene to load, make sure it's in your build settings scene list.</param>
    public void ServerLoadScene(string sceneName)
    {
        if (!isServer)
        {
            Debug.LogError("Attempting to call LoadScene from client. This method should only be called on the server");
            return;
        }

        ServerListenForLoadScene(sceneName);
        LoadSceneAdditively(sceneName);
        RpcLoadScene(sceneName);

        // Set all clients to not ready, they will flag themselves ready after scene load
        NetworkServer.SetAllClientsNotReady();
    }

    /// <summary>
    /// Unloads a scene on all clients. This method can only be called from the server.
    /// </summary>
    /// <param name="sceneName">Name of scene to unload, make sure it's in your build settings scene list.</param>
    public void ServerUnloadScene(string sceneName)
    {
        if (!isServer)
        {
            Debug.LogError("Attempting to call UnloadScene from client. This method should only be called on the server");
            return;
        }

        ServerListenForUnloadScene(sceneName);
        UnloadSceneAsync(sceneName);
        RpcUnloadScene(sceneName);
    }

    /// <summary>
    /// Changes the currently active scene. This method can only be called from the server.
    /// </summary>
    /// <param name="sceneName">Name of scene to make the active scene.</param>
    public void ServerSetActiveScene(string sceneName)
    {
        if (!isServer)
        {
            Debug.LogError("Attempting to call SetActiveScene from client. This method should only be called on the server");
            return;
        }

        ServerListenForSetActiveScene(sceneName);
        SetActiveScene(sceneName);
        RpcSetActiveScene(sceneName);
    }

    

    private void MsgClientLoadedScene(NetworkMessage msg)
    {
        string sceneName = msg.ReadMessage<StringMessage>().value;
        ServerDecrementSceneLoadedCount(sceneName);
    }

    private void MsgClientUnloadedScene(NetworkMessage msg)
    {
        string sceneName = msg.ReadMessage<StringMessage>().value;
        ServerDecrementSceneUnloadedCount(sceneName);
    }

    private void MsgClientActiveSceneChanged(NetworkMessage msg)
    {
        string sceneName = msg.ReadMessage<StringMessage>().value;
        ServerDecrementSetActiveSceneCount(sceneName);
    }

    private void ServerListenForLoadScene(string sceneName)
    {
        sceneLoadedStatus[sceneName] = NetworkServer.connections.Count;        
    }

    private void ServerListenForUnloadScene(string sceneName)
    {
        sceneUnloadedStatus[sceneName] = NetworkServer.connections.Count;
    }

    private void ServerListenForSetActiveScene(string sceneName)
    {
        sceneSetActiveStatus[sceneName] = NetworkServer.connections.Count;
    }

    /// <summary>
    /// HACK: These methods aren't really meant to be used long term, but are more temporary
    /// while we rough in the network events. The current problem with this approach is it doesn't
    /// handle a client disconnect properly. To do that, we'll need to watch the network manager
    /// and adjust our expectations accordingly (GameNetworkManager.OnServerRemovePlayer) if someone 
    /// disconnects while loading. For now this is good enough.
    /// </summary>
    /// <param name="sceneName"></param>
    private void ServerDecrementSceneLoadedCount(string sceneName)
    {
        if (sceneLoadedStatus.ContainsKey(sceneName))
        {
            sceneLoadedStatus[sceneName]--;

            if (sceneLoadedStatus[sceneName] <= 0)
            {
                DoneLoading(sceneName);
            }
        }
    }

    private void DoneLoading(string sceneName)
    {
        NetworkServer.SpawnObjects();
        FireSceneLoaded(sceneName);
    }
    
    private void ServerDecrementSceneUnloadedCount(string sceneName)
    {
        if (sceneUnloadedStatus.ContainsKey(sceneName))
        {
            sceneUnloadedStatus[sceneName]--;

            if (sceneUnloadedStatus[sceneName] <= 0)
            {
                DoneUnloading(sceneName);
            }
        }
    }

    private void DoneUnloading(string sceneName)
    {
        FireSceneUnloaded(sceneName);
    }

    private void ServerDecrementSetActiveSceneCount(string activeSceneName)
    {
        if (sceneSetActiveStatus.ContainsKey(activeSceneName))
        {
            sceneSetActiveStatus[activeSceneName]--;

            if (sceneSetActiveStatus[activeSceneName] <= 0)
            {
                DoneSetActiveScene(activeSceneName);
            }
        }
    }

    private void DoneSetActiveScene(string activeSceneName)
    {
        FireActiveSceneChanged(activeSceneName);
    }

    [ClientRpc]
    private void RpcLoadScene(string sceneName)
    {
        // do not load the scene if the client is also the host
        if (!isServer)
        {
            LoadSceneAdditively(sceneName);
        }
    }

    [ClientRpc]
    private void RpcUnloadScene(string sceneName)
    {
        // do not unload the scene if the client is also the host
        if (!isServer)
        {
            UnloadSceneAsync(sceneName);
        }
    }

    [ClientRpc]
    private void RpcSetActiveScene(string sceneName)
    {
        // do not set the active scene if the client is also the host
        if (!isServer)
        {
            SetActiveScene(sceneName);
        }
    }

    private void LoadSceneAdditively(string sceneName)
    {
        StartCoroutine(DoLoadSceneAsync(sceneName));
    }

    private IEnumerator DoLoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // ForceSceneId on all network identities to prevent collisions when loading multiple scenes
        SetSceneIds(SceneManager.GetSceneByName(sceneName));

        // Send ready message to server indicating this client is ready to spawn objects
        ClientScene.Ready(Player.localPlayer.connectionToServer);

        if (isServer)
        {
            ServerDecrementSceneLoadedCount(sceneName);
        }
        else
        {
            SendClientSceneLoadedMessage(sceneName);
        }
    }

    private void UnloadSceneAsync(string sceneName)
    {
        Scene oldScene = SceneManager.GetSceneByName(sceneName);

        if (oldScene.buildIndex == -1)
        {
            Debug.LogError("Attempting to unload scene that is not in the build settings: " + sceneName);
        }
        else if (oldScene == SceneManager.GetActiveScene())
        {
            Debug.LogError("Attempting to unload active scene: " + sceneName + ". You can not unload the active scene");
        }
        else if(oldScene.isLoaded)
        {
            StartCoroutine(DoUnloadSceneAsync(oldScene));
        }
    }

    private IEnumerator DoUnloadSceneAsync(Scene scene)
    {
        string sceneName = scene.name;
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
        
        if (isServer)
        {
            ServerDecrementSceneUnloadedCount(sceneName);
        }
        else
        {
            SendClientSceneUnloadedMessage(sceneName);
        }
    }

    private void SetActiveScene(string sceneName)
    {
        Scene previouslyActiveScene = SceneManager.GetActiveScene();
        Scene newScene = SceneManager.GetSceneByName(sceneName);

        if (!newScene.isLoaded)
        {
            Debug.LogError("Attempting to set unloaded scene to active scene: " + sceneName);
        }
        else if (newScene == previouslyActiveScene)
        {
            Debug.LogWarning("Attempting to set active scene to already active scene: " + sceneName);
        }
        else
        {
            SceneManager.SetActiveScene(newScene);
            
            if (isServer)
            {
                ServerDecrementSetActiveSceneCount(newScene.name);
            }
            else
            {
                SendClientActiveSceneChangedMessage(newScene.name);
            }
        }
    }
    
    private void FireSceneLoaded(string sceneName)
    {
        if(SceneLoaded != null)
        {
            SceneLoaded(sceneName);
        }
    }

    private void FireSceneUnloaded(string sceneName)
    {
        if (SceneUnloaded != null)
        {
            SceneUnloaded(sceneName);
        }
    }

    private void FireActiveSceneChanged(string activeSceneName)
    {
        if (ActiveSceneChanged != null)
        {
            ActiveSceneChanged(activeSceneName);
        }
    }

    private void SendClientSceneLoadedMessage(string sceneName)
    {
        GameManager.instance.client.Send(NetworkMessages.CLIENT_SCENE_LOADED, new StringMessage(sceneName));
    }

    private void SendClientSceneUnloadedMessage(string sceneName)
    {
        GameManager.instance.client.Send(NetworkMessages.CLIENT_SCENE_UNLOADED, new StringMessage(sceneName));
    }

    private void SendClientActiveSceneChangedMessage(string sceneName)
    {
        GameManager.instance.client.Send(NetworkMessages.CLIENT_ACTIVE_SCENE_CHANGED, new StringMessage(sceneName));
    }

    private void SetSceneIds(Scene scene)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings > 0 ? SceneManager.sceneCountInBuildSettings : 1;
        int maxSceneIdSize = (int.MaxValue - 1) / sceneCount;

        ForceSceneIds(scene, maxSceneIdSize);
    }

    private void ForceSceneIds(Scene scene, int maxSceneIdSize)
    {
        int nextSceneId = (scene.buildIndex * maxSceneIdSize) + 1;
        int maxSceneId = nextSceneId + maxSceneIdSize;

        foreach (GameObject rootObject in scene.GetRootGameObjects().OrderBy(ro => ro.name))
        {
            foreach (NetworkIdentity networkIdentity in rootObject.GetComponentsInChildren<NetworkIdentity>(true))
            {
                if (networkIdentity.GetComponent<NetworkManager>() != null)
                {
                    Debug.LogWarning("NetworkManager in " + scene.name + " has a NetworkIdentity component. This will cause the NetworkManager object to be disabled, so it is not recommended.");
                }

                if (nextSceneId >= maxSceneId)
                {
                    Debug.LogError("Scene index " + scene.buildIndex + " has more than the max allowed scene NetworkIdentities (" + maxSceneIdSize + "). Ignoring the extras.");
                    break;
                }
                
                networkIdentity.ForceSceneId(nextSceneId++);
            }
        }
    }
}
