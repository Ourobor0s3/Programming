CREATE TABLE IF NOT EXISTS books (
    isbn   VARCHAR(32) PRIMARY KEY,
    title  TEXT NOT NULL,
    author TEXT NOT NULL,
    year   INTEGER,
    pages  INTEGER
);