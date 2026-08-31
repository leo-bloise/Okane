-- migrate:up
ALTER TABLE wallets ADD CONSTRAINT ux_wallets_owner_id_name UNIQUE (owner_id, name);

-- migrate:down
ALTER TABLE wallets DROP CONSTRAINT ux_wallets_owner_id_name;
