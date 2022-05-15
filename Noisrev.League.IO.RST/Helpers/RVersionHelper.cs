using System;

namespace Noisrev.League.IO.RST.Helpers
{
    /// <summary>
    /// RVersion helper class.
    /// </summary>
    public static class RVersionHelper
    {
        /// <summary>
        /// Get Latest Version of RST
        /// </summary>
        /// <returns></returns>
        public static RVersion GetLatestVersion()
        {
            var array = Enum.GetValues(typeof(RVersion));
            return (RVersion)array.GetValue(array.Length - 1);
        }
    }
}
