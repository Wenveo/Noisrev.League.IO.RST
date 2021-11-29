namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    /// RType extension class.
    /// </summary>
    public static class RTypeHelper
    {
        /// <summary>
        /// Compute the key used to generate the hash.
        /// </summary>
        /// <param name="type">The RType</param>
        /// <returns>Returns the computed value.</returns>
        public static ulong ComputeKey(this RType type)
        {
            return (1UL << (int)type) - 1;
        }
        /// <summary>
        /// Gets the specified RType based on version.
        /// </summary>
        /// <param name="version">the version</param>
        /// <returns>Returns an RType, or null, depending on whether it is a valid version</returns>
        public static RType? GetRType(this int version) =>
            ((byte)version).GetRType();
        /// <summary>
        /// Gets the specified RType based on version.
        /// </summary>
        /// <param name="version">the version</param>
        /// <returns>Returns an RType, or null, depending on whether it is a valid version</returns>
        public static RType? GetRType(this byte version)
        {
            /* Version 2 and Version 3 */
            if (version == 2 || version == 3)
                return RType.Complex;
            else if (version == 4 || version == 5) /* Version 4, 5 */
                return RType.Simple;
            else /* Unknown */
                return null;
        }
    }
}
