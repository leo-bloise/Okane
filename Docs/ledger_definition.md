# Ledger

## Definition

The **Ledger** is the single, system-wide record of every [Transaction](transaction_definition.md) that has ever occurred in Okane. There is exactly **one** Ledger in the entire system — it is not owned by, scoped to, or duplicated per User. Every Transaction, regardless of which Users or [Wallets](wallet_definition.md) it involves, is recorded in this one Ledger.

## Purpose

The Ledger is the source of truth for all money movement in the system. A User does not have their own ledger — a User's financial history is simply the subset of Transactions in the one Ledger where one of their Wallets appears as `from_wallet` or `to_wallet`. Likewise, a Wallet's balance is a derived view over the Ledger, not independently stored data.

## Operations

- **Record a Transaction** — append a new Transaction to the Ledger. This is the only mutation ever performed on the Ledger.
- **Query** — read Transactions from the Ledger (e.g. filtered by Wallet, User, or date range) to compute balances and reports. Querying never changes the Ledger.

## Rules

1. There is exactly one Ledger in the system. It cannot be created, duplicated, or deleted.
2. The Ledger is **append-only**. Once a Transaction is recorded, it can never be edited or removed. Mistakes are corrected by recording a new, reversing Transaction — never by altering history.
3. Every Transaction recorded anywhere in the system belongs to this one Ledger. There is no concept of a per-user, per-wallet, or per-book ledger.
4. A Wallet's balance and a User's financial history are derived by querying the Ledger; they are not stored as independent facts.
