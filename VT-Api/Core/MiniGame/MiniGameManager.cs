using System;
using System.Collections.Generic;
using System.Linq;
using VT_Api.Exceptions;

namespace VT_Api.Core.MiniGame
{
    public class MiniGameManager
    {
        public static MiniGameManager Get => VtController.Get.MinGames;

        internal MiniGameManager() { }

        public List<MiniGameInformation> MiniGames { get; } = new List<MiniGameInformation>();

        internal void Init()
        {
            throw new NotImplementedException();
        }

        public string GetMiniGameName(int id)
        {
            if (!IsIDRegistered(id)) throw new VtMiniGameNotFoundException("A MiniGame was requested that is not registered. Please check your configs and plugins", id);

            return MiniGames.FirstOrDefault(x => x.ID == id).Name;
        }

        public IMiniGame GetMiniGame(string name)
        {
            var miniGameinformation = MiniGames.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if (miniGameinformation == null)
                throw new VtMiniGameNotFoundException("A MiniGame was requested that is not registered. Please check your configs and plugins.", name);

            if (miniGameinformation.MiniGameScript.GetConstructors().Any(x => x.GetParameters().Count() == 1 && x.GetParameters().First().ParameterType == typeof(int)))
                return (IMiniGame)Activator.CreateInstance(miniGameinformation.MiniGameScript, new object[] { miniGameinformation.ID });

            return (IMiniGame)Activator.CreateInstance(miniGameinformation.MiniGameScript);
        }

        public IMiniGame GetMiniGame(int id)
        {
            if (!IsIDRegistered(id)) throw new VtMiniGameNotFoundException("A MiniGame was requested that is not registered. Please check your configs and plugins.", id);

            var miniGameinformation = MiniGames.FirstOrDefault(x => x.ID == id);

            if (miniGameinformation.MiniGameScript.GetConstructors().Any(x => x.GetParameters().Count() == 1 && x.GetParameters().First().ParameterType == typeof(int)))
                return (IMiniGame)Activator.CreateInstance(miniGameinformation.MiniGameScript, new object[] { miniGameinformation.ID });

            return (IMiniGame)Activator.CreateInstance(miniGameinformation.MiniGameScript);
        }

        public void RegisterMiniGame<TMiniGame>() where TMiniGame : IMiniGame
        {
            var miniGame = (TMiniGame)Activator.CreateInstance(typeof(TMiniGame));
            var info = new MiniGameInformation(miniGame.GetMiniGameName(), miniGame.GetMiniGameID(), typeof(TMiniGame));

            if (IsIDRegistered(miniGame.GetMiniGameID())) throw new VtMiniGameAlreadyRegisteredException("A MiniGame was registered with an already registered ID.", info);

            MiniGames.Add(info);
        }

        public void RegisterMiniGame(MiniGameInformation info)
        {
            if (IsIDRegistered(info.ID)) throw new VtMiniGameAlreadyRegisteredException("A MiniGame was registered with an already registered ID", info);

            MiniGames.Add(info);
        }

        public bool IsIDRegistered(int id)
        {
            if (MiniGames.Any(x => x.ID == id)) return true;

            return false;
        }
    }
}
