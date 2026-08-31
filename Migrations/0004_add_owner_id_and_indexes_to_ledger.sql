-- migrate:up
ALTER TABLE ledger ADD COLUMN owner_id uuid REFERENCES users (id);

UPDATE ledger
SET owner_id = wallets.owner_id
FROM wallets
WHERE wallets.id = ledger.from_wallet_id;

ALTER TABLE ledger ALTER COLUMN owner_id SET NOT NULL;

CREATE INDEX ix_ledger_owner_id_recorded_at ON ledger (owner_id, recorded_at DESC);
CREATE INDEX ix_ledger_created_at ON ledger (created_at);

-- migrate:down
DROP INDEX ix_ledger_created_at;
DROP INDEX ix_ledger_owner_id_recorded_at;
ALTER TABLE ledger DROP COLUMN owner_id;
