# Creational Patterns

Patterns that abstract the instantiation process so a system is independent of
how its objects are created, composed, and represented.

---

## Factory Method

Define an interface for creating an object, but let subclasses decide which
class to instantiate.

**When to use**: a class can't anticipate the class of objects it must create;
you want subclasses to specify the created objects.

**When not to**: there is only one product type — just call the constructor.

```ts
interface Transport {
  deliver(): string;
}

class Truck implements Transport {
  deliver() { return "Delivered by land in a box."; }
}

class Ship implements Transport {
  deliver() { return "Delivered by sea in a container."; }
}

abstract class Logistics {
  abstract createTransport(): Transport; // the factory method

  planDelivery(): string {
    const transport = this.createTransport();
    return transport.deliver();
  }
}

class RoadLogistics extends Logistics {
  createTransport() { return new Truck(); }
}

class SeaLogistics extends Logistics {
  createTransport() { return new Ship(); }
}

const logistics: Logistics = new SeaLogistics();
console.log(logistics.planDelivery()); // Delivered by sea in a container.
```

---

## Abstract Factory

Provide an interface for creating *families* of related objects without
specifying their concrete classes.

**When to use**: the system must be configured with one of multiple families
of products (e.g. UI themes, cloud providers) and you must enforce that
products from one family are used together.

**When not to**: products rarely change as a set — Abstract Factory adds a lot
of indirection.

```ts
interface Button { render(): string; }
interface Checkbox { render(): string; }

class MacButton implements Button { render() { return "[ Mac Button ]"; } }
class WinButton implements Button { render() { return "[ Win Button ]"; } }
class MacCheckbox implements Checkbox { render() { return "[x] Mac"; } }
class WinCheckbox implements Checkbox { render() { return "[x] Win"; } }

interface GUIFactory {
  createButton(): Button;
  createCheckbox(): Checkbox;
}

class MacFactory implements GUIFactory {
  createButton() { return new MacButton(); }
  createCheckbox() { return new MacCheckbox(); }
}

class WinFactory implements GUIFactory {
  createButton() { return new WinButton(); }
  createCheckbox() { return new WinCheckbox(); }
}

function buildUI(factory: GUIFactory) {
  return `${factory.createButton().render()} ${factory.createCheckbox().render()}`;
}

const os = "mac";
console.log(buildUI(os === "mac" ? new MacFactory() : new WinFactory()));
```

---

## Builder

Separate the construction of a complex object from its representation so the
same construction process can create different representations.

**When to use**: an object has many optional parameters or a multi-step,
order-sensitive assembly; you want immutable results.

**When not to**: the object is simple — an options object literal is clearer.

```ts
class HttpRequest {
  constructor(
    readonly url: string,
    readonly method: string,
    readonly headers: Record<string, string>,
    readonly body?: string,
  ) {}
}

class HttpRequestBuilder {
  private method = "GET";
  private headers: Record<string, string> = {};
  private body?: string;

  constructor(private readonly url: string) {}

  withMethod(method: string): this { this.method = method; return this; }
  withHeader(k: string, v: string): this { this.headers[k] = v; return this; }
  withBody(body: string): this { this.body = body; return this; }

  build(): HttpRequest {
    return new HttpRequest(this.url, this.method, this.headers, this.body);
  }
}

const req = new HttpRequestBuilder("https://api.example.com/users")
  .withMethod("POST")
  .withHeader("Content-Type", "application/json")
  .withBody(JSON.stringify({ name: "Ada" }))
  .build();
```

---

## Prototype

Create new objects by cloning an existing instance.

**When to use**: instantiation is expensive, or the concrete type should be
chosen at runtime from a registry of pre-built prototypes.

**When not to**: plain data — `structuredClone()` or a spread is enough.

```ts
interface Cloneable<T> { clone(): T; }

class Shape implements Cloneable<Shape> {
  constructor(public x: number, public y: number, public color: string) {}

  clone(): Shape {
    return new Shape(this.x, this.y, this.color); // deep where needed
  }
}

const original = new Shape(10, 20, "red");
const copy = original.clone();
copy.color = "blue";
// original.color is still "red"
```

---

## Singleton

Ensure a class has only one instance and provide a global access point.

**When to use (rarely)**: exactly one coordinating object must exist (a
connection pool, an app-wide cache) and a module export won't do.

**When not to**: most cases — prefer a module singleton or dependency
injection. Classic Singletons hide dependencies and complicate testing.

```ts
// Idiomatic TS: a module is already a singleton.
class Logger {
  private logs: string[] = [];
  log(msg: string) { this.logs.push(msg); }
  history() { return [...this.logs]; }
}
export const logger = new Logger(); // single shared instance

// Class form when you need lazy init / test reset:
class Config {
  private static instance: Config | null = null;
  private constructor(readonly env: string) {}

  static get(): Config {
    return (Config.instance ??= new Config(process.env.NODE_ENV ?? "dev"));
  }

  static reset() { Config.instance = null; } // for tests
}
```
