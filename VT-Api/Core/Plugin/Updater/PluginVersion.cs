using System.Text.RegularExpressions;

namespace VT_Api.Core.Plugin.Updater
{
    public struct PluginVersion
    {
        #region Properties & Variable
        public const string DefaultsRegexVersion = @"[v,V]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)";

        int Major;
        int Minor;
        int Patch;
        #endregion

        #region Constructor & Destructor
        public PluginVersion(int major, int minor, int patch = 0)
        {
            Major = major; 
            Minor = minor; 
            Patch = patch; 
        }
        public PluginVersion(string version, string expression = DefaultsRegexVersion)
        {
            var pattern = new Regex(expression);
            var match = pattern.Match(version);
            Major = int.Parse(match.Groups["major"].Value);
            Minor = int.Parse(match.Groups["minor"].Value);
            Patch = int.Parse(match.Groups["patch"].Value);
        }
        #endregion

        #region Methods
        public static PluginVersion Parse(string s, string expression = DefaultsRegexVersion)
        {
            var pattern = new Regex(expression);
            var match = pattern.Match(s);
            return new PluginVersion(int.Parse(match.Groups["major"].Value), int.Parse(match.Groups["minor"].Value), int.Parse(match.Groups["patch"].Value));
        }

        public static bool TryParse(string s, out PluginVersion version, string expression = DefaultsRegexVersion)
        {
            var pattern = new Regex(expression);
            var match = pattern.Match(s);
            if (!match.Success)
            {
                version = new PluginVersion();
                return false;
            }

            if (!int.TryParse(match.Groups["major"].Value, out int major))
            {
                version = new PluginVersion();
                return false;
            }

            if (!int.TryParse(match.Groups["minor"].Value, out int minor))
            {
                version = new PluginVersion();
                return false;
            }

            if (!int.TryParse(match.Groups["patch"].Value, out int patch))
            {
                version = new PluginVersion();
                return false;
            }

            version = new PluginVersion(major, minor, patch);
            return true;
        }

        public override int GetHashCode()
        {
            int h = 23;
            h = h * 31 + Major;
            h = h * 31 + Minor;
            h = h * 31 + Patch;
            return h;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is string s)
            {
                if (TryParse(s, out var version))
                    return version == this;
                else
                    return false;
            }
            else if (obj is PluginVersion version)
                return version == this;
            return false;
        }

        public override string ToString()
            => $"{Major}.{Minor}.{Patch}";
        
        #endregion

        #region operator
        public static bool operator >(PluginVersion a, PluginVersion b)
        {
            if (a.Major > b.Major)
                return true;
            else if (a.Major < b.Major)
                return false;
            if (a.Minor > b.Minor)
                return true;
            else if(a.Major < b.Major)
                return false;
            if (a.Patch > b.Patch)
                return true;
            return false;
        }

        public static bool operator <(PluginVersion a, PluginVersion b)
        {
            if (a.Major < b.Major)
                return true;
            else if (a.Major > b.Major)
                return false;
            if (a.Minor < b.Minor)
                return true;
            else if (a.Major > b.Major)
                return false;
            if (a.Patch < b.Patch)
                return true;
            return false;
        }

        public static bool operator ==(PluginVersion a, PluginVersion b)
            => a.Patch == b.Patch && a.Minor == b.Minor && a.Major == b.Major;

        public static bool operator !=(PluginVersion a, PluginVersion b)
            => a.Patch != b.Patch || a.Minor != b.Minor || a.Major != b.Major;
        #endregion
    }
}
