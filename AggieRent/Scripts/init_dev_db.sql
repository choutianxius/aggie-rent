-- PostgreSQL

DROP TABLE IF EXISTS "admin";
DROP TABLE IF EXISTS "owner" CASCADE;
DROP TABLE IF EXISTS "applicant" CASCADE;
DROP TABLE IF EXISTS "apartment" CASCADE;
DROP TABLE IF EXISTS "apartment_applicant";
DROP TABLE IF EXISTS "wished_apartment";

DROP TYPE IF EXISTS us_state;
DROP TYPE IF EXISTS gender_type;

CREATE TYPE us_state AS ENUM (
    'alabama',
    'alaska',
    'arizona',
    'arkansas',
    'california',
    'colorado',
    'connecticut',
    'delaware',
    'florida',
    'georgia',
    'hawaii',
    'idaho',
    'illinois',
    'indiana',
    'iowa',
    'kansas',
    'kentucky',
    'louisiana',
    'maine',
    'maryland',
    'massachusetts',
    'michigan',
    'minnesota',
    'mississippi',
    'missouri',
    'montana',
    'nebraska',
    'nevada',
    'new_hampshire',
    'new_jersey',
    'new_mexico',
    'new_york',
    'north_carolina',
    'north_dakota',
    'ohio',
    'oklahoma',
    'oregon',
    'pennsylvania',
    'rhode_island',
    'south_carolina',
    'south_dakota',
    'tennessee',
    'texas',
    'utah',
    'vermont',
    'virginia',
    'washington',
    'west_virginia',
    'wisconsin',
    'wyoming'
);

CREATE TYPE gender_type AS ENUM
(
    'not_set',
    'female',
    'male',
    'non_binary'
);

CREATE TABLE IF NOT EXISTS "admin"
(
    id VARCHAR(255) PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    hashed_password VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS "owner"
(
    id VARCHAR(255) PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    hashed_password VARCHAR(255) NOT NULL,
    "name" VARCHAR(255) NOT NULL,
    "description" TEXT
);

CREATE TABLE IF NOT EXISTS apartment
(
    id VARCHAR(255) PRIMARY KEY,
    owner_id VARCHAR(255) NOT NULL REFERENCES "owner" (id) ON DELETE CASCADE,
    last_edited_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "name" VARCHAR(255) NOT NULL,
    "description" TEXT,
    "size" DECIMAL NOT NULL CHECK ("size" >= 0.0),
    bedrooms INTEGER NOT NULL CHECK (bedrooms > 0),
    bathrooms INTEGER NOT NULL CHECK (bathrooms > 0),
    apt_count INTEGER NOT NULL DEFAULT 1 CHECK (apt_count > 0),
    photo_urls TEXT[] DEFAULT '{}',
    available_from DATE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    available_to DATE NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    CONSTRAINT valid_available_range CHECK (available_from <= available_to),
    address_line1 TEXT NOT NULL,
    address_line2 TEXT NOT NULL,
    city VARCHAR(255) NOT NULL,
    "state" us_state NOT NULL,
    zip_code VARCHAR(255) NOT NULL,
    occupied BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS "applicant"
(
    id VARCHAR(255) PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    hashed_password VARCHAR(255) NOT NULL,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    gender gender_type NOT NULL DEFAULT 'not_set',
    birthday DATE,
    "description" TEXT,
    occupied_apartment_id VARCHAR(255) REFERENCES apartment (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS apartment_applicant
(
    apartment_id VARCHAR(255) NOT NULL REFERENCES apartment (id) ON DELETE CASCADE,
    applicant_id VARCHAR(255) NOT NULL REFERENCES applicant (id) ON DELETE CASCADE,
    PRIMARY KEY (apartment_id, applicant_id)
);

CREATE TABLE IF NOT EXISTS wished_apartment
(
    applicant_id VARCHAR(255) NOT NULL REFERENCES applicant (id) ON DELETE CASCADE,
    apartment_id VARCHAR(255) NOT NULL REFERENCES apartment (id) ON DELETE CASCADE,
    PRIMARY KEY (apartment_id, applicant_id)
);

-- Intialize table data below
