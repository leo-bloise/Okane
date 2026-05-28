CREATE TABLE IF NOT EXISTS ledger (
	id SERIAL PRIMARY KEY,
	amount NUMERIC(19, 4) NOT NULL,
	description TEXT,
	user_id BIGINT NOT NULL REFERENCES users(id),
	from_account BIGINT NOT NULL REFERENCES accounts(id),
	to_account BIGINT NOT NULL REFERENCES accounts(id),
	occured_at TIMESTAMP NOT NULL
);