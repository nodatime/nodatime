# Behavioral Patterns

Patterns concerned with algorithms and the assignment of responsibilities
between objects.

---

## Chain of Responsibility

Pass a request along a chain of handlers until one handles it.

**When to use**: more than one object may handle a request and the handler
isn't known a priori (middleware, validation pipelines, support escalation).

**When not to**: exactly one known handler — call it directly.

```ts
abstract class Handler {
  private next?: Handler;
  setNext(h: Handler): Handler { this.next = h; return h; }
  handle(req: number): string {
    return this.next ? this.next.handle(req) : "unhandled";
  }
}

class Level1 extends Handler {
  handle(req: number) { return req < 10 ? "L1 handled" : super.handle(req); }
}
class Level2 extends Handler {
  handle(req: number) { return req < 100 ? "L2 handled" : super.handle(req); }
}

const l1 = new Level1();
l1.setNext(new Level2());
console.log(l1.handle(5));   // L1 handled
console.log(l1.handle(50));  // L2 handled
console.log(l1.handle(999)); // unhandled
```

---

## Command

Encapsulate a request as an object, enabling queuing, logging, and undo.

**When to use**: you need undo/redo, transactional ops, scheduling, or to
decouple invoker from receiver.

**When not to**: a simple callback suffices and you don't need undo/metadata.

```ts
interface Command { execute(): void; undo(): void; }

class Light { on = false; }

class TurnOn implements Command {
  constructor(private light: Light) {}
  execute() { this.light.on = true; }
  undo() { this.light.on = false; }
}

class Remote {
  private history: Command[] = [];
  submit(cmd: Command) { cmd.execute(); this.history.push(cmd); }
  undoLast() { this.history.pop()?.undo(); }
}

const light = new Light();
const remote = new Remote();
remote.submit(new TurnOn(light)); // light.on === true
remote.undoLast();                // light.on === false
```

---

## Interpreter

Given a language, define a representation for its grammar and an interpreter
that uses it.

**When to use**: a simple, stable grammar you evaluate often (filters, rules,
arithmetic DSLs).

**When not to**: complex/evolving grammars — use a real parser/AST tool.

```ts
interface Expr { interpret(ctx: Record<string, number>): number; }

class Num implements Expr {
  constructor(private n: number) {}
  interpret() { return this.n; }
}
class Var implements Expr {
  constructor(private name: string) {}
  interpret(ctx: Record<string, number>) { return ctx[this.name] ?? 0; }
}
class Add implements Expr {
  constructor(private l: Expr, private r: Expr) {}
  interpret(ctx: Record<string, number>) {
    return this.l.interpret(ctx) + this.r.interpret(ctx);
  }
}

// (x + 5)
const expr = new Add(new Var("x"), new Num(5));
console.log(expr.interpret({ x: 10 })); // 15
```

---

## Iterator

Provide sequential access to elements of an aggregate without exposing its
representation.

**When to use**: custom traversal of a structure, or multiple simultaneous
traversals. In TS, implement `Symbol.iterator`/generators.

**When not to**: a plain array with `for...of` already works.

```ts
class NumberRange implements Iterable<number> {
  constructor(private start: number, private end: number) {}
  *[Symbol.iterator](): Iterator<number> {
    for (let i = this.start; i <= this.end; i++) yield i;
  }
}

for (const n of new NumberRange(1, 5)) process.stdout.write(`${n} `); // 1 2 3 4 5
const doubled = [...new NumberRange(1, 3)].map((n) => n * 2);          // [2,4,6]
```

---

## Mediator

Define an object that encapsulates how a set of objects interact, reducing
direct coupling.

**When to use**: many-to-many object communication (UI dialogs, chat rooms,
air-traffic control).

**When not to**: few participants — direct references are simpler than a
god-object mediator.

```ts
class ChatRoom {
  private users: User[] = [];
  register(u: User) { this.users.push(u); u.room = this; }
  send(from: User, msg: string) {
    for (const u of this.users) if (u !== from) u.receive(from.name, msg);
  }
}

class User {
  room?: ChatRoom;
  constructor(public name: string) {}
  send(msg: string) { this.room?.send(this, msg); }
  receive(from: string, msg: string) { console.log(`${this.name} <- ${from}: ${msg}`); }
}

const room = new ChatRoom();
const a = new User("Ada"), b = new User("Bob");
room.register(a); room.register(b);
a.send("hello"); // Bob <- Ada: hello
```

---

## Memento

Capture and externalize an object's internal state so it can be restored
later, without violating encapsulation.

**When to use**: snapshots for undo/checkpoint/rollback.

**When not to**: state is large/expensive to copy frequently — consider diffs
or command-based undo.

```ts
class EditorMemento {
  constructor(readonly content: string) {} // immutable snapshot
}

class Editor {
  private content = "";
  type(text: string) { this.content += text; }
  read() { return this.content; }
  save(): EditorMemento { return new EditorMemento(this.content); }
  restore(m: EditorMemento) { this.content = m.content; }
}

const ed = new Editor();
ed.type("Hello");
const checkpoint = ed.save();
ed.type(" World");
ed.restore(checkpoint);
console.log(ed.read()); // Hello
```

---

## Observer

Define a one-to-many dependency so that when one object changes state, all
dependents are notified automatically.

**When to use**: event/notification systems, reactive state. Prefer
`EventTarget`/`EventEmitter`/RxJS in production.

**When not to**: a single known dependent — call it directly.

```ts
type Observer<T> = (value: T) => void;

class Subject<T> {
  private observers = new Set<Observer<T>>();
  subscribe(o: Observer<T>): () => void {
    this.observers.add(o);
    return () => this.observers.delete(o); // unsubscribe
  }
  notify(value: T) { this.observers.forEach((o) => o(value)); }
}

const temperature = new Subject<number>();
const off = temperature.subscribe((t) => console.log(`Display: ${t}°`));
temperature.notify(22); // Display: 22°
off();
```

---

## State

Allow an object to alter its behavior when its internal state changes; it
appears to change class.

**When to use**: behavior depends on state and there are many state-dependent
conditionals (workflows, connection lifecycles, parsers).

**When not to**: two states / trivial logic — a boolean or enum is fine.

```ts
interface TrafficState { next(light: TrafficLight): void; color: string; }

class Red implements TrafficState {
  color = "RED";
  next(l: TrafficLight) { l.state = new Green(); }
}
class Green implements TrafficState {
  color = "GREEN";
  next(l: TrafficLight) { l.state = new Yellow(); }
}
class Yellow implements TrafficState {
  color = "YELLOW";
  next(l: TrafficLight) { l.state = new Red(); }
}

class TrafficLight {
  state: TrafficState = new Red();
  change() { this.state.next(this); }
  get color() { return this.state.color; }
}

const tl = new TrafficLight();
console.log(tl.color); tl.change();
console.log(tl.color); // RED -> GREEN
```

---

## Strategy

Define a family of interchangeable algorithms and make them swappable at
runtime.

**When to use**: multiple ways to do one thing chosen at runtime (sorting,
pricing, compression, auth).

**When not to**: one algorithm, or it never changes. In TS a function value is
often the whole pattern.

```ts
type SortStrategy = (a: number[]) => number[];

const quickSort: SortStrategy = (a) =>
  a.length <= 1 ? a : [
    ...quickSort(a.slice(1).filter((x) => x < a[0])),
    a[0],
    ...quickSort(a.slice(1).filter((x) => x >= a[0])),
  ];
const builtinSort: SortStrategy = (a) => [...a].sort((x, y) => x - y);

class Sorter {
  constructor(private strategy: SortStrategy) {}
  setStrategy(s: SortStrategy) { this.strategy = s; }
  sort(data: number[]) { return this.strategy(data); }
}

const sorter = new Sorter(quickSort);
console.log(sorter.sort([3, 1, 2])); // [1,2,3]
sorter.setStrategy(builtinSort);
```

---

## Template Method

Define the skeleton of an algorithm in a base method, deferring some steps to
subclasses.

**When to use**: several algorithms share structure but differ in specific
steps (data importers, build pipelines).

**When not to**: steps vary wildly — prefer Strategy (composition over
inheritance).

```ts
abstract class DataMiner {
  // template method — fixed sequence
  mine(path: string): string {
    const raw = this.openFile(path);
    const data = this.parse(raw);
    return `Analyzed: ${data}`;
  }
  protected abstract openFile(path: string): string;
  protected abstract parse(raw: string): string;
}

class CSVMiner extends DataMiner {
  protected openFile(p: string) { return `csv-bytes(${p})`; }
  protected parse(raw: string) { return raw.replace("bytes", "rows"); }
}

console.log(new CSVMiner().mine("data.csv"));
```

---

## Visitor

Represent an operation to be performed on elements of an object structure,
letting you add operations without modifying the elements.

**When to use**: a stable class hierarchy needs many unrelated operations
added over time (AST passes, document export).

**When not to**: the element hierarchy changes often — every change touches
every visitor. (A discriminated union + `switch` is the lighter TS idiom.)

```ts
interface ShapeVisitor<R> { circle(c: Circle): R; square(s: Square): R; }

interface Shape { accept<R>(v: ShapeVisitor<R>): R; }

class Circle implements Shape {
  constructor(public r: number) {}
  accept<R>(v: ShapeVisitor<R>) { return v.circle(this); }
}
class Square implements Shape {
  constructor(public side: number) {}
  accept<R>(v: ShapeVisitor<R>) { return v.square(this); }
}

const areaVisitor: ShapeVisitor<number> = {
  circle: (c) => Math.PI * c.r ** 2,
  square: (s) => s.side ** 2,
};

const shapes: Shape[] = [new Circle(2), new Square(3)];
console.log(shapes.map((s) => s.accept(areaVisitor))); // [12.56.., 9]
```
