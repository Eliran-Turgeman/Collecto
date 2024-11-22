<h1>
  <img src="https://github.com/user-attachments/assets/013fbaa7-02c0-4373-b8ba-d217f556da13" alt="Collecto Logo" width="30" style="vertical-align: middle;"/>
  Collecto
</h1>


Collecto is an open-source, self-hosted, lightweight, email collection service.  
Docs available at [https://www.16elt.com/Collecto/](https://www.16elt.com/Collecto/)

Latest Demo (Nov 24) : [https://youtu.be/H6m5f3WL2x0?si=GwT1bT6upA_EVIjk](https://youtu.be/H6m5f3WL2x0?si=GwT1bT6upA_EVIjk)

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

