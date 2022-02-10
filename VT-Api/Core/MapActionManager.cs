﻿using Synapse;
using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT_Api.Extension;
using VT_Api.Reflexion;
using UERandom = UnityEngine.Random;

namespace VT_Api.Core
{
    public class MapActionManager
    {
        //Original of SanyaPlugin https://github.com/sanyae2439/SanyaPlugin_Exiled
        public bool isAirBombCurrently = false;

        public static MapActionManager Get { get => VtController.Get.MapAction; }

        public Vector3[] AirbombPos
        {
            get
            {
                return new Vector3[]
                {
                    new Vector3(UERandom.Range(175, 182),  984, UERandom.Range( 25,  29)),
                    new Vector3(UERandom.Range(174, 182),  984, UERandom.Range( 36,  39)),
                    new Vector3(UERandom.Range(174, 182),  984, UERandom.Range( 36,  39)),
                    new Vector3(UERandom.Range(166, 174),  984, UERandom.Range( 26,  39)),
                    new Vector3(UERandom.Range(169, 171),  987, UERandom.Range(  9,  24)),
                    new Vector3(UERandom.Range(174, 175),  988, UERandom.Range( 10,  -2)),
                    new Vector3(UERandom.Range(186, 174),  990, UERandom.Range( -1,  -2)),
                    new Vector3(UERandom.Range(186, 189),  991, UERandom.Range( -1, -24)),
                    new Vector3(UERandom.Range(186, 189),  991, UERandom.Range( -1, -24)),
                    new Vector3(UERandom.Range(185, 189),  993, UERandom.Range(-26, -34)),
                    new Vector3(UERandom.Range(180, 195),  995, UERandom.Range(-36, -91)),
                    new Vector3(UERandom.Range(148, 179),  995, UERandom.Range(-45, -72)),
                    new Vector3(UERandom.Range(118, 148),  995, UERandom.Range(-47, -65)),
                    new Vector3(UERandom.Range( 83, 118),  995, UERandom.Range(-47, -65)),
                    new Vector3(UERandom.Range( 13,  15),  995, UERandom.Range(-18, -48)),
                    new Vector3(UERandom.Range( 84,  88),  988, UERandom.Range(-67, -70)),
                    new Vector3(UERandom.Range( 68,  83),  988, UERandom.Range(-52, -66)),
                    new Vector3(UERandom.Range( 53,  68),  988, UERandom.Range(-53, -63)),
                    new Vector3(UERandom.Range( 12,  49),  988, UERandom.Range(-47, -66)),
                    new Vector3(UERandom.Range( 38,  42),  988, UERandom.Range(-40, -47)),
                    new Vector3(UERandom.Range( 38,  43),  988, UERandom.Range(-32, -38)),
                    new Vector3(UERandom.Range(-25,  12),  988, UERandom.Range(-50, -66)),
                    new Vector3(UERandom.Range(-26, -56),  988, UERandom.Range(-50, -66)),
                    new Vector3(UERandom.Range( -3, -24), 1001, UERandom.Range(-66, -73)),
                    new Vector3(UERandom.Range(  5,  28), 1001, UERandom.Range(-66, -73)),
                    new Vector3(UERandom.Range( 29,  55), 1001, UERandom.Range(-66, -73)),
                    new Vector3(UERandom.Range( 50,  54), 1001, UERandom.Range(-49, -66)),
                    new Vector3(UERandom.Range( 24,  48), 1001, UERandom.Range(-41, -46)),
                    new Vector3(UERandom.Range(  5,  24), 1001, UERandom.Range(-41, -46)),
                    new Vector3(UERandom.Range( -4, -17), 1001, UERandom.Range(-41, -46)),
                    new Vector3(UERandom.Range(  4,  -4), 1001, UERandom.Range(-25, -40)),
                    new Vector3(UERandom.Range( 11, -11), 1001, UERandom.Range(-18, -21)),
                    new Vector3(UERandom.Range(  3,  -3), 1001, UERandom.Range( -4, -17)),
                    new Vector3(UERandom.Range(  2,  14), 1001, UERandom.Range(  3,  -3)),
                    new Vector3(UERandom.Range( -1, -13), 1001, UERandom.Range(  4,  -3)),
                };
            }
        }

        public IEnumerator<float> AirBomb(int waitforready, int limit)
        {
            if (isAirBombCurrently)
                yield break;
            else isAirBombCurrently = true;

            var OutsideRoom = Server.Get.Map.GetRoom(MapGeneration.RoomName.Outside);

            Map.Get.Cassie("danger . outside zone emergency termination sequence activated .", false);
            yield return MEC.Timing.WaitForSeconds(5f);

            while (waitforready > 0)
            {
                Map.Get.PlayAmbientSound(7);
                OutsideRoom.ChangeRoomLightColor(new Color(0.5f, 0, 0));
                yield return MEC.Timing.WaitForSeconds(0.5f);
                OutsideRoom.ChangeRoomLightColor(new Color(1, 0, 0));
                yield return MEC.Timing.WaitForSeconds(0.5f);
                waitforready--;
            }

            int throwcount = 0;
            while (isAirBombCurrently)
            {
                List<Vector3> randampos = AirbombPos.OrderBy(x => Guid.NewGuid()).ToList();
                foreach (var pos in randampos)
                {
                    Map.Get.SpawnGrenade(pos, Vector3.zero, 0.1f);
                    yield return MEC.Timing.WaitForSeconds(0.1f);
                }
                throwcount++;
                if (limit != -1 && limit <= throwcount)
                {
                    isAirBombCurrently = false;
                    break;
                }
                yield return MEC.Timing.WaitForSeconds(0.25f);
            }
            OutsideRoom.ChangeRoomLightColor(new Color(1, 0, 0), false);
            Map.Get.Cassie("outside zone termination completed .", false);
            yield break;
        }



        public Player GetPlayercoprs(Player player, float rayon)
        {
            var ragdolls = Map.Get.Ragdolls.Where(r => Vector3.Distance(r.GameObject.transform.position, player.Position) < rayon).ToList();
            
            ragdolls.Sort(
                (Synapse.Api.Ragdoll x, Synapse.Api.Ragdoll y) 
                => Vector3.Distance(x.GameObject.transform.position, player.Position).CompareTo(Vector3.Distance(y.GameObject.transform.position, player.Position)));
            
            if (ragdolls.Count == 0)
                return null;
            
            Player owner = ragdolls.First().Owner;
            
            if (owner != null && owner.RoleID == (int)RoleType.Spectator)
                return owner;
            else return null;
        }

        public int GetVoltage()
        {
            float totalvoltagefloat = 0;
            
            foreach (var generator in Map.Get.Generators)
                totalvoltagefloat += generator.generator._currentTime / generator.generator._totalActivationTime * 1000;
            
            return (int)totalvoltagefloat;
        }

        public void StartAirBombardement(int waitforready = 10, int limit = 5)
            => MEC.Timing.RunCoroutine(MapActionManager.Get.AirBomb(waitforready, limit));

        public void PlayAmbientSound(int id)
            => Server.Get.Host.GetComponent<AmbientSoundPlayer>().RpcPlaySound(id);

        public void StopAirBombardement()
            => MapActionManager.Get.isAirBombCurrently = false;

        public void ChangeRoomsLightColor(Color color)
        {
            foreach (Room room in Map.Get.Rooms)
                room.ChangeRoomLightColor(color);
        }

        public void ResetRoomsLightColor()
        {
            foreach (Room room in Map.Get.Rooms)
                room.ResetRoomLightColor();
        }
    }
}