# WeAreDevelopers 2026 — Building Distributed Apps with Graftcode

Welcome! In this workshop we'll build a small but complete distributed application
**without writing a single line of REST plumbing** — no controllers, no OpenAPI specs,
no hand-written fetch clients, no DTO mapping by hand. Instead we'll use **Graftcode**
to connect everything together.

## What is Graftcode?

Graftcode is a different way of doing service-to-service and frontend-to-backend
integration. Instead of exposing your logic as a REST API and then writing (and
re-writing) a client to consume it, you write your backend as **plain classes with
plain public methods** — and those public methods *are* the contract.

You then:

- **Host** that code with the Graftcode Gateway (`gg`), which makes those public
  methods callable from anywhere.
- **Install** the service into a consumer (another backend, or a frontend) as a normal
  package — a "graft" — using your usual package manager (`npm`, `dotnet add package`, etc.).
- **Call** the backend methods directly, as if they were local functions, with full type
  safety and IDE autocompletion.

When the backend changes, you just bump the package version — no regenerating clients,
no re-syncing specs, no rewriting fetch calls. The mental shift is simple: **you consume
a backend as a dependency, not as an API you have to integrate against.**

Learn more at [docs.graftcode.com](https://docs.graftcode.com).

## What We'll Build

Over the course of the workshop we'll build a **distributed city-weather app** made of
three independent pieces that talk to each other through Graftcode:

1. **An authentication service** — lets a user log in with a username and password and
   issues a JWT on success. We'll extend it so each user has a list of favourite cities,
   retrievable only by the authenticated caller (identity travels in the request, never
   as a method parameter).

2. **A weather service** — returns the current weather for a given city. It validates the
   caller's JWT and gets its data by **consuming an external Graftcode service** (an
   already-published weather graft) — again, no hand-written HTTP client.

3. **A React frontend** — where a user can log in, see their favourite cities, and view
   the weather for a selected city. It talks to both backend services through their graft
   packages, sending the JWT along so the backends know who's calling.

By the end you'll have a working app where a browser and two backend microservices all
communicate through Graftcode, with authentication flowing end-to-end. We'll build it
incrementally, one focused step at a time, so you can see each piece slot into place.

> Demo user for the app: **`wad`** / **`password`**

## Warm-up: a quick example by hand

Before we build the full app, we'll do a short hands-on example **together, live**, to
feel the Graftcode workflow end-to-end. We'll follow the official quick-start:

**[Connect a React frontend to a backend](https://docs.graftcode.com/quick-start/connect-frontend-to-backend/react)**

In just a few minutes we'll:

- Start from a plain React app.
- Open a live backend in **Graftcode Vision** and copy the generated install command
  (instead of reading an API spec).
- Install the backend as a typed npm package (a graft).
- Point the generated client at the backend host.
- Call a real backend method — `BillingLogic.CalculateMonthlyBill(...)` — straight from a
  React component, as if it were local code, with full autocompletion.

This warm-up gives you the "aha" moment: **installing and calling a backend feels exactly
like using any other package.** Once that clicks, everything we build afterwards will feel
natural.

Let's get started!
