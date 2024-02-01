# Movie App

## Introduction

The MovieApp in ASP.NET Core Web App (MVC).

## Table Of Contents

- [Movie App](#Movie-App)
  - [Introduction](#introduction)
  - [Table Of Contents](#table-of-contents)

- [Usage](#usage)
  - [Prerequisites for building the project](#prerequisites-for-building-the-project)
  - [Building the solution](#building-the-solution)
  - [Running the application](#running-the-application)
- [Architecture and Design Decisions](#architecture-and-design-decisions) 
  - [The Data Project](#the-data-project-moviesdata)
  - [The Web Application](#the-web-application-moviesweb) 
  - [The Test Projects](#the-test-projects)
  - [Concurrency Conflicts](#concurrency-conflicts)
- [Patterns Used](#patterns-used)

# Usage

## Prerequisites for building the project:
* The [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) with the `ASP.NET` and web development workload.

## Building the solution

The solution contains the release assets in the file `assets.zip`. The assets can be unzipped and this section can be skipped.

To build and run the project, open a command prompt to the root of the solution, and perform the following: 

1. Run tests

       dotnet test


2. Publish the Web project to a folder:

    ```
    cd ./src/Movies.Web
    dotnet publish -o ../../assets/web
    cd ../..
    ```
3. Run the `scripts.sql` to create the database and related tables. *NB:* The solution uses the Express edition of the SQL Server. The connection string can be found inside `appsettings.json` and be modified to any valid connection string.

## Running the application

1. To run the application, open command prompt to the `assets` folder created from the previous step (should be at the root of the solution), or extracted from the `assets.zip`, and run the executable

   - From the Web app Command Prompt:

        ```
        cd ./web
        dotnet Movies.Web.dll --urls "https://localhost:50423"
        ```

  3. Open your favourite browser and visit `https://localhost:50423`

# Architecture and Design Decisions

The application provides loosely coupled access to the data, i.e. the web frontend has no idea how to retrieve the underlying data. This allows us to switch to another service (e.g. an external API) without interrupting our application.

## The Data Project (`Movies.Data`)

This houses the infrastructure for the data access.

## The Web Application (`Movies.Web`)

This is the entry point for the web front-end application. This is the ASP.NET Core MVC web application, which is responsible for the UI logic. The app configurations are contained in the `appsettings.json` file.

## The Test Projects

These house our project tests.

## Concurrency Conflicts

The system implements a simple concurrency via concurrency token, `Version`. i.e. The data modification fail on save if the data has changed since it was queried.

In the code below, the `[Timestamp]` attribute maps a property to a SQL Server `rowversion` column. Since `rowversion` automatically changes when the row is updated, this provides a very useful concurrency token with minimum-effort that protects the entire row.

    public class Movie
    {
      public string Id { get; set; }
      public string MovieName { get; set; } = default!;

      [Timestamp]
      public byte[] Version { get; set; }
    }

# Patterns Used

The solution architecture uses the CQRS pattern (Command and Query Responsibility Segregation) together with the Unit of Work pattern.

**CQRS pattern**_: Separates write commands from read queries.

**Unit of Work pattern**_: Manages transactions and ensures that multiple operations are treated as a single logical unit, i.e. they should all succeed or fail. This guarantees data integrity and consistency in applications.