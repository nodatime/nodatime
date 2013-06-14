---
layout: userguide
title: Threading in Noda Time
category: advanced
weight: 120
---

Each type in the public API has its thread safety documented briefly, but that documentation usually
refers to this entry in the user guide, too. Readers are advised to read Eric Lippert's blog post,
["What is this thing you call thread safe?"][lippert] in addition to this guide. This page aims for
the *spirit* of a useful guide to the thread safety of Noda Time, but if it leaves your questions unanswered,
please feel free to request clarification on the [mailing list][].

Many of the types in Noda Time are immutable to a greater or lesser extent (again, Eric Lippert's blog provides
a [rich seam of information on the topic of mutability][immutability]). When used carefully, this allows values
of these types to be accessed safely between multiple threads - and the degree of care required is *reasonably* small in
most cases.

Immutable reference types
-------------------------

These are the simplest to document, and include all time zone and calendar implementations, the time zone providers,
and [`Period`](noda-type://NodaTime.Period). No members of these types modify visible state; if you have a reference to
an instance of one of these types, you can do no harm to other threads by calling any members via that reference.

Some of the reference types do contain mutable state internally (usually for caching purposes) but this is invisible to
the caller: locks are acquired carefully to avoid deadlocks, and (as far as we're aware!) only internal code is called while
a lock is being held, avoiding the prospect of user code causing problems.

If you choose to have writable fields of these types visible between multiple threads, you are responsible for ensuring
that any changes are visible to other threads (there's nothing we can do about that) but you at least won't cause any
strange behaviour within the types that way: if you call a method via a field which is being changed by another thread,
you'll either see the result of the call on the original value, or on the new value.

Immutable value types
---------------------

All the value types in Noda Time are immutable, but this doesn't give quite as much thread safety as immutable reference
types. While the CLI specification guarantees that modifications to a reference type field are atomic, there are no such
*general* guarantees for value types. (There are guarantees around certain sizes, but some of the value types in Noda Time
are larger than that limited guarantee. I wouldn't want to start relying on it directly anyway.)

So long as you either make your own fields read-only *or* synchronize access to the fields, you should be fine - but if you
use unsynchronized access to writable fields, it's entirely possible for a "hybrid" value to be visible, with part of the old
value and part of the new value. So as an example using Noda Time types, if you tried to format a
[`LocalDateTime`](noda-type://NodaTime.LocalDateTime) field as a string, and at the same time another thread changed the value,
you could end up with the date and time from the old value formatted using the calendar system from the new value. Needless to
say, this is far from ideal - don't do it.

Mutable reference types
-----------------------

Some classes are deliberately mutable within Noda Time - [`PeriodBuilder`](noda-type://NodaTime.PeriodBuilder) is a good example.
These types are very obviously mutable, and should *not* be shared between threads without explicit synchronization. No type
in Noda Time has any specific thread affinity, so you shouldn't see any ill effects if you do use synchronization. Even so, we would
generally recommend against doing this: the types are generally *designed* around making it easy to build a value within a single thread.

Exceptions
----------

The exceptions within Noda Time are documented in the same way as the .NET types, partly as we can't make any firmer guarantees around
safety than the base class provides. That said, none of the exceptions within Noda Time have any mutable state declared within Noda Time code.

Enums
-----

Enums should generally be treated as immutable value types. Accessing the enum values directly can *never* have any nasty effects,
but be careful around using a writable field of an enum type, for the same reasons as given in the earlier discussion.

[lippert]: http://blogs.msdn.com/b/ericlippert/archive/2009/10/19/what-is-this-thing-you-call-thread-safe.aspx
[immutability]: http://blogs.msdn.com/b/ericlippert/archive/tags/immutability/
[mailing list]: http://groups.google.com/group/noda-time
