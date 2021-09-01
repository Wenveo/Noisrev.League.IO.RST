using System;
using System.Collections.ObjectModel;

namespace Noisrev.League.IO.RST.Intefaces
{
    public interface IRST : IAsyncDisposable, IDisposable
    {
        string Magic { get; }
        byte Version { get; }
        string Config { get; }
        long DataOffset { get; }
        RType Type { get; }
        RMode Mode { get; }
        ReadOnlyCollection<RSTEntry> Entries { get; }

        void AddEntry(string key, string value);
        void AddEntry(ulong hash, string value);
        RSTEntry Find(ulong hash);
        void Insert(int index, RSTEntry entry);
        void Remove(ulong hash);
        bool Remove(RSTEntry entry);
        void RemoveAt(int index);
        void ReplaceAll(string oldText, string newText, bool caseSensitive = false);
    }
}