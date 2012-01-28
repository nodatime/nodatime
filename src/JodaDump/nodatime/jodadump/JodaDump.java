// Copyright 2009-2012 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package nodatime.jodadump;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
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
            // Sort IDs in ordinal fashion
            List<String> ids = new ArrayList<String>(zones.keySet());
            Collections.sort(ids); // compareTo does ordinal comparisons
            for (String id : ids) {
                dumpZone(id, zones.get(id));
            }
        }
    }
    
    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormat
        .forPattern("yyyy-MM-dd HH:mm:ss")
        .withZone(DateTimeZone.UTC);
    private static final Instant START =
        new DateTime(1900, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
    private static final Instant END =
        new DateTime(2050, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
    
    private static void dumpZone(String id, DateTimeZone zone) {
        // TODO: Reinstate this when Noda Time understands aliases better.
        // See issue 32 for more details.
        // if (!id.equals(zone.getID())) {
        //    System.out.println(id + " == " + zone.getID());
        //    System.out.println();
        //    return;
        // }
        System.out.println(id);
        long now = START.getMillis();
        while (now < END.getMillis()) {
            int standardOffset = zone.getStandardOffset(now);
            int wallOffset = zone.getOffset(now);
            
            System.out.printf("%s  %s  %s%n",
                              INSTANT_FORMAT.print(now),
                              printOffset(standardOffset),
                              printOffset(wallOffset - standardOffset));
            // TODO: Output the name when we've got parity in handling
            // of auto-addition for summer/winter in Noda Time.
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
