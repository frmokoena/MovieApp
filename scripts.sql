IF DB_ID (N'moviesdb') IS NOT NULL
DROP DATABASE [moviesdb];
GO

CREATE DATABASE [moviesdb];
GO

USE [moviesdb];
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Actors] (
    [ActorID] int NOT NULL IDENTITY,
    [ActorName] varchar(50) NOT NULL,
    [ActorDOB] datetime2 NOT NULL,
    [Version] rowversion NOT NULL,
    CONSTRAINT [PK_Actors] PRIMARY KEY ([ActorID])
);
GO

CREATE TABLE [Genres] (
    [GenreID] int NOT NULL IDENTITY,
    [GenreName] varchar(50) NOT NULL,
    [Version] rowversion NOT NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY ([GenreID])
);
GO

CREATE TABLE [Movies] (
    [MovieID] int NOT NULL IDENTITY,
    [MovieName] varchar(50) NOT NULL,
    [ReleaseYear] int NOT NULL,
    [Genre] int NOT NULL,
    [Version] rowversion NOT NULL,
    CONSTRAINT [PK_Movies] PRIMARY KEY ([MovieID]),
    CONSTRAINT [FK_Movies_Genres_Genre] FOREIGN KEY ([Genre]) REFERENCES [Genres] ([GenreID])
);
GO

CREATE TABLE [Characters] (
    [ActorID] int NOT NULL,
    [MovieID] int NOT NULL,
    [CharacterName] varchar(50) NOT NULL,
    [Version] rowversion NOT NULL,
    CONSTRAINT [PK_Characters] PRIMARY KEY ([ActorID], [MovieID]),
    CONSTRAINT [FK_Characters_Actors_ActorID] FOREIGN KEY ([ActorID]) REFERENCES [Actors] ([ActorID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Characters_Movies_MovieID] FOREIGN KEY ([MovieID]) REFERENCES [Movies] ([MovieID]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Actors_ActorName] ON [Actors] ([ActorName]) INCLUDE ([ActorDOB]);
GO

CREATE UNIQUE INDEX [IX_Characters_CharacterName_MovieID] ON [Characters] ([CharacterName], [MovieID]);
GO

CREATE INDEX [IX_Characters_MovieID] ON [Characters] ([MovieID]);
GO

CREATE UNIQUE INDEX [IX_Genres_GenreName] ON [Genres] ([GenreName]);
GO

CREATE INDEX [IX_Movies_Genre] ON [Movies] ([Genre]);
GO

CREATE INDEX [IX_Movies_MovieName] ON [Movies] ([MovieName]) INCLUDE ([ReleaseYear], [Genre]);
GO

COMMIT;
GO