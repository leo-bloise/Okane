# Wallet

## Definition

A **Wallet** is a named container of value, owned by exactly one User. It groups money for a particular purpose — such as "Food", "Savings", or "Rent" — or represents the boundary between a User's tracked finances and the outside world (the **External Wallet**).

Wallets are the two endpoints of every [Transaction](transaction_definition.md): a Transaction always moves value `from_wallet` → `to_wallet`. A Wallet never holds a balance directly — its balance is derived from the [Ledger](ledger_definition.md).

## Properties

| Property | Description |
|---|---|
| `Id` | Unique identifier of the Wallet. |
| `Name` | User-facing name (e.g. "Food", "Savings", "External Wallet"). |
| `Owner` | The single User the Wallet belongs to. |
| `Kind` | `External` (the User's reserved boundary wallet) or `Standard` (a regular, user-created wallet). |
| `Balance` | Derived: sum of Transactions where the Wallet is `to_wallet`, minus the sum where it is `from_wallet`. Never set directly. |
| `Status` | `Active` or `Archived`. Archived Wallets are read-only. |
| `CreatedAt` | Audit timestamp. |

## Operations

- **Create** — a User opens a new `Standard` Wallet with a name. A new Wallet starts with a balance of zero.
- **Rename** — change the `Name` of a `Standard` Wallet.
- **Archive** — mark a Wallet as `Archived`, making it read-only. Does not delete its history.
- **Reactivate** — return an `Archived` Wallet to `Active`.
- **Delete** — permanently remove a Wallet. Only permitted under the rules below.

The `External` Wallet is created automatically and is not subject to Create, Rename, or Delete by the User (see Rules).

## Rules

1. **Every Wallet belongs to exactly one User.** Wallets cannot be shared, transferred, or jointly owned.
2. **Every User has exactly one External Wallet**, created automatically when the User account is created. It cannot be renamed, archived, or deleted. It represents money entering or leaving the User's tracked finances from outside Okane (e.g. a purchase from a merchant, a salary deposit, an ATM withdrawal) without requiring a dedicated Wallet per outside party.
3. **Beyond the External Wallet, a User may create any number of `Standard` Wallets**, and may rename, archive, reactivate, or delete them subject to the rules below.
4. **Balance is derived, not set.** No operation may directly assign a value to `Balance`; it only changes as a side effect of Transactions recorded against the Wallet.
5. **A Wallet cannot be deleted if it has been used in at least one Transaction.** Only an empty `Standard` Wallet (zero Transactions) may be deleted; otherwise it may only be archived.
6. **An archived Wallet is read-only.** No new Transaction may reference it as `from_wallet` or `to_wallet` until it is reactivated.
7. **Only the owning User may modify a Wallet** — create, rename, archive, reactivate, or delete.
