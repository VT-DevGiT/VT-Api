using MEC;
using Synapse.Api;
using System.IO;
using UnityEngine;
using VT_Api.Extension;

namespace VT_Api.Core.Audio
{
    public class AudioManager
    {
        #region Attributes & Properties
        public static AudioManager Get => Singleton<AudioManager>.Instance;

        private Controller _controller;

        #endregion

        #region Constructors & Destructor
        internal AudioManager() {

            Synapse.Api.Logger.Get.Debug("AudioRun");
        }

        #endregion

        #region Methods

        internal void Init()
        {
            //_controller = new Controller();
        }

        public void Loop(bool enabled)
        {
            _controller.Loop = enabled;
            //Synapse.Api.Logger.Get.Info($"Loop : {_controller.Loop}");
        }

        private void UnmutePlayer(Player player)
        {
            var id = player.Radio.mirrorIgnorancePlayer._playerId;
            _controller.UnMutePlayer(id);
        }

        private void MutePlayer(Player player)
        {
            var id = player.Radio.mirrorIgnorancePlayer._playerId;
            _controller.MutePlayer(id);
        }

        public bool Play(string mpgFilePath)
        {
            if (!File.Exists(mpgFilePath))
            {
                Synapse.Api.Logger.Get.Info($"File not found : {mpgFilePath}");
                return false;
            }

            Timing.RunCoroutine(_controller.PlayFromFile(mpgFilePath));
            //Synapse.Api.Logger.Get.Info("Playing.");
            return true;
        }

        public void Stop()
        {
            _controller.Stop();
            //Synapse.Api.Logger.Get.Info("Stopped.");
        }

        public void Volume(uint volume)
        {
            _controller.Volume = Mathf.Clamp(volume, 0, 100) / 100;
            _controller.RefreshChannels();

            //Synapse.Api.Logger.Get.Info($"Volume set to {volume}.");
        }
        #endregion
    }
}
