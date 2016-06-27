Documentation plan
====

This plan is evolving. This document helps me keep track of what I'm
doing.

Ultimate goals:

- Ready-to-serve HTML is in nodatime.org repo, as it is now
- Must be able to edit previous user guides to fix typos
- Should be able to regenerate API reference docs with new style,
  even for old versions.
- Should not need to fetch/rebuild code in order to rebuild docs of old
  versions. (Not sure about this... could be handy, e.g. if docfx
  metadata generator changes.)
- Should be able to integrate some sort of version picker.
- Conceptual documentation in markdown
- Extra information (available in portable code, first version
  present, thread safety, non-nullable parameters) should be included
  as per the current site.
- Better integration of API documentation and conceptual
  documentation.

Directory layout
----

```
docs\{README.md, docfx.json}
    |- developer (markdown)
    |- unstable\
    |          |- userguide (markdown)
    |          \- api (output of docfx metadata, not in git)
    |          
    |- 1.0.x\
    |       |- userguide (markdown)
    |       \- api (output of docfx metadata, stored in git)
    |- 1.1.x (as per 1.0.x)
    |- 1.2.x (as per 1.0.x)
    \- 1.3.x (as per 1.0.x)
```

Each `api` directory will also contain the results of whatever tool
provides the extra information such as thread-safety.
(Alternatively, that tool can modify the yml files directly.)

A one-off process will generate the `yml` files in 1.0.x etc with
docfx. These can then be stored in Git, so that `docfx build` can
use them. A normal `docfx metadata` command will only repopulate
`docs\unstable\api`, as that's the only code that changes on the
default branch.

Once 2.0 ships, we'll take a copy of the `unstable` directory
(including `api`) and add it to git. The `unstable` directory
will then be for 2.1. `docfx.json` will be updated to reflect the
presence of the 2.0 directory.

Build process
----

This will be scripted, but:

- Delete `_site` (output folder)
- Delete `unstable\api`
- Run `docfx metadata -f`
- Run whatever tool populates extra information
- Run `docfx build`
- Clean target directory in `nodatime.org` repo
- Copy everything from `_site` into the target directory in `nodatime.org`

Unknown bits
----

- How to link the versions together
- Should the home page itself be built in docfx, or is that asking
  too much?
- Various questions around populating the extra data
- How do we get docfx to cope with there being multiple API versions
  with the same UIDs? Aargh. Could disambiguate UIDs, but that's
  really painful in terms of editing.
