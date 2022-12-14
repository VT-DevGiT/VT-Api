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
using VT_Api.Core.Command;
using VT_Api.Core.Roles;
using VT_Api.Core.Teams;
using VT_Api.Core.Items;

using EventHandler = VT_Api.Core.Events.EventHandler;
using VT_Api.Core.Behaviour;
using VT_Api.Core.NPC;
using VT_Api.Core.Audio;
using VT_Api.Core.Translation;

public class VtController
{
    #region Properties & Variable
    public static VtController Get { get; private set; }

    internal AutoRegisterManager AutoRegister { get => Singleton<AutoRegisterManager>.Instance; } // nothing  public (yet)
    public AudioManager Audio { get => Singleton<AudioManager>.Instance; }
    internal MiniGameManager MinGames { get => Singleton<MiniGameManager>.Instance; } // not finish
    public RoleManager Role { get => Singleton<RoleManager>.Instance; }
    public TeamManager Team { get => Singleton<TeamManager>.Instance; }
    public EventHandler Events { get => Singleton<EventHandler>.Instance; }
    public MapAndRoundManger MapAction { get => Singleton<MapAndRoundManger>.Instance; }
    public NetworkLiar NetworkLiar { get => Singleton<NetworkLiar>.Instance; }
    public ItemManager Item { get => Singleton<ItemManager>.Instance; }
    public TranslationManager Translation { get => Singleton<TranslationManager>.Instance; }
    internal NpcManger Npc { get => Singleton<NpcManger>.Instance; } // not finish
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

        if (VtVersion.Debug == true)
            Harmony.DEBUG = true;

        _enabled = true;

        Get = new VtController();

        VtController.Get.LogMessage();
        VtController.Get.AplidePatch();
        VtController.Get.InitAll();
        VtController.Get.CheckUpdate();

        Logger.Get.Info("Vt-API is now ready!");
    }

    private void CheckUpdate()
    {
        if (!Configs.VtConfiguration.AutoUpdate)
            return;

        var updater = new Updater();
        var isUpdate = updater.Update();
        
        if (isUpdate)
            Logger.Get.Warn("The VT-API is update on the last version !");
    }

    private void LogMessage()
    {
        ServerConsole.AddLog("Vt-API Initialising!", System.ConsoleColor.Yellow);

        if (VtVersion.Debug)
            Logger.Get.Warn("Debug Version of Vt-Api loaded! This Version should only be used for testing and not playing");

        if (VtVersion.BasedGameVersion != GameCore.Version.VersionString)
            Logger.Get.Warn("Vt-Version : Different Game Version than expected. Bugs may occurre");
    }

    private void InitAll()
    {
        TryInit(AutoRegister.Init, "Initialising AutoRegister failed");
        TryInit(Audio.Init, "Initialising Audio failed");
        TryInit(MinGames.Init, "Initialising MinGames failed");
        TryInit(Events.Init, "Initialising Events failed");
        TryInit(Commands.Init, "Initialising Commands failed");
        TryInit(Configs.Init, "Initialising Configs failed");
        TryInit(Team.Init, "Initialising Team failed");
        TryInit(Role.Init, "Initialising Role failed");
        TryInit(Item.Init, "Initialising Item failed");
        TryInit(Translation.Init, "Initialising Translation failed");
        TryInit(Npc.Init, "Initialising Npc failed");
    }

    private void TryInit(Action init, string msg)
    {
        try
        {
            init();
        }
        catch (Exception ex)
        {
            Logger.Get.Error(msg + "\n" + ex);
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