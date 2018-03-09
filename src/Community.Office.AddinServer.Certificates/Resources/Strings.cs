using System.Resources;

namespace Community.Office.AddinServer.Certificates.Resources
{
    /// <summary>Represents a strings resource manager.</summary>
    internal static class Strings
    {
        private static readonly ResourceManager _resourceManager =
            new ResourceManager(typeof(Strings).Namespace + "." + typeof(Strings).Name, typeof(Strings).Assembly);

        /// <summary>Returns the value of the specified string.</summary>
        /// <param name="name">The name of the string to retrieve.</param>
        /// <returns>The value of the resource localized for the caller's current UI culture, or null if name cannot be found in a resource set.</returns>
        public static string GetString(string name)
        {
            return _resourceManager.GetString(name);
        }
    }
}