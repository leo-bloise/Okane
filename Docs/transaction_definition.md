# Transaction

## Definition

A **Transaction** is a single, strict one-to-one transfer of value from one [Wallet](wallet_definition.md) to another. It is the only kind of entry recorded in the [Ledger](ledger_definition.md): debiting `from_wallet` and crediting `to_wallet` by the same `Amount`.

## Properties

| Property | Description |
|---|---|
| `Id` | Unique identifier of the Transaction. |
| `Amount` | The value transferred, denominated in BRL. Must be positive. |
| `FromWallet` | The Wallet the value is debited from. |
| `ToWallet` | The Wallet the value is credited to. |
| `Description` | Optional free-text memo (e.g. "Lunch at McDonald's"). |
| `RecordedAt` | Timestamp the Transaction was recorded. Immutable. |

## Operations

- **Record** — the only operation. Creates a new Transaction and appends it to the Ledger.

There is no update or delete operation. Because the Ledger is append-only (see [Ledger rules](ledger_definition.md#rules)), a Transaction can never be edited or removed after it is recorded — a mistake is corrected by recording a new, reversing Transaction with `from_wallet` and `to_wallet` swapped.

## Rules

1. **A Transaction always has exactly one `FromWallet` and one `ToWallet`.** Splits, multi-party transactions, and transactions with no counterpart are not supported.
2. **`FromWallet` and `ToWallet` must be different Wallets.** A Transaction cannot transfer value to itself.
3. **`FromWallet` and `ToWallet` must belong to the same User.** Since every User's [External Wallet](wallet_definition.md#rules) also belongs to that User, this single rule covers both allowed cases — a transfer between two of the User's own `Standard` Wallets, and a transfer between a `Standard` Wallet and the User's own `External` Wallet. A direct transfer between two different Users' Wallets is not possible.
4. **`Amount` must be strictly greater than zero.**
5. **`Amount` is denominated in BRL.** Multi-currency support is out of scope for now.
6. **A Transaction is immutable once recorded.** It cannot be edited or deleted.
7. **Both `FromWallet` and `ToWallet` must be `Active`.** A Transaction cannot reference an archived Wallet.
