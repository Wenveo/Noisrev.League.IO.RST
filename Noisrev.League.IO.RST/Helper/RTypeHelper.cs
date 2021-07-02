namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    /// RType extension classes.
    /// </summary>
    public static class RTypeHelper
    {
        /// <summary>
        /// Compute the key used to generate the hash.
        /// </summary>
        /// <param name="type">The RType</param>
        /// <returns></returns>
        public static ulong ComputeKey(this RType type)
        {
            return (1UL << ((int)type)) - 1;
        }
    }
}
