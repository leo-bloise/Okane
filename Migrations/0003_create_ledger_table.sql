CREATE TABLE ledger
(
    id             uuid           PRIMARY KEY,
    from_wallet_id uuid           NOT NULL REFERENCES wallets (id),
    to_wallet_id   uuid           NOT NULL REFERENCES wallets (id),
    amount         numeric(18, 2) NOT NULL CHECK (amount > 0),
    description    text,
    recorded_at    timestamptz    NOT NULL,
    created_at     timestamptz    NOT NULL,
    CHECK (from_wallet_id <> to_wallet_id)
);

CREATE INDEX ix_ledger_from_wallet_id ON ledger (from_wallet_id);
CREATE INDEX ix_ledger_to_wallet_id ON ledger (to_wallet_id);
