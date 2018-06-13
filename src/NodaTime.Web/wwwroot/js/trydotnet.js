// TODO: Create a trydotnet object to call functions on, like hljs.
// TODO: Hopefully remove this entirely from this repo, and use something provided by Try .NET directly :)

// Define somewhere the origin
const editorOrigin = "https://try.dot.net";

// some helper functions
function postMessageToEditor(message) {
    document.getElementById('trydotnet-editor').contentWindow.postMessage(message, editorOrigin);
}

function setupWorkspace() {
    postMessageToEditor({
        "type": "setWorkspace",
        "workspace": {
            "workspaceType": "nodatime.api",
            "files": [{
                "name": "Program.cs",
                "text": "// Common system namespaces\nusing System;\nusing System.Collections.Generic;\nusing System.Globalization;\nusing System.Linq;\nusing System.Text;\n\n// All the NodaTime namespaces\nusing NodaTime;\nusing NodaTime.Calendars;\nusing NodaTime.Extensions;\nusing NodaTime.Text;\nusing NodaTime.TimeZones;\nusing NodaTime.TimeZones.Cldr;\nusing NodaTime.Utility;\n\n// Remove these if we don't include the NodaTime.Testing package \nusing NodaTime.Testing;\nusing NodaTime.Testing.TimeZones;\n\n// We probably don't want to bring these in, at least for most snippets.\n// using NodaTime.Testing.Extensions;\n\nnamespace TryNodaTime\n{\n class Program\n {\n static void Main(string[] args)\n {\n #region fragment\n #endregion\n }\n }\n}"
            }],
            "buffers": [{
                "id": "Program.cs@fragment",
                "content": "var zone = DateTimeZoneProviders.Tzdb[\"Europe/London\"];\nvar clock = SystemClock.Instance.InZone(zone);\nvar now = clock.GetCurrentZonedDateTime();\nvar pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;\nConsole.WriteLine(pattern.Format(now));",
                "position": 0
            }]
        },
        "bufferId": "Program.cs@fragment"
    });
}

function showEditor() {
    postMessageToEditor({
        type: "showEditor"
    });
    postMessageToEditor({
        type: "configureMonacoEditor",
        editorOptions: {
            minimap: {
                enabled: false
            }
        }
    });
}

function runCode() {
    setOutput("Running...", []);
    postMessageToEditor({
        type: "run"
    });
}

function processRunResults(message) {
    var output = document.getElementById('trydotnet-output');
    setOutput(message.outcome, message.output);
}

function setCode(code) {
    postMessageToEditor({
        type: "setSourceCode",
        sourceCode: code
    });
}

function setOutput(message, lines) {
    var output = document.getElementById('trydotnet-output');
    output.innerHTML = "";
    output.appendChild(document.createTextNode(message));
    for (line of lines) {
        output.appendChild(document.createElement("br"));
        output.appendChild(document.createTextNode(line));
    }
}

// TODO: Put this into an initialize method
for (codeElement of document.getElementsByTagName("code")) {
    if (codeElement.className === "language-csharp-trydotnet") {
        codeElement.className = "language-csharp trydotnet";
        var pre = codeElement.parentElement;
        var runDiv = document.createElement("div");
        var runButton = document.createElement("button");
        runButton.textContent = "Copy to editor";
        runButton.className = "trydotnetbutton";
        runDiv.appendChild(runButton);
        var createListener = code => () => setCode(code);
        runButton.addEventListener('click', createListener(codeElement.textContent));
        pre.parentNode.insertBefore(runButton, pre.nextSibling);
    }
}

// Add relevant hooks.
if (document.addEventListener) {
    document.addEventListener('DOMContentLoaded', function () {
        document.getElementById('trydotnet-run').addEventListener('click', runCode);
    });
}

if (window.addEventListener) {
    window.addEventListener('message', (m) => {
        if (m.origin === editorOrigin) {
            if (m.data.type === "RunCompleted") {
                processRunResults(m.data);
            }
            if (m.data.type === "HostListenerReady") {
                setupWorkspace();
                showEditor();
            }
        }
    });
}
