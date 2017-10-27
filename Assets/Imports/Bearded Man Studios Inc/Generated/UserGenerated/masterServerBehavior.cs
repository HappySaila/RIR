using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"string\", \"int\", \"int\"][\"string\", \"int\"][\"string\"][\"string\", \"string\"][\"string\"][\"string\"][\"string\", \"string\"][\"ushort\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"name\", \"partySize\", \"mmr\"][\"name\", \"mmr\"][\"Name\"][\"team1\", \"team2\"][\"name\"][\"roomName\"][\"team1\", \"team2\"][\"port\"]]")]
	public abstract partial class masterServerBehavior : NetworkBehavior
	{
		public const byte RPC_CREATE_ROOM = 0 + 5;
		public const byte RPC_JOIN_ROOM = 1 + 5;
		public const byte RPC_START_MATCHING = 2 + 5;
		public const byte RPC_START_GAME = 3 + 5;
		public const byte RPC_PLAYER_JOIN_ROOM = 4 + 5;
		public const byte RPC_READY_UP = 5 + 5;
		public const byte RPC_STOP_SEARCHING = 6 + 5;
		public const byte RPC_CONNECT_TO_NEW_SERVER = 7 + 5;
		
		public masterServerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (masterServerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("createRoom", createRoom, typeof(string), typeof(int), typeof(int));
			networkObject.RegisterRpc("joinRoom", joinRoom, typeof(string), typeof(int));
			networkObject.RegisterRpc("startMatching", startMatching, typeof(string));
			networkObject.RegisterRpc("StartGame", StartGame, typeof(string), typeof(string));
			networkObject.RegisterRpc("playerJoinRoom", playerJoinRoom, typeof(string));
			networkObject.RegisterRpc("readyUp", readyUp, typeof(string));
			networkObject.RegisterRpc("stopSearching", stopSearching, typeof(string), typeof(string));
			networkObject.RegisterRpc("connectToNewServer", connectToNewServer, typeof(ushort));

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId))
					ProcessOthers(gameObject.transform, obj.NetworkId + 1);
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata == null)
				return;

			byte transformFlags = obj.Metadata[0];

			if (transformFlags == 0)
				return;

			BMSByte metadataTransform = new BMSByte();
			metadataTransform.Clone(obj.Metadata);
			metadataTransform.MoveStartIndex(1);

			if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
			{
				MainThreadManager.Run(() =>
				{
					transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
					transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
				});
			}
			else if ((transformFlags & 0x01) != 0)
			{
				MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
			}
			else if ((transformFlags & 0x02) != 0)
			{
				MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
			}
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new masterServerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new masterServerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// string name
		/// int partySize
		/// int mmr
		/// </summary>
		public abstract void createRoom(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string name
		/// int mmr
		/// </summary>
		public abstract void joinRoom(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string Name
		/// </summary>
		public abstract void startMatching(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string team1
		/// string team2
		/// </summary>
		public abstract void StartGame(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string name
		/// </summary>
		public abstract void playerJoinRoom(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string roomName
		/// </summary>
		public abstract void readyUp(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string team1
		/// string team2
		/// </summary>
		public abstract void stopSearching(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// ushort port
		/// </summary>
		public abstract void connectToNewServer(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}