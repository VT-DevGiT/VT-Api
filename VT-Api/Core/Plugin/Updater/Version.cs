using System.Text.RegularExpressions;

namespace VT_Api.Core.Plugin.Updater
{
    public struct Version
    {
        #region Properties & Variable
        int Major;
        int Minor;
        int Patch;
        #endregion

        #region Constructor & Destructor
        public Version(int major, int minor, int patch = 0)
        {
            Major = major; 
            Minor = minor; 
            Patch = patch; 
        }
        public Version(string version, string expression = @"[v,V]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+))")
        {
            var pattern = new Regex(expression);
            var match = pattern.Match(version);
            Major = int.Parse(match.Groups["major"].Value);
            Minor = int.Parse(match.Groups["minor"].Value);
            Patch = int.Parse(match.Groups["patch"].Value);
        }
        #endregion

        #region Methods
        public static Version Parse(string s, string expression = @"[v,V]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+))")
        {
            var pattern = new Regex(expression);
            var match = pattern.Match(s);
            return new Version(int.Parse(match.Groups["major"].Value), int.Parse(match.Groups["minor"].Value), int.Parse(match.Groups["patch"].Value));
        }

        public static bool TryParse(string s, out Version version, string expression = @"[v,V]?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+))")
        {
            var pattern = new Regex(expression);
            var match = pattern.Match(s);
            if (!match.Success)
            {
                version = new Version();
                return false;
            }

            if (!int.TryParse(match.Groups["major"].Value, out int major))
            {
                version = new Version();
                return false;
            }

            if (!int.TryParse(match.Groups["minor"].Value, out int minor))
            {
                version = new Version();
                return false;
            }

            if (!int.TryParse(match.Groups["patch"].Value, out int patch))
            {
                version = new Version();
                return false;
            }

            version = new Version(major, minor, patch);
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
            else if (obj is Version version)
                return version == this;
            return false;
        }
        #endregion

        #region operator
        public static bool operator >(Version a, Version b)
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

        public static bool operator <(Version a, Version b)
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

        public static bool operator ==(Version a, Version b)
            => a.Patch == b.Patch && a.Minor == b.Minor && a.Major == b.Major;

        public static bool operator !=(Version a, Version b)
            => a.Patch != b.Patch || a.Minor != b.Minor || a.Major != b.Major;
        #endregion
    }
}
