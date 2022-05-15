using Mirror;
using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core
{
    public class NetworkLiar
    {
        public static NetworkLiar Get { get => VtController.Get.NetworkLiar; }

        public void SendRole(Player player, RoleType info, List<Player> players)
        {
            const byte bytecodes = 8;

            var owner = NetworkWriterPool.GetWriter();
            var observer = NetworkWriterPool.GetWriter();

            // Get behavior and index of it
            GetBehaviour<CharacterClassManager>(player.Hub.networkIdentity, out var behaviourIndex, out var behaviour);

            // Writ
            owner.WriteByte((byte)behaviourIndex);

            var positionRef = owner.Position;
            owner.WriteInt32(0);
            var positionData = owner.Position;

            behaviour.SerializeObjectsDelta(owner);

            // Write var
            owner.WriteUInt64(bytecodes);
            owner.WriteSByte((sbyte)info);

            // Write syncdata position data
            WritePostion(owner, positionRef, positionData);

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
            {
                var arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, positionRef, owner.Position - positionRef);
            }

            //send
            SendAndRecycle(owner, observer, players, player);
        }

        public void SendDisplayInfo(Player player, string info, List<Player> players)
        {
            const byte bytecodes = 2;

            var owner = NetworkWriterPool.GetWriter();
            var observer = NetworkWriterPool.GetWriter();
            
            // Get behavior and index of it
            GetBehaviour<NicknameSync>(player.Hub.networkIdentity, out var behaviourIndex, out var behaviour);

            // Writ
            owner.WriteByte((byte)behaviourIndex);

            var positionRef = owner.Position;
            owner.WriteInt32(0);
            var positionData = owner.Position;

            behaviour.SerializeObjectsDelta(owner);

            // Write var
            owner.WriteUInt64(bytecodes);
            owner.WriteString(info);

            // Write syncdata position data
            WritePostion(owner, positionRef, positionData);

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
            {
                var arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, positionRef, owner.Position - positionRef);
            }

            //send
            SendAndRecycle(owner, observer, players, player); 
        }

        public void SendAndRecycle(PooledNetworkWriter owner, PooledNetworkWriter observer, List<Player> receivers, Player player)
        {
            //send
            foreach (var client in receivers)
                client.Hub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = player.Hub.networkIdentity.netId, payload = owner.ToArraySegment() });

            //Free
            NetworkWriterPool.Recycle(owner);
            NetworkWriterPool.Recycle(observer);
        }

        public void GetBehaviour<T>(NetworkIdentity networkIdentity, out int behaviourIndex, out NetworkBehaviour behaviour)
        {
            // Get behavior and index of it
            for (var i = 0; i < networkIdentity.NetworkBehaviours.Length; i++)
            {
                if (networkIdentity.NetworkBehaviours[i].GetType() == typeof(T))
                {
                    behaviourIndex = i;
                    behaviour = networkIdentity.NetworkBehaviours[i];
                    return;
                }
            }
            throw new NullReferenceException($"Get behaviour faild ! Player ave no {typeof(T).Name} !");
            //not working
            /*
            behaviourIndex = Array.FindIndex(networkIdentity.NetworkBehaviours, b => b.GetType() == typeof(T));
            if (behaviourIndex != -1)
                throw new NullReferenceException($"Get behaviour faild ! Player ave no {typeof(T).Name} !");//todo fix this

            behaviour = networkIdentity.NetworkBehaviours[behaviourIndex];
            */
        }

        public void WritePostion(PooledNetworkWriter owner, int positionRef, int positionData)
        {
            int positionEnd = owner.Position;
            owner.Position = positionRef;
            owner.WriteInt32(positionEnd - positionData);
            owner.Position = positionEnd;
        }
    }
}
