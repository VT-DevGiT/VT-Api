﻿// copy past of SynapseVersion

using VT_Api.Core.Plugin.Updater;

public static class VtVersion
{
    public const int Major = 1;

    public const int Minor = 3;

    public const int Patch = 0;

    public const SynapseVersion.VersionType Type =

#if DEBUG
        SynapseVersion.VersionType.Dev;
#elif PRERELEASE
        SynapseVersion.VersionType.Pre;
#elif RELEASE
        SynapseVersion.VersionType.None;
#endif

    public const string SubVersion = "";

    public const string BasedGameVersion = SynapseVersion.BasedGameVersion;

    public static bool Debug =

#if DEBUG
    true;
#else
    SynapseVersion.Debug;
#endif

    public static PluginVersion GetVersion()
        => new PluginVersion(Major, Minor, Patch);

    public static string GetVersionName()
    {
        var version = $"{Major}.{Minor}.{Patch}";

        if (Type != SynapseVersion.VersionType.None)
            version = $"-{Type} {SubVersion}";

        if (Debug)
            version += " DEBUG";

        return version;
    }
}