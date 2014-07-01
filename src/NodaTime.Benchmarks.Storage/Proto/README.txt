To run the code generation, from the NodaTime.Benchmarks.Storage\Proto directory,
run:

..\..\packages\Google.ProtocolBuffers.2.4.1.521\tools\ProtoGen.exe benchmarks.proto

If this fails with a message such as:
benchmarks.proto:1:1: Expected top-level statement (e.g. "message").
... then check whether Visual Studio has added a UTF-8 BOM to the start of the message.