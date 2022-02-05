// copy past of SynapseController
using HarmonyLib;
using Synapse.Api;
using System;
using VT_Api.Core.Plugin;
using VT_Api.Core.MiniGame;
using VT_Api.Core.Events;
using VT_Api.Core;
using VT_Api.Config;
using VT_Api.Exceptions;

using EventHandler = VT_Api.Core.Events.EventHandler;
using VT_Api.Core.Command;
using VT_Api.Core.Roles;
using VT_Api.Core.Teams;

public class VtController
{
    #region Properties & Variable
    public static VtController Get { get; private set; }

    internal AutoRegisterManager AutoRegister { get => Singleton<AutoRegisterManager>.Instance; } // nothing  public (yet)
    public MiniGameManager MinGames { get => Singleton<MiniGameManager>.Instance; }
    public RoleManager Role { get => Singleton<RoleManager>.Instance; }
    public TeamManager Team { get => Singleton<TeamManager>.Instance; }
    public EventHandler Events { get => Singleton<EventHandler>.Instance; }
    public MapActionManager MapAction { get => Singleton<MapActionManager>.Instance; }
    internal CommandHandler Commands { get => Singleton<CommandHandler>.Instance; } // nothing  public (yet)
    public Config Configs { get => Singleton<Config>.Instance; }

    private static bool _enabled = false;
    #endregion

    #region Constructor & Destructor
    private VtController()
    { 
    
    }
    #endregion

    #region Methods
    public static void InitApi()
    {
        if (_enabled) return;

        _enabled = true;

        Get = new VtController();

        VtController.Get.LogMessage();
        VtController.Get.AplidePatch();
        VtController.Get.InitAll();

        Logger.Get.Info("Vt-API is now ready!");
    }

    private void LogMessage()
    {
        ServerConsole.AddLog("Vt-API Initialising!", System.ConsoleColor.Cyan);

        if (VtVersion.Debug)
            Logger.Get.Warn("Debug Version of Vt-Api loaded! This Version should only be used for testing and not playing");

        if (VtVersion.BasedGameVersion != GameCore.Version.VersionString)
            Logger.Get.Warn("Vt-Version : Different Game Version than expected. Bugs may occurre");
    }

    private void InitAll()
    {
        try
        {
            AutoRegister.Init();
            MinGames.Init();
            Events.Init();
            Commands.Init();
            Configs.Init();
            Team.Init();
            Role.Init();
        }
        catch (Exception e)
        {
            throw new VtInitAllHandlerExceptions($"Vt-init: Error while Initialising the handlers!\n Please fix the Issue and restart your Server!\n{e}\nStackTrace:\n{e.StackTrace}", e);
        }
    }

    private void AplidePatch()
    {
        try
        {
            var instance = new Harmony("Vt_Api.patches");
            instance.PatchAll();
            Logger.Get.Info("Harmony Patching was sucessfully!");
        }
        catch (Exception e)
        {
            throw new VtInitPatchsException($"Vt-init: Harmony Patching threw an error!\n{e}\nStackTrace:\n{e.StackTrace}", e);
        }
    }
#endregion
}