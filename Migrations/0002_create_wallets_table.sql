-- migrate:up
CREATE TABLE wallets
(
    id         uuid         PRIMARY KEY,
    owner_id   uuid         NOT NULL REFERENCES users (id),
    name       varchar(200) NOT NULL,
    kind       varchar(20)  NOT NULL CHECK (kind IN ('Standard', 'External')),
    status     varchar(20)  NOT NULL CHECK (status IN ('Active', 'Archived')),
    created_at timestamptz  NOT NULL
);

CREATE INDEX ix_wallets_owner_id ON wallets (owner_id);
