CREATE TABLE IF NOT EXISTS tenants (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL
);

ALTER TABLE users
    ADD COLUMN IF NOT EXISTS is_super_user BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE patruljer
    ADD COLUMN IF NOT EXISTS tenant_id TEXT;

ALTER TABLE poster
    ADD COLUMN IF NOT EXISTS tenant_id TEXT;

ALTER TABLE points
    ADD COLUMN IF NOT EXISTS tenant_id TEXT;

CREATE TABLE IF NOT EXISTS user_tenants (
    user_id TEXT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    tenant_id TEXT NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    role_name TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (user_id, tenant_id)
);

CREATE INDEX IF NOT EXISTS idx_patruljer_tenant_id ON patruljer(tenant_id);
CREATE INDEX IF NOT EXISTS idx_poster_tenant_id ON poster(tenant_id);
CREATE INDEX IF NOT EXISTS idx_points_tenant_id ON points(tenant_id);
CREATE INDEX IF NOT EXISTS idx_user_tenants_tenant_id ON user_tenants(tenant_id);
CREATE INDEX IF NOT EXISTS idx_user_tenants_user_id ON user_tenants(user_id);

INSERT INTO tenants (id, name)
VALUES ('default-event', 'Default Event')
ON CONFLICT (id) DO NOTHING;

UPDATE patruljer SET tenant_id = 'default-event' WHERE tenant_id IS NULL;
UPDATE poster SET tenant_id = 'default-event' WHERE tenant_id IS NULL;
UPDATE points SET tenant_id = 'default-event' WHERE tenant_id IS NULL;

ALTER TABLE patruljer
    ALTER COLUMN tenant_id SET NOT NULL;

ALTER TABLE poster
    ALTER COLUMN tenant_id SET NOT NULL;

ALTER TABLE points
    ALTER COLUMN tenant_id SET NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_patruljer_tenant_id') THEN
        ALTER TABLE patruljer
            ADD CONSTRAINT fk_patruljer_tenant_id FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_poster_tenant_id') THEN
        ALTER TABLE poster
            ADD CONSTRAINT fk_poster_tenant_id FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_points_tenant_id') THEN
        ALTER TABLE points
            ADD CONSTRAINT fk_points_tenant_id FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE;
    END IF;
END $$;

INSERT INTO roles (id, name, normalized_name, description, concurrency_stamp)
SELECT gen_random_uuid()::text, 'SuperUser', 'SUPERUSER', 'Global access to all tenants', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE normalized_name = 'SUPERUSER');

INSERT INTO roles (id, name, normalized_name, description, concurrency_stamp)
SELECT gen_random_uuid()::text, 'Administrator', 'ADMINISTRATOR', 'Manage patruljer, posts and points', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE normalized_name = 'ADMINISTRATOR');

INSERT INTO roles (id, name, normalized_name, description, concurrency_stamp)
SELECT gen_random_uuid()::text, 'PostUser', 'POSTUSER', 'Can give points in tenant', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE normalized_name = 'POSTUSER');
