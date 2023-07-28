// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Noisrev.League.IO.RST;

/// <summary>
/// Used to build <see cref="RSTFile"/>. This class cannot be inherited.
/// </summary>
public sealed class RSTBuilder : IDictionary<ulong, string>
{
    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>
    /// <para>The value associated with the specified key. If the specified key is not found,</para>
    /// <para>a get operation throws a <see cref="KeyNotFoundException"/>, and</para>
    /// <para>a set operation creates a new element with the specified key.</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">The key is null.</exception>
    /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
    public string this[ulong key]
    {
        get => Current.Entries[key];
        set => Current.Entries[key] = value;
    }

    /// <summary>
    /// Current <see cref="RSTFile"/>.
    /// </summary>
    public RSTFile Current { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTBuilder"/> class.
    /// </summary>
    public RSTBuilder()
    {
        Current = new RSTFile(RVersion.Ver5);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTBuilder"/> class using the specified <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="input">The <see cref="RSTFile"/> used to initialize the value of the instance.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="input"/> is null.</exception>
    public RSTBuilder(RSTFile input)
    {
        Current = input ?? throw new ArgumentNullException(nameof(input));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, string value)
    {
        Current.Entries.Add(key, value);
        return this;
    }


    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, bool value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, byte value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, sbyte value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, char value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, short value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, int value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, long value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, float value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, double value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, decimal value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, ushort value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, uint value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, ulong value)
    {
        return Add(key, value.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>A reference to this instance after the add operation has completed.</returns>
    /// <exception cref="ArgumentException">An element with the same key already exists in the current <see cref="RSTFile"/>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public RSTBuilder Add(ulong key, object? value)
    {
        if (null == value)
        {
            return this;
        }

        return Add(key, value.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Build the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="path">The path used to save the current <see cref="RSTFile"/>.</param>
    /// <returns>A reference to this instance after the operation is complete.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null.</exception>
    public RSTBuilder Build(string path)
    {
        Current.Write(path);
        return this;
    }

    /// <summary>
    /// Build the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="outputStream">The stream used to output <see cref="RSTFile"/>.</param>
    /// <param name="leaveOpen">true to leave the stream open after the <see cref="BinaryWriter"/> object is disposed; otherwise, false.</param>
    /// <returns>A reference to this instance after the operation is complete.</returns>
    /// <exception cref="ArgumentException">The <paramref name="outputStream"/> does not support writes.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="outputStream"/> is null.</exception>
    public RSTBuilder Build(Stream outputStream, bool leaveOpen)
    {
        Current.Write(outputStream, leaveOpen);
        return this;
    }

    /// <summary>
    /// Removes all elements in the current <see cref="RSTFile"/>.
    /// </summary>
    /// <returns>A reference to this instance after the operation is complete.</returns>
    public RSTBuilder Clear()
    {
        Current.Entries.Clear();
        return this;
    }

    /// <summary>
    /// Determines whether the current <see cref="RSTFile"/> contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the <see cref="RSTFile"/>.</param>
    /// <returns>True if the current <see cref="RSTFile"/> contains an element with the specified key; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool ContainsKey(ulong key)
    {
        return Current.Entries.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether the current <see cref="RSTFile"/> contains the specified value.
    /// </summary>
    /// <param name="value">The value to locate in the <see cref="RSTFile"/>.</param>
    /// <returns>True if the <see cref="RSTFile"/> contains an element with the specified value; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
    public bool ContainsValue(string value)
    {
        return Current.Entries.ContainsValue(value);
    }

    /// <summary>
    /// Removes the value with the specified key in the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="RSTFile"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool Remove(ulong key)
    {
        return Current.Entries.Remove(key);
    }

    /// <summary>
    /// Replaces all matched elements in the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="oldText">The string to be replaced.</param>
    /// <param name="newText">Replace with a new string.</param>
    /// <returns>A reference to this instance after the operation is complete.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="oldText"/> or <paramref name="newText"/> is null.</exception>
    public RSTBuilder ReplaceAll(string oldText, string newText)
    {
#if NET7_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(oldText);
#else
        if (oldText == null)
        {
            throw new ArgumentNullException(nameof(oldText), "The value cannot be an empty string.");
        }
#endif
        if (newText == null)
        {
            return this;
        }

        foreach (var item in Current.Entries.Where(x => x.Value.Contains(oldText)).ToArray())
        {
            Current.Entries[item.Key] = item.Value.Replace(oldText, newText);
        }

        return this;
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    /// <summary>
    /// Replaces all matched texts in the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="oldText">The string to replaced.</param>
    /// <param name="newText">Replace with a new string.</param>
    /// <param name="caseSensitive">Whether to enable case-sensitive comparison. Compares lowercase strings by default.</param>
    /// <returns>A reference to this instance after the operation is complete.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="oldText"/> or <paramref name="newText"/> is null.</exception>
    public RSTBuilder ReplaceAll(string oldText, string newText, bool caseSensitive = false)
    {
#if NET7_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(oldText);
#else
        if (oldText == null)
        {
            throw new ArgumentNullException(nameof(oldText), "The value cannot be an empty string.");
        }
#endif
        if (newText == null)
        {
            return this;
        }

        if (caseSensitive)
        {
            foreach (var item in Current.Entries.Where(x => x.Value.Contains(oldText)).ToArray())
            {
                Current.Entries[item.Key] = item.Value.Replace(oldText, newText, StringComparison.CurrentCulture);
            }
        }
        else
        {
            foreach (var item in Current.Entries.Where(x => x.Value.IndexOf(oldText, 0, x.Value.Length, StringComparison.CurrentCultureIgnoreCase) >= 0).ToArray())
            {
                Current.Entries[item.Key] = item.Value.Replace(oldText, newText, StringComparison.CurrentCulture);
            }
        }

        return this;
    }
#endif

    /// <summary>
    /// Sets the font config for the current <see cref="RSTFile"/>. Only supported in <see cref="RVersion.Ver2"/>.
    /// </summary>
    /// <param name="fontConfig">The font config to set.</param>
    /// <returns>True if the font config was successfully sets; otherwise, false.</returns>
    public bool SetConfig(string fontConfig)
    {
        // Version 2
        if (Current.Version == RVersion.Ver2)
        {
            // Set the config
            Current.Config = fontConfig;
            // Return
            return true;
        }
        // Not version 2
        else
        {
            // Return
            return false;
        }
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, string value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, bool value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, byte value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, sbyte value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, char value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, short value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, int value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, long value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, float value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, double value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, decimal value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, ushort value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, uint value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the current <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, ulong value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds the specified key and value to the <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
    public bool TryAdd(ulong key, object value)
    {
        if (!ContainsKey(key))
        {
            Add(key, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// <para>When this method returns, contains the value associated with the specified key,</para>
    /// <para>if the key is found; otherwise, the default value for the type of the value parameter.</para>
    /// </param>
    /// <returns>True if the current <see cref="RSTFile"/> contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(ulong key, [NotNullWhen(true)] out string? value)
    {
        return Current.Entries.TryGetValue(key, out value);
    }

    ICollection<ulong> IDictionary<ulong, string>.Keys
    {
        get
        {
            return Current.Entries.Keys;
        }
    }

    ICollection<string> IDictionary<ulong, string>.Values
    {
        get
        {
            return Current.Entries.Values;
        }
    }

    int ICollection<KeyValuePair<ulong, string>>.Count
    {
        get
        {
            return ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).Count;
        }
    }

    bool ICollection<KeyValuePair<ulong, string>>.IsReadOnly
    {
        get
        {
            return ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).IsReadOnly;
        }
    }

    void IDictionary<ulong, string>.Add(ulong key, string value)
    {
        Current.Entries.Add(key, value);
    }

    void ICollection<KeyValuePair<ulong, string>>.Add(KeyValuePair<ulong, string> item)
    {
        ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).Add(item);
    }

    void ICollection<KeyValuePair<ulong, string>>.Clear()
    {
        ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).Clear();
    }

    bool ICollection<KeyValuePair<ulong, string>>.Contains(KeyValuePair<ulong, string> item)
    {
        return ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).Contains(item);
    }

    void ICollection<KeyValuePair<ulong, string>>.CopyTo(KeyValuePair<ulong, string>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<ulong, string>>.Remove(KeyValuePair<ulong, string> item)
    {
        return ((ICollection<KeyValuePair<ulong, string>>)Current.Entries).Remove(item);
    }

#if !NETCOREAPP
    bool IDictionary<ulong, string>.TryGetValue(ulong key, out string value)
    {
        return Current.Entries.TryGetValue(key, out value);
    }
#endif

    IEnumerator<KeyValuePair<ulong, string>> IEnumerable<KeyValuePair<ulong, string>>.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<ulong, string>>)Current.Entries).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Current.Entries).GetEnumerator();
    }
}
