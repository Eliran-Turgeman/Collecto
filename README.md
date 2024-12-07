<h1>
  <img src="https://github.com/user-attachments/assets/013fbaa7-02c0-4373-b8ba-d217f556da13" alt="Collecto Logo" width="30" style="vertical-align: middle;"/>
  Collecto
</h1>

![Docker Pulls](https://img.shields.io/docker/pulls/elirant/collecto)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/Eliran-Turgeman/Collecto)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Eliran-Turgeman/Collecto/test.yml)


**Collecto** is an open-source, self-hosted, lightweight email collection service designed to securely gather and manage your audience's email addresses. With Collecto, you can integrate email capture forms into your websites, apps, or landing pages without relying on external SaaS platforms. Our goal is to give you full control, transparency, and ease of use right from day one.  

Docs available at [https://www.16elt.com/Collecto/](https://www.16elt.com/Collecto/)

## Demo (Nov 24)

https://github.com/user-attachments/assets/6c74680b-e4d4-43f0-965d-419cd78ac671



## Installation

### Clone the Repository

```bash
git clone https://github.com/Eliran-Turgeman/Collecto.git
cd Collecto
```

### Setup & Running the Application

Collecto uses Docker Compose to run its services, including the email collection API and Redis for caching. To get the application up and running, follow these steps:

1. Ensure you are in the project directory where the docker-compose.yml file is located.
2. Run the following command to build and start the services:

Prod (using the latest image from DockerHub):
```bash
docker-compose up --build
```

Local/Dev (building Dockerfile):
```bash
docker-compose --file .\docker-compose.dev.yml up --build
```


Once the services are up and running:

* The Collecto API will be available at [http://localhost:5001](http://localhost:5001)
* You can access the Swagger UI (for API testing and exploration) at [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html)

To stop the running containers, use one of the following commands, based on how you started the application:

```bash
docker-compose down
```

```bash
docker-compose --file .\docker-compose.dev.yml down --build
```

