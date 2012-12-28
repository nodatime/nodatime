namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Defines the types of files we can write to.
    /// </summary>
    public enum OutputType
    {
        /// <summary>Generates the output file in ResX format.</summary>
        ResX,
        /// <summary>Generates the output file in Resource format.</summary>
        Resource,
        /// <summary>Generates the output file in custom NodaTime data format.</summary>
        NodaZoneData
    }
}