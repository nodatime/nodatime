// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using Microsoft.CodeAnalysis;

namespace NodaTime.CodeDiagnostics.Test.Framework
{
    internal class SimplifiedDiagnostic : IEquatable<SimplifiedDiagnostic>
    {
        internal string Id { get; }
        internal FileLocation Location { get; }
        internal string Message { get; }

        internal SimplifiedDiagnostic(string id, FileLocation location, string message)
        {
            this.Id = id;
            this.Location = location;
            this.Message = message;
        }

        public static SimplifiedDiagnostic FromDiagnostic(Diagnostic diagnostic)
        {
            return new SimplifiedDiagnostic(diagnostic.Id,
                FileLocation.FromLocation(diagnostic.Location),
                diagnostic.GetMessage());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SimplifiedDiagnostic);
        }

        public bool Equals(SimplifiedDiagnostic other)
        {
            return other != null &&
                   other.Id == Id &&
                   other.Location.Equals(Location) &&
                   other.Message.Equals(Message);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + Id.GetHashCode();
            hash = hash * 31 + Location.GetHashCode();
            hash = hash * 31 + Message.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} - {2}", Location, Id, Message);
        }
    }
}