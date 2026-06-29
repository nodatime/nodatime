---
name: gof-patterns
description: >-
  Gang of Four design patterns reference for TypeScript projects. Use when
  choosing or implementing a design pattern, refactoring toward a known pattern,
  reviewing code for pattern misuse, or answering "which pattern fits here"
  questions. Covers all 23 creational, structural, and behavioral patterns with
  idiomatic TypeScript examples and guidance on when (and when not) to use each.
---

# Gang of Four Patterns in TypeScript

The 23 classic design patterns from *Design Patterns: Elements of Reusable
Object-Oriented Software* (Gamma, Helm, Johnson, Vlissides), adapted to
idiomatic TypeScript.

## 🎯 Why: Design for Change

The goal of writing software is to be able to **change it safely**. Every
GoF pattern is a named seam that isolates one axis of change: Strategy
hides algorithm choice, Adapter hides a vendor swap, Observer hides who's
listening, Factory hides what's instantiated. Reach for a pattern when
you have a *demonstrated* axis of change to absorb. A pattern installed
speculatively adds coupling without buying any change-safety.

## How to use this skill

1. Identify the *kind* of problem from the decision guide below.
2. Open the matching reference file for the full TypeScript example and the
   "when to use / when not to" notes.
3. Prefer the simplest construct that solves the problem. In TypeScript, a
   union type, a closure, or a plain function often beats a class-heavy
   pattern. Reach for a GoF pattern when the indirection earns its keep.

## Reference files

- `references/creational.md` — object creation: Factory Method, Abstract
  Factory, Builder, Prototype, Singleton.
- `references/structural.md` — object composition: Adapter, Bridge, Composite,
  Decorator, Facade, Flyweight, Proxy.
- `references/behavioral.md` — object interaction & responsibility: Chain of
  Responsibility, Command, Interpreter, Iterator, Mediator, Memento, Observer,
  State, Strategy, Template Method, Visitor.

## Decision guide — symptom → pattern

| You are trying to… | Consider | Category |
|---|---|---|
| Create objects without naming the concrete class | Factory Method, Abstract Factory | Creational |
| Build a complex object step by step | Builder | Creational |
| Copy an existing object | Prototype | Creational |
| Guarantee exactly one instance | Singleton (often a module instead) | Creational |
| Make an incompatible interface usable | Adapter | Structural |
| Vary abstraction and implementation independently | Bridge | Structural |
| Treat individual objects and groups uniformly | Composite | Structural |
| Add behavior to objects without subclassing | Decorator | Structural |
| Hide a complex subsystem behind one interface | Facade | Structural |
| Share many fine-grained objects cheaply | Flyweight | Structural |
| Control access to an object (lazy, remote, guarded) | Proxy | Structural |
| Pass a request along a series of handlers | Chain of Responsibility | Behavioral |
| Encapsulate a request as an object (undo, queue) | Command | Behavioral |
| Evaluate sentences in a small language | Interpreter | Behavioral |
| Traverse a collection without exposing its structure | Iterator | Behavioral |
| Reduce many-to-many object coupling | Mediator | Behavioral |
| Capture and restore object state | Memento | Behavioral |
| Notify dependents of state changes | Observer | Behavioral |
| Change behavior when internal state changes | State | Behavioral |
| Swap one of several algorithms at runtime | Strategy | Behavioral |
| Fix an algorithm skeleton, vary steps | Template Method | Behavioral |
| Add operations to a class hierarchy without editing it | Visitor | Behavioral |

## TypeScript-specific guidance

- **Singleton**: an ES module with exported bindings is already a singleton.
  Use the class form only when you need lazy init or to swap the instance in
  tests.
- **Strategy / Command / State**: a function or a discriminated union is often
  lighter than a class hierarchy. Use classes when strategies carry state.
- **Prototype**: `structuredClone()` covers most deep-copy needs; implement
  `clone()` only for objects with methods or non-cloneable fields.
- **Observer**: `EventTarget`, `EventEmitter`, or an RxJS `Subject` are
  battle-tested implementations — hand-roll only for learning or tight control.
- **Decorator**: the TC39/experimental `@decorator` syntax is a *language*
  feature, not the GoF pattern. The GoF Decorator is object composition; keep
  them mentally separate.
- Favor `interface` for pattern contracts and discriminated unions for
  closed sets of variants; lean on `readonly` and `as const` for immutability
  in Memento/Prototype.
