The generated API documentation is in api/. To build the help with Sandcastle,
you'll need a number of other tools installed. More details to come, but see
http://noda-time.blogspot.com/2010/04/documentation-with-sandcastle-notebook.html
for a starting point.

The user and developer guides (userguide/ and devloper/) are generated from
source in src/docs/userguide/ and src/docs/developer/ using BuildMarkdownDocs
To regenerate the documentation, run builddocs.bat from the root of the project
(or 'make docs' under Mono).
