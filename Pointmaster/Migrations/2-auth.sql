CREATE TABLE IF NOT EXISTS roles (
    id TEXT PRIMARY KEY,
    name TEXT NULL,
    normalized_name TEXT NULL,
    concurrency_stamp TEXT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_roles_normalized_name
ON roles (normalized_name)
WHERE normalized_name IS NOT NULL;

CREATE TABLE IF NOT EXISTS users (
    id TEXT PRIMARY KEY,
    username TEXT NULL,
    normalized_username TEXT NULL,
    password_hash TEXT NULL,
    security_stamp TEXT NULL,
    concurrency_stamp TEXT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_normalized_username
ON users (normalized_username)
WHERE normalized_username IS NOT NULL;
