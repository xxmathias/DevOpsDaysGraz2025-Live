using System;

namespace DevOpsDaysTasks.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class VersionNameAttribute : Attribute
    {
        public string VersionName { get; }
        public VersionNameAttribute(string versionName)
        {
            VersionName = versionName;
        }
    }
}
