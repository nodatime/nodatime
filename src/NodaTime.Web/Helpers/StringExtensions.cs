// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Web.Helpers
{
    public static class StringExtensions
    {
        public static string TruncateGuid(this string guid)
        {
            if (guid == null)
            {
                return null;
            }
            if (guid.Length < 8)
            {
                return guid;
            }
            return guid.Substring(0, 4) + "..." + guid.Substring(guid.Length - 4);
        }

        public static string TruncateCommit(this string commit)
        {
            if (commit == null)
            {
                return null;
            }
            // Github seems to think 7 characters is enough...
            return commit.Substring(0, Math.Min(commit.Length, 7));
        }
    }
}
