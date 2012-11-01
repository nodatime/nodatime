JodaDump is a utility to dump time zone transitions from Joda Time
in a format which is cmopatible with NodaTime.CheckTimeZones, so that
we can validate that our time zone parsing is functionally equivalent.

You'll need a copy of Joda Time (http://joda-time.sf.net) in order to
run it, although it doesn't matter which version of the time zone
information it has - JodaDump reads it from the tzdb sources anyway.

Compile:
javac -cp joda-time-2.1.jar nodatime\jodadump\JodaDump.java

Run (for example)
java -cp joda-time-2.1.jar;. nodatime.jodadump.JodaDump ..\ZoneInfoCompiler\Data\2012h > jodadump.txt
