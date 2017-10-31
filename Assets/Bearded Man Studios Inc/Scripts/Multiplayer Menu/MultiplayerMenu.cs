using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
	public InputField ipAddress = null;
	public InputField portNumber = null;
	public bool DontChangeSceneOnConnect = false;

	public GameObject networkManager = null;
	public GameObject[] ToggledButtons;
	private NetworkManager mgr = null;

	private List<Button> _uiButtons = new List<Button>();

	private void Start()
	{
		ipAddress.text = "127.0.0.1";
		portNumber.text = "15937";

			// Do any firewall opening requests on the operating system
			NetWorker.PingForFirewall(ushort.Parse(portNumber.text));
		

			Rpc.MainThreadRunner = MainThreadManager.Instance;
	}

	public void Connect()
	{
		ushort port;
		if(!ushort.TryParse(portNumber.text, out port))
		{
			Debug.LogError("The supplied port number is not within the allowed range 0-" + ushort.MaxValue);
		    	return;
		}

		NetWorker client;


		client = new UDPClient();
		((UDPClient)client).Connect(ipAddress.text, (ushort)port);


		Connected(client);
	}

	public void Host()
	{
		NetWorker server;

		server = new UDPServer(64);

			((UDPServer)server).Connect(ipAddress.text, ushort.Parse(portNumber.text));


		server.playerTimeout += (player, sender) =>
		{
			Debug.Log("Player " + player.NetworkId + " timed out");
		};
        NetworkManager.Instance.Initialize(server);
        Connected(server);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
			Host();
		else if (Input.GetKeyDown(KeyCode.C))
			Connect();
		else if (Input.GetKeyDown(KeyCode.L))
		{
			NetWorker.localServerLocated -= TestLocalServerFind;
			NetWorker.localServerLocated += TestLocalServerFind;
			NetWorker.RefreshLocalUdpListings();
		}
	}

	private void TestLocalServerFind(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
	{
		Debug.Log("Address: " + endpoint.Address + ", Port: " + endpoint.Port + ", Server? " + endpoint.IsServer);
	}

	public void Connected(NetWorker networker)
	{
        NetworkManager.Instance.Initialize(networker);
        if (!networker.IsBound)
		{
			Debug.LogError("NetWorker failed to bind");
			return;
		}

		if (mgr == null && networkManager == null)
		{
			Debug.LogWarning("A network manager was not provided, generating a new one instead");
			networkManager = new GameObject("Network Manager");
			mgr = networkManager.AddComponent<NetworkManager>();
		}
		else if (mgr == null)
			mgr = Instantiate(networkManager).GetComponent<NetworkManager>();


        /*if (networker.IsServer)
			SceneManager.sceneLoaded += CreateInlineChat;
            */
        NetworkObject.Flush(networker);
        /*if (networker is IServer)
		{
                Debug.Log("I AM THE SERVER");
				NetworkObject.Flush(networker); //Called because we are already in the correct scene!
                Debug.Log(networker.NetworkObjectList.Count);
                Debug.Log(networker.IsServer);

        }*/
        if (false)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        Invoke("spawnObject", 1f);
    }

    public void spawnObject()
    {

        //Debug.Log(mgr.IsServer);
        //Debug.Log(NetworkManager.Instance.IsServer);
        NetworkManager.Instance.InstantiatemasterServer();
        //NetworkManager.Instance.
    }

    private void CreateInlineChat(Scene arg0, LoadSceneMode arg1)
	{
		SceneManager.sceneLoaded -= CreateInlineChat;
		var chat = NetworkManager.Instance.InstantiateChatManager();
		DontDestroyOnLoad(chat.gameObject);
	}

	private void SetToggledButtons(bool value)
	{
		for (int i = 0; i < _uiButtons.Count; ++i)
			_uiButtons[i].interactable = value;
	}

	private void OnApplicationQuit()
	{
	}
}
