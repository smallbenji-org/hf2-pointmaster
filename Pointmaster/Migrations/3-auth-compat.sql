ALTER TABLE roles
    ADD COLUMN IF NOT EXISTS concurrency_stamp TEXT;

ALTER TABLE users
    ADD COLUMN IF NOT EXISTS security_stamp TEXT;

ALTER TABLE users
    ADD COLUMN IF NOT EXISTS concurrency_stamp TEXT;

CREATE UNIQUE INDEX IF NOT EXISTS ux_roles_normalized_name
ON roles (normalized_name)
WHERE normalized_name IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_normalized_username
ON users (normalized_username)
WHERE normalized_username IS NOT NULL;
