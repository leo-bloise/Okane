# Project Overview: Okane Frontend

This is the frontend for the **Okane** application, a personal finance tracker. It is built with **Next.js (App Router)** and runs on **Bun**. It serves as a **Backend for Frontend (BFF)**, mediating requests between the user's browser and an **ASP.NET Core API**.

## Core Technology Stack

- **Framework**: Next.js 15+ (App Router)
- **Runtime/Package Manager**: Bun
- **Backend API**: ASP.NET Core
- **Styling**: Tailwind CSS v4
- **HTTP Clients**: 
  - `axios`: Used in the service layer for calling the external ASP.NET Core API.
  - `fetch`: Used for internal BFF calls and standard browser requests.
- **Form Management**: React Hook Form + Zod
- **UI Components**: shadcn/ui (Radix UI)

## Architectural Patterns

### 1. BFF Pattern (Backend for Frontend)
The Next.js application acts as a proxy/mediator:
- **Client Components** call internal Next.js API routes (e.g., `/app/(authorized)/accounts/api/route.ts`).
- **Server Components** use `fetchFromServer` from `@/lib/api.server-component` to call internal API routes.
- **Internal API Routes** use services from `@/lib/service` to communicate with the ASP.NET Core API.

### 2. Service Layer
Located in `lib/service/`, these files contain the logic for interacting with the external ASP.NET Core API.
- Use the `api` instance from `@/lib/api` (Axios).
- Always use `createHeaders()` from `retrieve-token.service.ts` for authenticated requests.

### 3. Data Fetching
- **Server Components**: Prefer fetching data from internal APIs via `fetchFromServer`.
- **API Response Structure**: Follow the `BaseResponse<T>` pattern defined in `lib/types/base.response.ts`.

### 4. Authentication
- Session tokens are stored in an encrypted `session` cookie.
- Token management is handled in `lib/service/login.service.ts` and `lib/service/retrieve-token.service.ts`.
- Server-side utilities for encryption/decryption are in `lib/utils.server.ts`.

## Directory Structure

- `app/`: Next.js pages, layouts, and internal API routes (BFF).
- `components/ui/`: Primitive UI components (shadcn/ui style).
- `components/custom/`: Feature-specific and complex components.
- `lib/service/`: Logic for external API interaction.
- `lib/types/`: TypeScript interfaces and Zod schemas.
- `lib/hooks/`: Custom React hooks.

## Coding Standards

- **Type Safety**: Use TypeScript strictly. Define types/interfaces in `lib/types`.
- **Validation**: Use Zod for form and API data validation.
- **Consistency**: 
  - Components should be modular and follow the `index.tsx` + `View.tsx` pattern for complex components if necessary.
  - Prefer composition over inheritance.
- **Errors**: Catch and handle errors using the `BaseResponse` status codes and messages.

## Development Commands

- `bun run dev`: Start development server.
- `bun run build`: Build for production.
- `bun run lint`: Run ESLint.
- `bun install`: Add dependencies.
