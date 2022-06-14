using System;
using System.Runtime.Serialization;
using Utf8Json;

namespace VT_Api.Core.Plugin.Updater
{
    public class Release
    {
        [DataMember(Name = "id")] 
        public readonly int Id;
        
        [DataMember(Name = "tag_name")] 
        public readonly string TagName;
        
        [DataMember(Name = "prerelease")] 
        public readonly bool PreRelease;
        
        [DataMember(Name = "created_at")] 
        public readonly DateTime CreatedAt;
        
        [DataMember(Name = "assets")] 
        public readonly ReleaseAsset[] Assets;

        [SerializationConstructor]
        public Release(int id, string tag_name, bool prerelease, DateTime created_at, ReleaseAsset[] assets)
        {
            Id = id;
            TagName = tag_name;
            PreRelease = prerelease;
            CreatedAt = created_at;
            Assets = assets;
        }
    }
}
