# Structural Patterns

Patterns concerned with how classes and objects are composed to form larger
structures.

---

## Adapter

Convert the interface of a class into another interface clients expect.

**When to use**: you must integrate an existing/third-party class whose
interface doesn't match what your code needs.

**When not to**: you own both sides — just change the interface.

```ts
interface PaymentProcessor { pay(amountCents: number): string; }

// Incompatible third-party SDK:
class LegacyStripe {
  makeCharge(dollars: number) { return `Charged $${dollars} via Stripe`; }
}

class StripeAdapter implements PaymentProcessor {
  constructor(private legacy: LegacyStripe) {}
  pay(amountCents: number) { return this.legacy.makeCharge(amountCents / 100); }
}

const processor: PaymentProcessor = new StripeAdapter(new LegacyStripe());
console.log(processor.pay(2599)); // Charged $25.99 via Stripe
```

---

## Bridge

Decouple an abstraction from its implementation so the two can vary
independently.

**When to use**: a class has two orthogonal dimensions that would otherwise
explode into a class-per-combination matrix (e.g. *shape* × *renderer*).

**When not to**: only one dimension ever varies.

```ts
interface Renderer { drawCircle(r: number): string; }

class SVGRenderer implements Renderer {
  drawCircle(r: number) { return `<circle r="${r}" />`; }
}
class CanvasRenderer implements Renderer {
  drawCircle(r: number) { return `ctx.arc(0,0,${r})`; }
}

abstract class Shape {
  constructor(protected renderer: Renderer) {}
  abstract draw(): string;
}

class Circle extends Shape {
  constructor(renderer: Renderer, private radius: number) { super(renderer); }
  draw() { return this.renderer.drawCircle(this.radius); }
}

console.log(new Circle(new SVGRenderer(), 5).draw());    // <circle r="5" />
console.log(new Circle(new CanvasRenderer(), 5).draw()); // ctx.arc(0,0,5)
```

---

## Composite

Compose objects into tree structures and treat individual objects and
compositions uniformly.

**When to use**: a part-whole hierarchy where clients should ignore the
difference between leaves and containers (file systems, UI trees).

**When not to**: the structure is flat, or leaf/container behavior diverges
sharply.

```ts
interface FileSystemNode { size(): number; }

class FileLeaf implements FileSystemNode {
  constructor(private bytes: number) {}
  size() { return this.bytes; }
}

class Directory implements FileSystemNode {
  private children: FileSystemNode[] = [];
  add(node: FileSystemNode) { this.children.push(node); return this; }
  size() { return this.children.reduce((sum, c) => sum + c.size(), 0); }
}

const root = new Directory()
  .add(new FileLeaf(100))
  .add(new Directory().add(new FileLeaf(50)).add(new FileLeaf(25)));
console.log(root.size()); // 175
```

---

## Decorator

Attach additional responsibilities to an object dynamically by wrapping it.

**When to use**: you need to add/combine behaviors at runtime without a
combinatorial subclass explosion (streams, middleware, I/O).

**When not to**: behaviors are fixed at compile time — subclass or compose
directly. (Distinct from TS `@decorator` syntax.)

```ts
interface Coffee { cost(): number; description(): string; }

class Espresso implements Coffee {
  cost() { return 2; }
  description() { return "Espresso"; }
}

abstract class CoffeeDecorator implements Coffee {
  constructor(protected inner: Coffee) {}
  abstract cost(): number;
  abstract description(): string;
}

class Milk extends CoffeeDecorator {
  cost() { return this.inner.cost() + 0.5; }
  description() { return `${this.inner.description()} + Milk`; }
}

class Sugar extends CoffeeDecorator {
  cost() { return this.inner.cost() + 0.2; }
  description() { return `${this.inner.description()} + Sugar`; }
}

const order = new Sugar(new Milk(new Espresso()));
console.log(order.description(), order.cost()); // Espresso + Milk + Sugar 2.7
```

---

## Facade

Provide a unified, higher-level interface to a set of interfaces in a
subsystem.

**When to use**: you want a simple entry point over a complex subsystem and to
shield callers from its moving parts.

**When not to**: callers genuinely need fine-grained subsystem access.

```ts
class CPU { freeze() {} jump(p: number) {} execute() {} }
class Memory { load(pos: number, data: string) {} }
class HardDrive { read(lba: number): string { return "boot-data"; } }

class ComputerFacade {
  private cpu = new CPU();
  private ram = new Memory();
  private hd = new HardDrive();

  start() {
    this.cpu.freeze();
    this.ram.load(0, this.hd.read(0));
    this.cpu.jump(0);
    this.cpu.execute();
    return "Booted";
  }
}

console.log(new ComputerFacade().start());
```

---

## Flyweight

Share fine-grained objects to support large numbers of them efficiently by
separating intrinsic (shared) from extrinsic (context) state.

**When to use**: huge counts of similar objects blow memory (glyphs, tiles,
particles) and most state is shareable.

**When not to**: objects are few or mostly unique — the indirection costs more
than it saves.

```ts
class TreeType { // intrinsic, shared
  constructor(readonly name: string, readonly texture: string) {}
  draw(x: number, y: number) { return `${this.name}@(${x},${y})`; }
}

class TreeFactory {
  private static pool = new Map<string, TreeType>();
  static get(name: string, texture: string): TreeType {
    const key = `${name}:${texture}`;
    let t = this.pool.get(key);
    if (!t) this.pool.set(key, (t = new TreeType(name, texture)));
    return t;
  }
}

const forest = Array.from({ length: 1000 }, (_, i) =>
  ({ type: TreeFactory.get("Oak", "oak.png"), x: i, y: i })); // 1 TreeType
```

---

## Proxy

Provide a surrogate for another object to control access to it (lazy, remote,
protective, caching).

**When to use**: you need lazy initialization, access control, caching, or
logging around an expensive/remote object.

**When not to**: no access concern exists — call the object directly. (JS
`Proxy` is a built-in tool for this.)

```ts
interface Image { display(): string; }

class RealImage implements Image {
  constructor(private file: string) { /* expensive load */ }
  display() { return `Showing ${this.file}`; }
}

class LazyImageProxy implements Image {
  private real?: RealImage;
  constructor(private file: string) {}
  display() {
    this.real ??= new RealImage(this.file); // load on first use
    return this.real.display();
  }
}

const img = new LazyImageProxy("photo.jpg"); // not loaded yet
console.log(img.display());                  // loaded now
```
