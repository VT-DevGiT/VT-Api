﻿using Newtonsoft.Json;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Synapse;
using Synapse.Translation;

namespace VT_Api.Core.Translation
{
    public class TranslationManager
    {

        #region Properties & Variables
        public static TranslationManager Get => Singleton<TranslationManager>.Instance;

        public Dictionary<string, string> PlayersLanguage = new Dictionary<string, string>();
        #endregion

        #region Constructor & Destructor
        internal TranslationManager() { }
        #endregion

        #region Methods
        internal void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Player.PlayerJoinEvent += OnJoin;
            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent += OnRoundRestart;

        }

        //https://stackoverflow.com/questions/4327629/get-user-location-by-ip-address
        public IpInfo GetIpInfo(Player player) => GetIpInfo(player.IpAddress);
        public IpInfo GetIpInfo(string ip)
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/" + ip);
            return JsonConvert.DeserializeObject<IpInfo>(info);
        }

        /// <returns>The country name in English</returns>
        public string GetUserCountry(Player player)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                ipInfo = GetIpInfo(player);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }

        /// <returns>The country name in English</returns>
        public string GetUserCountry(string ip)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                ipInfo = GetIpInfo(ip);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }

        public string GetLanguage(Player player)
        {
            var contry = GetUserCountry(player);
            var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => new RegionInfo(c.Name).EnglishName == contry);
            return cultureInfo.IetfLanguageTag;
        }

        public TPluginTranslation GetTranslation<TPluginTranslation>(SynapseTranslation<TPluginTranslation> translation, Player player) where TPluginTranslation : IPluginTranslation
        {
            if (!PlayersLanguage.TryGetValue(player.NickName, out var language))
            {
                Logger.Get.Error($"The player language of {player.NickName} is not set !");
                return translation.ActiveTranslation;
            }
            return translation[language];
        }
        #endregion

        #region Events
        private void OnJoin(PlayerJoinEventArgs ev)
        {
            if (PlayersLanguage.ContainsKey(ev.Player.NickName))
                return;
            var language = ev.Player.GetData("Language");
            if (language == null)
            {
                language = GetLanguage(ev.Player);
                ev.Player.SetData("Language", GetLanguage(ev.Player));
            }
            PlayersLanguage.Add(ev.Player.NickName, language);
        }

        private void OnRoundRestart()
        {
            foreach(var playerLanguage in PlayersLanguage)
            {
                if (!Server.Get.Players.Any(p => p.NickName == playerLanguage.Key))
                    PlayersLanguage.Remove(playerLanguage.Key);
            }
        }
        #endregion
    }
}
