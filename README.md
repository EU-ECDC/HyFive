# HyFive 

A tool for digital hand hygiene observation collection.
The tool consists of two parts: Admin and Observation.

If you have reached this repository through a public search, we strongly advise that you reach out to ECDC to get full integration support.
If you are interested in the hyFive tool and would like to deploy it in your setting, please contact us at hyfive@ecdc.europa.eu. 

## Observation

An interface where observers (Observer) can log in with using Single Sign-On.

An Observer can:
- View their associated institutions
- Perform various types of observations (Five indications, Glove use, Hand jewelry, Protective equipment) on their institutions' departments
- Submit observations in the form of Sessions to the institution's "Coordinator"


## Admin
An administrative interface where administrators "Admin" and assigned coordinators "Coordinator" can log in.

"FHIdmin" can:
- Edit code sets
- View all observation data
- Edit institutions
- Create/edit "Coordinator" users and "Observer" users for institutions.

"Coordinator" can:
- View their institution(s)' observations
- Submit observation sessions
- Create Observers and Coordinators for their institution(s)


## Authentication
Authentication is performed using Single Sign-On using the OIDC protocol.

## Authorization
Authorization is performed using information issued in a OIDC token together with data stored in a user table (User) with distinguished user types for "Admin", "Coordinator" and "Observer".
Each API endpoint is secured using a policy.
Where necessary, additional access control is performed by inspecting the API request and denying access based on query parameters, e.g. if a Coordinator tries to query data from an institution they are not a Coordinator for.

## Seeding the application
On initial setup of the application, it must be seeded with data.
This is done by calling the Seed method in the API of the HyFive.Observation solution.
1. Start up the HyFive.Observation solution
2. Navigate to '/swagger'
3. Scroll down to the 'Seed' method and run it.

## Container Compose environment

A compose environment is included with the project, which can be used as a 
development environment and for testing the solution.

The environment has been tested with docker and podman.

To start the environment, run the appropriate compose command for your container
engine.
ex.
```bash
podman compose up
```

This will make the system availble, binding to localhost to avoid problems with
SSO url resolution. The following endpoints will be made available.
 - http://admin.localhost:8001
 - http://observation.localhost:8000
 - http://auth.localhost:8080

Once the system has spun up, it is pre-configured with the following users:
for HyFive:
 - observer:observer
 - admin:admin

for the Keycloak SSO container:
 - admin:admin

Note that no authorizations are created by default in the system, these must be
added as noted in the [DEPLOYMENT](docs/DEPLOYMENT.md) file.
