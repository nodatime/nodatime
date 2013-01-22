namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    interface ITzdbWriter
    {
        void Write(TzdbDatabase tzdb, WindowsMapping mapping);
    }
}
