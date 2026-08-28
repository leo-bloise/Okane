CREATE TABLE users
(
    id            uuid         PRIMARY KEY,
    name          varchar(200) NOT NULL,
    email         varchar(320) NOT NULL UNIQUE,
    password_hash text         NOT NULL,
    created_at    timestamptz  NOT NULL
);
