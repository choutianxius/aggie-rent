-- PostgreSQL

DROP TABLE IF EXISTS "user";

DROP TYPE IF EXISTS user_role;

CREATE TYPE user_role AS ENUM ('User', 'Admin');

CREATE TABLE IF NOT EXISTS "user"
(
    user_id VARCHAR(255) PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    hashed_password VARCHAR(255) NOT NULL,
    "role" user_role NOT NULL DEFAULT 'User'
);

INSERT INTO "user" VALUES
(
    'b1e02e51-073e-46e6-94ea-cbf8a843dadb', -- A random Guid
    'admin@tamu.edu',
    '$2a$10$Xii7sw1Zr0zkr3DDvx9yBenUXyrTdA.VMIFtqSzmeSio9E4Uox5Ye', -- @dm1nP@ssw0rd
    'Admin'
);