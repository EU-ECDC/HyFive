# HyFive Deployment Overview

This guide provides instructions for deploying HyFive using containers on 
kubernetes. It covers environment setup, database configuration, and user 
management integration.

## Review the configuration

Regardless of deployment option, there are a number of things that need to be
configured for the solution to run. Where possible, sane defaults are used to
minimize the amount of configuration needed.

The full configuration options for the Observation and Admin modules are 
available in their respective `settings.json` files, located at:

 - Observation: [HyFive.Observation/appsettings.json](../../HyFive.Observation/appsettings.json)
 - Admin: [HyFive.Admin/appsettings.json](../../HyFive.Admin/appsettings.json)

These are standard .NET settings and can be overwritten with environment 
variables by using `__` to join nested json settings. 

ex.
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
    }
  }
}
```
can be overwritten like:
```yaml
env:
  Logging__LogLevel__Default: "Warning"
```
to change the default log level from `Information` to `Warning`.

### Configuring the database connection

The HyFive system uses a PostgreSQL database for data storage. The solution has
been tested using PostgreSQL 16, but is likely to work with newer versions as 
well.
Both the Obervation and Admin containers expect a connection string as part of
the settings to connect to this database. This is a standard PostgreSQL 
connection string, commonly formatted like 
`Server=; Database=; Username=; Password=`. A full list of connection string 
options is available in the 
[npgsql documentation](https://www.npgsql.org/doc/connection-string-parameters.html).

#### Configuring the database connection for helm

If deploying the PostgreSQL database in the cluster using the helm chart, no 
configuration is needed. Username and database name may be changed but the 
host will be set by helm, and the password can be left empty to generate a 
secure password.

When using a predeployed database, add the connection credentials in the 
[values](../../infra/helm-chart/values.yaml) file before deploying. 

**Note**: Adding secrets to the values file requires the values file to be 
kept secure.

### Configuring Single Sign-On

To enable integration with organization Single Sign-On (SSO) systems, HyFive 
user access is configured using OpenID Connect (OIDC). The following settings
need to be provided to connect to an OIDC provider:

```json
"OpenIdConnect": {
  "AuthUse": "true",
  "Authority": "",
  "ClientId": "",
  "ClientSecret": ""
},
```

The `Authority`, `ClientId`, and `ClientSecret`values need to be obtained from 
the SSO system and set here. Setting these values will connect the HyFive 
system to the SSO provider. A few more settings are required to inform HyFive 
which values should be used for authorization though.

First of all are the claim types, which describe to HyFive which values should
be used to infer access role, environment access, user display name, and user 
unique ID.

```json
"ClaimTypes": {
  "RoleClaimType": "http://sso/role/atom",
  "EnvironmentAccessClaimType": "http://sso/supo/env",
  "DisplayNameClaimType": "http://sso/displayName",
  "UniqueIdentifierClaimType": "http://sso/guid"
},
```

These are combined with the following settings to configure which users have 
which access:

```json
  "Security": {
    "EnvironmentAccess": "DEV",
  }
```

Sets the value which needs to be present in the field referenced in the 
`ClaimTypes.EnvironmentAccessClaimType` setting to have access to the deployed
environment. 

ex.

```json
"ClaimTypes": {
  "EnvironmentAccessClaimType": "http://sso/supo/env",
},
"Security": {
  "EnvironmentAccess": "DEV",
}
```
means that the value at `http://sso/supo/env` in the user JWT needs to be set
to `"DEV"` to have access to the environment. 

#### Redirect pages

In addition to the OpenID Connect settings above, HyFive also requires
configuration of the login and logout redirect URIs. These are used by the
identity provider to return the user to the application after authentication
and to direct the user to a landing page after logout.

These values must be configured for **both the Admin and Observation** projects.

```yaml
RedirectPagesSettings__RedirectLogInUri: "https://<your-admin-domain>/signin-oidc"
RedirectPagesSettings__RedirectLogOutUri: "https://<your-admin-domain>/"

RedirectPagesSettings__RedirectLogInUri: "https://<your-observation-domain>/signin-oidc"
RedirectPagesSettings__RedirectLogOutUri: "https://<your-observation-domain>/"
```

RedirectLogInUri: The callback URI where the identity provider should
send the user after a successful login. For both modules this is normally the
/signin-oidc endpoint.

RedirectLogOutUri: The page the user should be redirected to after logging
out. This is usually the root of the respective application or a public landing page.

Choosing the domain:

Replace <your-admin-domain> and <your-observation-domain> with the actual
publicly accessible domain names (or load balancer / ingress hostnames) for the
Admin and Observation deployments in your environment.

**Important**: These values must exactly match the redirect URIs registered
for the application in the Single Sign-On system. If they differ, sign-in and
sign-out will fail.

## Deploy the Database

HyFive supports three database deployment modes:

 - Local: Run the database on the same host as the application.
 - Remote: Connect to an existing remote database.
 - Container: Deploy the database in a container.

The available helm charts can deploy a postgresql database that is sufficient 
for development, allowing for quick deployment and configuration. This helm 
chart isnot configured for high availability and does not include backups 
though. So 
**For production deployments, we recommend using a remote managed database.**

If you are making a local database deployment, the best resource for install 
information is [the official site](https://www.postgresql.org/download/).

## Set up single sign-on user management

HyFive is configured to use Single Sign-On (SSO) user management using OpenID
Connect (OIDC). This means that users are not handled inside HyFive itself, they
are handled inside a separate system, such as PingOne, Google Identity Platform,
or Microsoft Entra ID. HyFive is added as a trusted app in the this identity 
platform, and HyFive is configured to trust the provider. 

This step is important but as SSO providers have different interfaces, we can 
only provide general information.

The setup will be something like this:

 - Create an app registration for HyFive in the SSO system.
 - Make sure that user groups are created for HyFive Observers and Administrators (and users added to these groups)
 - Make sure that SSO values are available for:
   - user role
   - user environment access 
   - user display name
   - user unique id
 - Configure HyFive with the corresponding values

The Environment Access value is mainly used if there are multiple environments
(such as when running one production system and one training system) but is 
required even if there is only one system deployed.

## Deploy the containers

### Running locally with compose

The solution contains the [docker-compose.yml](../../docker-compose.yml) file,
which can be used to run a local version of the system using a container 
orchestrator. This has been tested using docker compose and podman compose.

to start the environment using docker:
```bash
docker compose up
```

or using podman:
```bash
podman compose up 
```

This should result in the system becoming available at http://localhost:8080.

Note that the compose project does not include a Single Sign-On (SSO) provider,
so user management needs to be plugged in.

### Deploying the containers on Kubernetes using Helm

These instructions assume that you are interfacing with your kuberentes cluster
using the command line interfaces. If you use a different interface, adapt the 
instructions.

1. Ensure the Kubernetes target is available and reachable.

```bash
kubectl get pods
```
This command should list the available pods running on the kuberenetes cluster.
What is running is not important, just that you can connect to the cluster.

2. Helm is installed.

```bash
cd infra/helm-chart
helm -h
```

This should show the helm help screen. If helm isn't installed, follow the 
instructions at [helm.sh](https://helm.sh/docs/intro/install/) to install helm,
then continue following the instructions.

3. Edit `values.yaml` with your settings.

The [values.yaml`](../../infra/helm-chart/values.yaml) file is used to populate
the helm templates when deploying, thus they need to be updated with the 
settings for your deployment. The file itself includes comments to help you 
configure. See the [Configuration](configuration.md) instructions for additional
help.

**Note that if password or keys are added to the `values.yaml` file, it needs to
be handled as a sensitive data and should be kept securely.**

4. Deploy

Run:
```bash
helm upgrade --install <name> . --values values.yaml
```
where `<name>` is the name of your deployment. This is commonly the name of the
environment that's being deployed to, such as `dev`. The `<name>` value will be 
used as a prefix for all the deployed resources, allowing multiple instances to
be deployed if needed.

5. Monitor pods and services for successful startup.

There are several way to monitor the deployment procedure, but a simple one is
running:

```bash
watch kubectl get pods
```
which will show you the running containers, updating every 3 seconds. 
Should the containers fail to deploy, review the settings and then run the 
upgrade command again to update the settings.

General information for troubleshooting is available in the 
[Kubernetes documentation](https://kubernetes.io/docs/tasks/debug/debug-application/)
as well.
