// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test.Utility
{
    public class NodaReadOnlyDictionaryTest
    {
        [Test]
        public void MutatingMembers()
        {
            var original = new Dictionary<int, int>();
            IDictionary<int, int> readOnly = new NodaReadOnlyDictionary<int, int>(original);
            Assert.Throws<NotSupportedException>(() => readOnly.Add(10, 10));
            Assert.Throws<NotSupportedException>(() => readOnly.Add(new KeyValuePair<int, int>(10, 10)));
            Assert.Throws<NotSupportedException>(() => readOnly.Remove(10));
            Assert.Throws<NotSupportedException>(() => readOnly.Remove(new KeyValuePair<int, int>(10, 10)));
            Assert.Throws<NotSupportedException>(() => readOnly.Clear());
            Assert.Throws<NotSupportedException>(() => readOnly[15] = 20);
        }

        [Test]
        public void PassThroughMembers()
        {
            var original = new Dictionary<int, int> { { 10, 20 } };
            IDictionary<int, int> readOnly = new NodaReadOnlyDictionary<int, int>(original);
            Assert.AreEqual(20, readOnly[10]);
            Assert.IsTrue(readOnly.TryGetValue(10, out int value));
            Assert.AreEqual(20, value);
            Assert.AreEqual(1, readOnly.Count);
            Assert.IsTrue(readOnly.Contains(new KeyValuePair<int, int>(10, 20)));
            CollectionAssert.AreEqual(original, readOnly);
            CollectionAssert.AreEqual(original.Keys, readOnly.Keys);
            CollectionAssert.AreEqual(original.Values, readOnly.Values);
            var array = new KeyValuePair<int, int>[1];
            readOnly.CopyTo(array, 0);
            Assert.AreEqual(new KeyValuePair<int, int>(10, 20), array[0]);
        }

        [Test]
        public void NonPassThroughMembers()
        {
            var original = new Dictionary<int, int> { { 10, 20 } };
            IDictionary<int, int> readOnly = new NodaReadOnlyDictionary<int, int>(original);
            Assert.IsTrue(readOnly.IsReadOnly);
        }
    }
}
