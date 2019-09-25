// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NodaTime.Test.TimeZones.IO
{
    public class TzdbStreamDataTest
    {
        [Test]
        public void Minimal()
        {
            var data = new TzdbStreamData(CreateMinimalBuilder());
            Assert.NotNull(data.TzdbVersion);
        }

        [Test]
        public void MissingStringPool()
        {
            var builder = CreateMinimalBuilder();
            builder.stringPool = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void MissingTzbdAliasMap()
        {
            var builder = CreateMinimalBuilder();
            builder.tzdbIdMap = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void MissingTzdbVersion()
        {
            var builder = CreateMinimalBuilder();
            builder.tzdbVersion = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void MissingWindowsMapping()
        {
            var builder = CreateMinimalBuilder();
            builder.windowsMapping = null;
            Assert.Throws<InvalidNodaDataException>(() => new TzdbStreamData(builder));
        }

        [Test]
        public void InvalidVersion()
        {
            // It's hard to create a stream that's valid apart from the version, so we'll just
            // give one with an invalid version and check that it looks like the right message.
            var stream = new MemoryStream(new byte[] { 0, 0, 0, 1 });
            var exception = Assert.Throws<InvalidNodaDataException>(() => TzdbStreamData.FromStream(stream));
            Assert.IsTrue(exception.Message.Contains("version"));
        }

        [Test]
        [TestCase(TzdbStreamFieldId.TimeZone, nameof(TzdbStreamData.Builder.HandleZoneField))]
        [TestCase(TzdbStreamFieldId.Zone1970Locations, nameof(TzdbStreamData.Builder.HandleZone1970LocationsField))]
        [TestCase(TzdbStreamFieldId.ZoneLocations, nameof(TzdbStreamData.Builder.HandleZoneLocationsField))]
        public void MissingStringPool(object fieldIdObject, string handlerMethodName)
        {
            var fieldId = (TzdbStreamFieldId) fieldIdObject;
            var field = new TzdbStreamField(fieldId, new byte[1]);
            var method = typeof(TzdbStreamData.Builder).GetMethod(handlerMethodName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new Exception($"Can't find method {handlerMethodName}");
            var builder = new TzdbStreamData.Builder();
            
            var exception = Assert.Throws<TargetInvocationException>(() => method.Invoke(builder, new object[] { field }));
            Assert.IsInstanceOf<InvalidNodaDataException>(exception.InnerException);
        }

        // Note: we don't test this with a CldrSupplementalWindowsZones field, as they're both awkward to build.
        // All other fields are valid with a single byte of 0, which is usually a count. This is very convenient.
        [Test]
        [TestCase(TzdbStreamFieldId.StringPool, nameof(TzdbStreamData.Builder.HandleStringPoolField))]
        [TestCase(TzdbStreamFieldId.TzdbIdMap, nameof(TzdbStreamData.Builder.HandleTzdbIdMapField))]
        [TestCase(TzdbStreamFieldId.TzdbVersion, nameof(TzdbStreamData.Builder.HandleTzdbVersionField))]
        [TestCase(TzdbStreamFieldId.Zone1970Locations, nameof(TzdbStreamData.Builder.HandleZone1970LocationsField))]
        [TestCase(TzdbStreamFieldId.ZoneLocations, nameof(TzdbStreamData.Builder.HandleZoneLocationsField))]
        public void DuplicateField(object fieldIdObject, string handlerMethodName)
        {
            var fieldId = (TzdbStreamFieldId) fieldIdObject;
            
            var field = new TzdbStreamField(fieldId, new byte[1]);
            var method = typeof(TzdbStreamData.Builder).GetMethod(handlerMethodName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new Exception($"Can't find handler method {handlerMethodName}");
            var builder = new TzdbStreamData.Builder();
            // Provide an empty string pool if we're not checking for a duplicate string pool.
            if (fieldId != TzdbStreamFieldId.StringPool)
            {
                builder.stringPool = new List<string>();
            }

            // First call should be okay
            method.Invoke(builder, new object[] { field });
            
            // Second call should throw
            var exception = Assert.Throws<TargetInvocationException>(() => method.Invoke(builder, new object[] { field }));
            Assert.IsInstanceOf<InvalidNodaDataException>(exception.InnerException);
        }

        [Test]
        public void DuplicateTimeZoneField()
        {
            // This isn't really a valid field, but we don't parse the data yet anyway - it's
            // enough to give the ID.
            var zoneField = new TzdbStreamField(TzdbStreamFieldId.StringPool, new byte[1]);

            var builder = new TzdbStreamData.Builder();
            builder.stringPool = new List<string> { "zone1" };
            builder.HandleZoneField(zoneField);
            Assert.Throws<InvalidNodaDataException>(() => builder.HandleZoneField(zoneField));
        }

        private static TzdbStreamData.Builder CreateMinimalBuilder() =>
            new TzdbStreamData.Builder
            {
                stringPool = new List<string>(),
                tzdbIdMap = new Dictionary<string, string>(),
                tzdbVersion = "tzdb-version",
                windowsMapping = new WindowsZones("cldr-version", "tzdb-version", "windows-version", new MapZone[0]),
            };
    }
}
