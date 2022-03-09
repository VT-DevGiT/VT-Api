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
            var behaviorOwner = player.Hub.networkIdentity;
            var behaviourIndex = Array.FindIndex(behaviorOwner.NetworkBehaviours, 0, behaviorOwner.NetworkBehaviours.Length, b => b.GetType() == typeof(CharacterClassManager));
            if (behaviourIndex != -1)
                throw new NullReferenceException("FakeRole faild ! Player ave no CharacterClassManager !");

            var behaviour = behaviorOwner.NetworkBehaviours[behaviourIndex];

            // Writ
            owner.WriteByte((byte)behaviourIndex);

            var positionRef = owner.Position;
            owner.WriteInt32(0);
            var position32 = owner.Position;

            behaviour.SerializeObjectsDelta(owner);

            // Write custom syncvar
            owner.WriteUInt64(bytecodes);
            owner.WriteSByte((sbyte)info);

            // Write syncdata position data
            int positionEnd = owner.Position;
            owner.Position = positionRef;
            owner.WriteInt32(positionEnd - position32);
            owner.Position = positionEnd;

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
            {
                var arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, positionRef, owner.Position - positionRef);
            }

            //send
            foreach (var client in players)
                client.Hub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = player.Hub.networkIdentity.netId, payload = owner.ToArraySegment() });

            //Free
            NetworkWriterPool.Recycle(owner);
            NetworkWriterPool.Recycle(observer);
        }

        public void SendRole(Player player, string info, List<Player> players)
        {

            const byte bytecodes = 2;

            var owner = NetworkWriterPool.GetWriter();
            var observer = NetworkWriterPool.GetWriter();

            // Get behavior and index of it
            var behaviorOwner = player.Hub.networkIdentity;
            var behaviourIndex = Array.FindIndex(behaviorOwner.NetworkBehaviours, 0, behaviorOwner.NetworkBehaviours.Length, b => b.GetType() == typeof(NicknameSync));
            if (behaviourIndex != -1)
                throw new NullReferenceException("FakeRole faild ! Player ave no CharacterClassManager !");

            var behaviour = behaviorOwner.NetworkBehaviours[behaviourIndex];

            // Writ
            owner.WriteByte((byte)behaviourIndex);

            var positionRef = owner.Position;
            owner.WriteInt32(0);
            var position32 = owner.Position;

            behaviour.SerializeObjectsDelta(owner);

            // Write custom syncvar
            owner.WriteUInt64(bytecodes);
            owner.WriteString(info);

            // Write syncdata position data
            int positionEnd = owner.Position;
            owner.Position = positionRef;
            owner.WriteInt32(positionEnd - position32);
            owner.Position = positionEnd;

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
            {
                var arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, positionRef, owner.Position - positionRef);
            }

            //send
            foreach (var client in players)
                client.Hub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = player.Hub.networkIdentity.netId, payload = owner.ToArraySegment() });

            //Free
            NetworkWriterPool.Recycle(owner);
            NetworkWriterPool.Recycle(observer);
        }
    }
}
