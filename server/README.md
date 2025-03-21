# Backend application for HobbyCom

# Table of Contents

- [Backend application for HobbyCom](#backend-application-for-hobbycom)
- [Table of Contents](#table-of-contents)
  - [Introduction](#introduction)
  - [Running the application](#running-the-application)
    - [Environment variables](#environment-variables)
    - [Set up database](#set-up-database)
      - [Generate database schema](#generate-database-schema)
  - [API Documentation](#api-documentation)
    - [Swagger UI](#swagger-ui)
  - [Deployment](#deployment)

## Introduction

This is a backend application for the hobbyCom app. The application is built using ASP.NET Core (C#). Built in collaboration with Nori Alija (NorAlija). Documentation is updated upon new features or bug fix.

## Running the application

To run the application locally

```sh
CD your-project-dir/server/ # moves to project root solution

dotnet clean && dotnet build # downloads all the necessary packages

#optionally open a new terminal
cd your-project-dir/server/HobbyCom.Presenter.API

dotnet watch
``` 

### Environment variables

To run the app, configure the required environment variables for this project. The environment variable are stored in `appsettings.Development.json` file found in ``Presenter.API`` layer but it is ignored so **<ins>please refer to the example one to see how to set your env varibles<ins>**. This will change later to user-secrets for local dev (recommended).

Below are the environment variables required for the application:

| Environment Variable                  | Description                                                                           |
| ------------------------------------- | ------------------------------------------------------------------------------------- |
| `Jwt:PublicKey`                       | -----BEGIN PUBLIC KEY-----keycontent----END PUBLIC KEY-----                           |
| `Jwt:Key`                             | -----BEGIN RSA PRIVATE KEY-----keycontent----END PUBLIC KEY-----                      |
| `Authentication:ValidIssuer`          | yourIssuer                                                                            |
| `Authentication:ValidAudience`        | yourAudience                                                                          |
| `Authentication:JwtSecret`            | this is a seceret from supabase. you might not need it if using different db provider |
| `ConnectionStrings:SessionConnection` | User Id=xx;Password=xx;Server=xxx;Port=xxx;Database=postgres (format might differ)    |
| `Supabase:Url`                        | YOUR_SUPABASE_URL  (if using supabase)                                                |
| `Supabase:Key`                        | YOUR_SUPABASE_KEY  (if using supabase)                                                |


The RSA PublicKey and Key(privateKey) are auto generated when the application runs. The can be found in the ``Presenter.API`` layer. These keys are used to generate and decode the JWT tokens. Note these keys are ignored and shouln't be pushed to version control.

Secrets during development should be saved with user-secrets. To set up the secretes run the below (note that you need to in the Presenter.API layer):

 ```bash
   dotnet user-secrets init # initiates user-secrets if not set already

   # set the secrets. Example
   dotnet user-secrets set "ConnectionStrings:localpostgresql" "Host=yourHost;Database=yourDB;Username=Username;Password=yourPSWD"

   ```

### Set up database

You can use a local postgreSQL server or use a cloud one example NeonDb or Supabase

#### Generate database schema

The schmas are created and push to the DB using Entity framework. (You can delete the Migration folder found in ``Infrastructure`` layer)

From the Solution root (PMS_Project) run:

Generate a new migration named for example InitialCreate based on the current model:
  ```bash
  dotnet ef migrations add InitialCreate --project HobbyCom.Infrastructure --startup-project HobbyCom.Presenter.API
  ```
  Apply the pending migrations to the database, creating the necessary tables and relationships.
  ```bash
  dotnet ef database update --project HobbyCom.Infrastructure --startup-project HobbyCom.Presenter.API
  ```
  Whenever there is a change to any of the schemas, create a new migration with a unique name and then update the databse as in the scripts above.

## API Documentation

### Swagger UI

After running the application, the API documentation can be accessed
from following url: [http://localhost:5050/swagger-ui/index.html](http://localhost:5140/swagger-ui/index.html)

The APIs can then called from the front-end as follow:

- [http://localhost:5050/v1/api/specifcendpoint](http://localhost:5140/v1/api/specifcendpoint) #see docs
  
for the frontend to use the endpoints use your machine IP. example:
- [http://192.xxx.x.xx:5050/v1/api/specifcendpoint](http://localhost:5140/v1/api/specifcendpoint)

## Deployment

To be Deployed
