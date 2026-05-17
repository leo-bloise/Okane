CREATE TABLE IF NOT EXISTS ledger (
	id SERIAL PRIMARY KEY,
	amount NUMERIC(19, 4) NOT NULL,
	description TEXT,
	user_id BIGINT NOT NULL REFERENCES users(id),
	fingerprint CHAR(64) NOT NULL,
	occured_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	CONSTRAINT unique_fingerprint UNIQUE (user_id, fingerprint)
);