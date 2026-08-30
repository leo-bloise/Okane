ALTER TABLE wallets ADD CONSTRAINT ux_wallets_owner_id_name UNIQUE (owner_id, name);
