---
layout: userguide
title: Why does Noda Time exist?
category: intro
weight: 20
---

Noda Time exists for .NET for the same reason that [Joda Time][1]
exists for Java: the built-in libraries for handling dates and times are inadequate.
Both platforms provide far too few types to represent date and time
values in a way which encourages the developer to really consider
what kind of data they're dealing with... which in turn makes it
hard to treat the data consistently.

While the .NET date/time types are actually easier to use than their
Java counterparts (largely as a result of being immutable structs
instead of mutable classes), they're actually less powerful - in
.NET, there's no such concept of "a date and time in a specific time
zone" for example.

Noda Time aims to change that with a library which is powerful and
easy to use *correctly*. It is built on the underlying "engine" of
Joda Time, but the public API has been largely rewritten, both to
provide an API which is more idiomatic for .NET, and also to rectify
some of the Joda Time decisions which the Noda Time team view as
"unfortunate". (Some of these are simply due to having different goals;
others I'd argue are really mistakes.)

For a more detailed critique on the problems with the existing
date/time support in .NET (and `DateTime` in particular), see this
[blog post][2]. Of course, if a later version of .NET comes out with
a new date/time API, the Noda Time team would happily go into
retirement (other than for the sake of those forced to stick with
earlier versions of .NET, of course).

[1]: http://www.joda.org/joda-time
[2]: http://noda-time.blogspot.com/2011/08/what-wrong-with-datetime-anyway.html
