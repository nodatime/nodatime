// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    ///   Represents an alias link between a target (existing item) and a source (the alias) time
    ///   zone.
    /// </summary>
    /// <remarks>
    ///   Immutable, thread-safe.
    /// </remarks>
    internal class ZoneAlias : IEquatable<ZoneAlias>
    {
        private readonly string alias;
        private readonly string existing;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZoneAlias" /> class.
        /// </summary>
        /// <param name="existing">The existing zone name.</param>
        /// <param name="alias">The alias zone name.</param>
        internal ZoneAlias(string existing, string alias)
        {
            this.existing = existing;
            this.alias = alias;
        }

        /// <summary>
        ///   Gets or sets the time zone alias name.
        /// </summary>
        /// <value>The alias name.</value>
        internal string Alias
        {
            get { return alias; }
        }

        /// <summary>
        ///   Gets or sets the existing time zone name.
        /// </summary>
        /// <value>The existing name.</value>
        internal string Existing
        {
            get { return existing; }
        }

        #region IEquatable<ZoneAlias> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter;
        ///   otherwise, false.
        /// </returns>
        public bool Equals(ZoneAlias other)
        {
            if (other == null)
            {
                return false;
            }
            return Existing == other.Existing && Alias == other.Alias;
        }
        #endregion

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ZoneAlias);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data
        ///   structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Existing);
            hash = HashCodeHelper.Hash(hash, Alias);
            return hash;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Alias + " --> " + Existing;
        }
    }
}
