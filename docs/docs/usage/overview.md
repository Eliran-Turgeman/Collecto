# Usage Overview

## Prerequisites

Before you get started, ensure you have the following installed on your system:

**Git**: Used for cloning the repository. Install Git from [here](https://git-scm.com/downloads).  
**Docker**: Used for running the application in containers. Install Docker from [here](https://docs.docker.com/engine/install/).  
**Docker** Compose: Ensure Docker Compose is installed as part of your Docker installation. If not, refer to [here](https://docs.docker.com/engine/install/)  

## Clone the Repository

```bash
git clone https://github.com/Eliran-Turgeman/Collecto.git
cd Collecto
```

## Setup & Running the Application

Collecto uses Docker Compose to run its services, including the email collection API and Redis for caching. To get the application up and running, follow these steps:

1. Make sure you are in the project directory where the docker-compose.yml file is located.
2. Run the following command to build and start the services:

```bash
docker-compose up --build
```

Once the services are up and running:

* The Collecto API will be available at: http://localhost:5001
* You can access the Swagger UI (for API testing and exploration) at: http://localhost:5001/swagger/index.html

To stop the running containers, use the following command:

```bash
docker-compose down
```


## Environment Variables

Create a `.env` file in the solution root and make sure you configure the following variables:

- **EMAIL_FROM** - Email confirmations will be sent from this address
- **EMAIL_SMTP_SERVER** - SMTP server that will be used to deliver confirmation emails (i.e smtp.gmail.com)
- **EMAIL_USERNAME** - Your email address, in most cases should be identical to **EMAIL_FROM**
- **EMAIL_PASSWORD** = Password to authenticate to your email account.
- **VALID_CORS_ORIGINS** = In order to make API calls to collecto from your site, enter your site url here.
Can be left empty in case you don't have a site url yet.
Supports multiple origins separated by a comma (",").
- **COLLECTO_DOMAIN** = If you deployed collecto, you can populate this field so that links from emails are redirected correctly. (defaults to localhost:5001)

## Configuration

### Rate Limiting

The IpRateLimiting section in the `appsettings.json` file defines how Collecto handles requests to protect against abuse or spamming by limiting the number of requests that can be made within a specific time period.

```json
"IpRateLimiting": {
  "EnableEndpointRateLimiting": true,
  "StackBlockedRequests": false,
  "RealIpHeader": "X-Real-IP",
  "ClientIdHeader": "X-ClientId",
  "HttpStatusCode": 429,
  "GeneralRules": [
    {
      "Endpoint": "*:/api/EmailSignups",
      "Period": "1m",
      "Limit": 10
    }
  ]
}
```

* **EnableEndpointRateLimiting**: This enables rate limiting for specific endpoints. In this case, rate limiting is applied to the /api/EmailSignups endpoint.

* **StackBlockedRequests**: This setting controls whether requests that hit the rate limit are "stacked" and retried automatically. In this case, it’s set to false, meaning that blocked requests won’t be automatically retried.

* **RealIpHeader**: This defines the header to check for the real IP address. This is useful when the application is behind a proxy or load balancer, where the IP address in the request might be that of the proxy, not the client.

* **ClientIdHeader**: Defines a custom header (X-ClientId) that can be used to identify the client making the request, which can be useful for applying rate limits based on specific users.

* **HttpStatusCode**: Specifies the HTTP status code to return when a client exceeds the allowed request limit. In this case, 429 indicates "Too Many Requests."

* **GeneralRules**: This is an array of rate-limiting rules. In this example:
    * **Endpoint**: *:/api/EmailSignups applies the rule to any HTTP method (GET, POST, etc.) on the /api/EmailSignups endpoint.
    * **Period**: 1m specifies a 1-minute period.
    * **Limit**: 10 specifies that a maximum of 10 requests can be made from a single IP address to this endpoint in 1 minute.

### Feature Toggles

The FeatureToggles section in the `appsettings.json` file allows you to enable or disable specific features within the application without modifying the codebase. This is useful for rolling out new features incrementally or for A/B testing.

```json
"FeatureToggles": {
  "EmailConfirmation": true
}
```

* **EmailConfirmation**: When set to true, this enables the email confirmation feature, meaning:
    1. Users will need to confirm their email address as part of the signup process.
    2. When someone signs up to a form you created, they will have to confirm the signup.