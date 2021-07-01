using Noisrev.League.IO.RST.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// Riot String Table File
    /// </summary>
    public class RSTFile : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Standard head
        /// </summary>
        public static readonly string Magic = "RST";
        /// <summary>
        /// RST File Font Config, using by RST v2
        /// </summary>
        public string Config { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long DataOffset { get; set; }
        /// <summary>
        /// Hash Algorithm key
        /// </summary>
        public RType Type { get; private set; }
        /// <summary>
        /// Use LazyLoad for items
        /// </summary>
        public bool UseLazyLoad { get; private set; }
        /// <summary>
        /// RST File Version
        /// </summary>
        public global::System.Version Version { get; private set; }
        /// <summary>
        /// Collection of RST entries
        /// </summary>
        public ReadOnlyCollection<RSTEntry> Entries { get; set; }

        private ulong hashKey;
        private List<RSTEntry> entries;

        internal Stream stream = new MemoryStream();
        internal BinaryReader br;

        /// <summary>
        /// Initialize the RSTFile class
        /// </summary>
        public RSTFile()
        {
            this.entries = new List<RSTEntry>();
            this.Entries = entries.AsReadOnly();
        }

        /// <summary>
        /// Initialize and set the version
        /// </summary>
        /// <param name="version">RST Version</param>
        public RSTFile(RType type, Version version) : this()
        {
            this.Type    = type;
            this.Version = version;
        }

        /// <summary>
        /// Read the RST file from the stream
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.BinaryReader object is disposed; otherwise, false.
        /// <param name="useLazyLoad">Sets whether to use LazyLoad</param>
        /// </param>
        public RSTFile(Stream input, bool leaveOpen, bool useLazyLoad) : this()
        {
            // Set LazyLoad
            UseLazyLoad = useLazyLoad;
            // Set Stream
            long org = input.Position;
            input.CopyTo(stream);
            input.Seek(org, SeekOrigin.Begin);
            stream.Seek(0, SeekOrigin.Begin);
            if (!leaveOpen)
            {
                input.Dispose();
            }

            // Init BinaryReader, use UTF-8
            br = new BinaryReader(stream, Encoding.UTF8);

            // Read magic code
            string magic = br.ReadString(3);
            if (magic != Magic)
            {
                // Invalid magic code
                throw new InvalidDataException($"Invalid RST file header: {magic}");
            }

            //Set Version
            byte major =  br.ReadByte();
            if (major == 2 || major == 3)
            {
                // The keys for versions 2 and 3
                Type = RType.Complex;
                if (major == 2)
                {
                    // 0 or 1
                    bool hasConfig = br.ReadBoolean();
                    if (hasConfig) // true
                    {
                        // Config length
                        int length = br.ReadInt32();
                        // Reads the string into the buffer.
                        byte[] buffer = br.ReadBytes(length);

                        // Convert the bytes of the buffer to a UTF-8 string and set it to Config.
                        Config = Encoding.UTF8.GetString(buffer);
                    }
                }
                // Version 3
                // pass
            }
            else if (major == 4)
            {
                // Key for version 4
                Type = RType.Simple;
            }
            else
            {
                // Not equivalent to versions 2, 3, and 4
                throw new InvalidDataException($"Unsupported RST version: {Version}");
            }

            // Set hash key
            hashKey = (1UL << ((int)Type)) - 1;
            // Read Count
            int count = br.ReadInt32();

            // Initialize dictionary.
            // Key is offset, value is hash
            List<KeyValuePair<long, ulong>> pairs = new List<KeyValuePair<long, ulong>>(count);

            for (int i = 0; i < count; i++)
            {
                //Read the hash data
                ulong Hashgroup = br.ReadUInt64();

                // Generate offset
                long offset = Convert.ToInt64(Hashgroup >> ((int)Type));
                // Generate hash
                ulong hash = Hashgroup & hashKey;

                // Add offsets and hashes to the resource dictionary
                pairs.Add(new KeyValuePair<long, ulong>(offset, hash));
            }

            // Read minor
            byte minor = br.ReadByte();

            // Set Version
            Version = new Version(major, minor);

            DataOffset = br.BaseStream.Position;
            foreach (KeyValuePair<long, ulong> keyValuePair in pairs)
            {
                entries.Add(new RSTEntry(this, keyValuePair.Key, keyValuePair.Value));
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (stream as IDisposable)?.Dispose();
            }

            stream = null;
        }
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (stream != null)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }

            stream = null;
        }
    }
}
