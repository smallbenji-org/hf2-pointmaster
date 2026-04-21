CREATE TABLE IF NOT EXISTS poster (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS patruljer (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS points (
    id SERIAL PRIMARY KEY,
    points INT,
    turnout INT,
    patrulje_id INT,
    post_id INT
);