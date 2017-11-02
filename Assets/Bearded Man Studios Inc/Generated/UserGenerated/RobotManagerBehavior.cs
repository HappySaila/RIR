using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"Vector3\", \"Vector3\"][\"Vector3\", \"Quaternion\", \"int\"][\"Vector3\"][\"int\", \"int\"][][\"int\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"force\", \"point\"][\"Position\", \"rotation\", \"team\"][\"position\"][\"team\", \"type\"][][\"team\"]]")]
	public abstract partial class RobotManagerBehavior : NetworkBehavior
	{
		public const byte RPC_RAM_PLAYER = 0 + 5;
		public const byte RPC_SET_STARTING = 1 + 5;
		public const byte RPC_MAKE_INTO_LABOURER = 2 + 5;
		public const byte RPC_SYNC_ROBOT_STATE = 3 + 5;
		public const byte RPC_DESTROY_MYSELF = 4 + 5;
		public const byte RPC_I_DIED = 5 + 5;
		
		public RobotManagerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (RobotManagerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("ramPlayer", ramPlayer, typeof(Vector3), typeof(Vector3));
			networkObject.RegisterRpc("setStarting", setStarting, typeof(Vector3), typeof(Quaternion), typeof(int));
			networkObject.RegisterRpc("makeIntoLabourer", makeIntoLabourer, typeof(Vector3));
			networkObject.RegisterRpc("syncRobotState", syncRobotState, typeof(int), typeof(int));
			networkObject.RegisterRpc("destroyMyself", destroyMyself);
			networkObject.RegisterRpc("iDied", iDied, typeof(int));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId))
					ProcessOthers(gameObject.transform, obj.NetworkId + 1);
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
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
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new RobotManagerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new RobotManagerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// Vector3 force
		/// Vector3 point
		/// </summary>
		public abstract void ramPlayer(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// Vector3 Position
		/// Quaternion rotation
		/// int team
		/// </summary>
		public abstract void setStarting(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// Vector3 position
		/// </summary>
		public abstract void makeIntoLabourer(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int team
		/// int type
		/// </summary>
		public abstract void syncRobotState(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void destroyMyself(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void iDied(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}