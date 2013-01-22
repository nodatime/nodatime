using System.Collections.Generic;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Provides for a list of Zone objects. All of the Zone objects will have the same
    ///   name which is made available on the list.
    /// </summary>
    internal class ZoneList : List<Zone>
    {
        /// <summary>
        ///   Gets the name of the Zone objects in this list.
        /// </summary>
        /// <value>The zone name or null if the list is empty.</value>
        public string Name
        {
            get { return Count > 0 ? this[0].Name : null; }
        }
    }
}
