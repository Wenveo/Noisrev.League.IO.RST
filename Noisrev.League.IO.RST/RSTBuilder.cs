/*
 * Copyright (c) Noisrev, GZSkins, Inc.
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

using Noisrev.League.IO.RST.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// Used to build <see cref="RSTFile"/>. This class cannot be inherited.
    /// </summary>
    public sealed class RSTBuilder
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
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public string this[ulong key]
        {
            get => Current.Entries[key];
            set => Current.Entries[key] = value;
        }

        /// <summary>
        /// Current RST file.
        /// </summary>
        public RSTFile Current { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSTBuilder"/> class.
        /// </summary>
        public RSTBuilder()
        {
            Current = new RSTFile(RVersionHelper.GetLatestVersion());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSTBuilder"/> class using the specified <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="input">The <see cref="RSTFile"/> used to initialize the value of the instance.</param>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        public RSTBuilder(RSTFile input)
        {
            Current = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, string value)
        {
            Current.Entries.Add(key, value);
            return this;
        }


        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, bool value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, byte value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, sbyte value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, char value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, short value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, int value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, long value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, float value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, double value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, decimal value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, ushort value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, uint value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, ulong value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
            return Add(key, value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>A reference to this instance after the add operation has completed.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public RSTBuilder Add(ulong key, object value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);

            if (null == value)
            {
                return this;
            }
            return Add(key, value.ToString());
        }

        /// <summary>
        /// Build the Current RST file.
        /// </summary>
        /// <param name="fileLocation">Output path of the RST file.</param>
        /// <returns>A reference to this instance after the operation is complete.</returns>
        /// <exception cref="ArgumentNullException">fileLocation is null.</exception>
        public RSTBuilder Build(string fileLocation)
        {
            Current.Write(fileLocation);
            return this;
        }

        /// <summary>
        /// Build the Current RST file.
        /// </summary>
        /// <param name="outputStream">The stream used to output RST file.</param>
        /// <param name="leaveOpen">true to leave the stream open after the <see cref="BinaryWriter"/> object is disposed; otherwise, false.</param>
        /// <returns>A reference to this instance after the operation is complete.</returns>
        /// <exception cref="ArgumentException">The outputStream does not support writes.</exception>
        /// <exception cref="ArgumentNullException">outputStream is null.</exception>
        public RSTBuilder Build(Stream outputStream, bool leaveOpen)
        {
            Current.Write(outputStream, leaveOpen);
            return this;
        }

        /// <summary>
        /// Removes all elements from the <see cref="RSTFile"/>.
        /// </summary>
        /// <returns>A reference to this instance after the operation is complete.</returns>
        public RSTBuilder Clear()
        {
            Current.Entries.Clear();
            return this;
        }

        /// <summary>
        /// Determines whether the <see cref="RSTFile"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="RSTFile"/>.</param>
        /// <returns>true if the <see cref="RSTFile"/> contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public bool ContainsKey(ulong key)
        {
            return Current.Entries.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="RSTFile"/> contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="RSTFile"/>.</param>
        /// <returns>true if the <see cref="RSTFile"/> contains an element with the specified value; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public bool ContainsValue(string value)
        {
            return Current.Entries.ContainsValue(value);
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the <see cref="RSTFile"/>.</returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public bool Remove(ulong key)
        {
            return Current.Entries.Remove(key);
        }

        /// <summary>
        /// Replaces all matched elements in the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="oldText">The string to be replaced.</param>
        /// <param name="newText">Replace with a new string.</param>
        /// <param name="caseSensitive">Whether to enable case-sensitive comparison. Compares lowercase strings by default.</param>
        /// <returns>A reference to this instance after the operation is complete.</returns>
        /// <exception cref="ArgumentNullException"/>
        public RSTBuilder ReplaceAll(string oldText, string newText, bool caseSensitive = false)
        {
            if (oldText == null)
                throw new ArgumentNullException(nameof(oldText));
            if (newText == null)
                return this;

            // Set a list
            var list = caseSensitive
                ? Current.Entries.Where(x => x.Value.Contains(oldText))
                : Current.Entries.Where(x => x.Value.ToLower().Contains(oldText.ToLower()));

            foreach (var item in list)
            {
                // Set Text
                Current.Entries[item.Key] = newText;
            }
            return this;
        }

        /// <summary>
        /// Set the config of <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="conf">The config.</param>
        /// <returns>It must be version 2 to set the config. Set to return true on success or false on failure.</returns>
        public bool SetConfig(string conf)
        {
            // Version 2
            if (Current.Version == RVersion.Ver2)
            {
                // Set the config
                Current.Config = conf;
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public bool TryAdd(ulong key, string value)
        {
            Contract.Ensures(Contract.Result<RSTBuilder>() != null);
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
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// Adds the specified key and value to the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>True if the element with the specified key was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="RSTFile"/>.</exception>
        /// <exception cref="ArgumentNullException">key is null.</exception>
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
        /// <returns>true if the <see cref="RSTFile"/> contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(ulong key, out string value)
        {
            return Current.Entries.TryGetValue(key, out value);
        }
    }
}
