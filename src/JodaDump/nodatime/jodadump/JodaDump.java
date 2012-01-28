package nodatime.jodadump;

import java.io.*;
//File;
//import java.io.IOException;
import java.util.Map;

import org.joda.time.*;
import org.joda.time.format.*;
import org.joda.time.tz.*;

public final class JodaDump {
    
    public static void main(String[] args) throws IOException {
        
        if (args.length < 1 || args.length > 2) {
            System.out.println("Usage: JodaDump <directory> [zone]");
            return;
        }
        File directory = new File(args[0]);
        if (!directory.isDirectory()) {
            System.out.println(directory + " is not a directory");
            return;
        }
        
        File[] files = directory.listFiles();
        
        ZoneInfoCompiler compiler = new ZoneInfoCompiler();
        PrintStream out = System.out;
        System.setOut(new PrintStream(new ByteArrayOutputStream()));
        Map<String, DateTimeZone> zones = compiler.compile(null, files);
        System.setOut(out);

        if (args.length == 2) {
            String id = args[1];
            DateTimeZone zone = zones.get(id);
            if (zone == null) {
                System.out.println("Zone " + id + " does not exist");
            } else {
                dumpZone(id, zone);
            }
        } else {
            for (String id : zones.keySet()) {
                dumpZone(id, zones.get(id));
            }
        }
    }
    
    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormat
        .forPattern("yyyy-MM-dd HH:mm:ss.sss")
        .withZone(DateTimeZone.UTC);
    private static final Instant START =
        new DateTime(1900, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
    private static final Instant END =
        new DateTime(2050, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
    
    private static void dumpZone(String id, DateTimeZone zone) {
        if (!id.equals(zone.getID())) {
            System.out.println(id + " == " + zone.getID());
            System.out.println();
            return;
        }
        
        System.out.println(id);
        long now = START.getMillis();
        while (now < END.getMillis()) {
            int standardOffset = zone.getStandardOffset(now);
            int wallOffset = zone.getOffset(now);
            
            System.out.println(INSTANT_FORMAT.print(now) + "  "
                               + printOffset(standardOffset) + "  "
                               + printOffset(wallOffset - standardOffset) + "  "
                               + zone.getName(now));
            long next = zone.nextTransition(now);
            if (next <= now) {
                break;
            }
            now = next;
        }
        System.out.println();
    }
    
    private static String printOffset(long millis) {
        long seconds = Math.abs(millis) / 1000;
        String sign = millis < 0 ? "-" : "+";
        return String.format("%s%02d:%02d:%02d", sign,
                             seconds / 3600,
                             (seconds / 60) % 60,
                             seconds % 60);
    }
}
