
namespace NodaTime
{
    /// <summary>
    /// A system of defining time in terms of years, months, days and so forth.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The most commonly use calendar system in Noda Time is IsoCalendarSystem,
    /// which is used as a default value in many overloaded methods and constructors.
    /// </para>
    /// <para>
    /// A calendar system has no specific time zone; an IChronology represents the union
    /// of a time zone with a calendar system.
    /// </para>
    public interface ICalendarSystem
    {
    }
}
