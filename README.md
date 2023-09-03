# RESThooks on Microsoft Service Fabric

<img align="center" alt="RESThooks" width="100%" src="./docs/Architecture.jpg" />

This is a flavor of [RESThooks][resthooks] implementation on Microsoft Service Fabric.

> Originally, this was created to tryout a scalable publisher/subscriber design around a message broker like [RabbitMQ][rmq] or [Azure Service Bus Topics][asb-topics].

[resthooks]: https://resthooks.io
[rmq]: https://
[asb-topics]: https://

### What's included:

In case you were wondering:

- Originally, written on a Mac with Windows 10 in Colombo, Sri Lanka.
- Written in C# with Visual Studio 2022.
- Built on .NET Core 3.1 with Service Fabric.
- Pub/Sub on RabbitMQ, thanks to MassTransit and Docker.
- Stored as JSON-data on Azure SQL, thanks to EF Core and Docker.
- Served with Mock Servers on Postman.



## Build and Run from the Source

If you are completely new to .NET and Service Fabric, the [.NET Learning Center][ms-docs-dotnet-learning-center] is a good source of information. First, you want to ensure [Service Fabric Local Cluster][sf-local-devenv-setup] is up and running.

[ms-docs-dotnet-learning-center]: https://dotnet.microsoft.com/en-us/learn
[sf-local-devenv-setup]: https://learn.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started



With SF Local Cluster Manager:

- Reset Local Cluster.
- Switch Cluster Mode to `1 Node`, and
- Start Local Cluster.



Then you can build and run from the source, like you typically do for any other .NET project.

With Visual Studio:

- Set Startup Project to `src/Host/Skol.Resthooks.Fabric.sfproj`.
- Ensure Solution Platform is set to `x64`.
- Press `F5` to launch the project.



## Emulate Azure SQL and RabbitMQ with Docker

First, you want to ensure Docker is up and running. Then you'll be able to spin up Azure SQL for data and RabbitMQ for messaging. Luckily, `.devcontainer/` directory contains the `compose-dev.yml` script.

With Terminal:

- Run `docker info` to verify Docker is up and running.

- Run `docker compose -f .devcontainer/compose-dev.yml up -d`.

Next, you want to create a database with the seed data.

With Azure Data Studio:

- Connect to SQL Server `localhost`.
- Create a new database `Rh_IntentsDb`.

- Open the Database Project in `db/Skol.Resthooks.SqlDb` directory, and
- Publish.



## Mocking Backend APIs with Postman

Finally, you want to receive notifications and for that I am using Postman as a mock server. _Mocking with the Postman API_ is a good source to setup a mock server.

With VS Code:

- Create a .env file inside the `tests/e2e/` directory, and
- Paste the `HOST` and the `HOST_API_KEY` of your Mock Server.

That's it. Now you can create a few endpoints on Postman.



## Things to Tryout

First, you want a RESTful API client, like Postman to subscribe for a few topics to receive notifications. 

With VS Code:

1. Install [REST-client extension][vscode-rest-client-ext].
2. Open `Resthooks.Subs.http` file.
3. Change the `notification_url` in the `POST` request payload, and
4. Click `Send Request` to subscribe.

[vscode-rest-client-ext]: https://marketplace.visualstudio.com/items?itemName=humao.rest-client



Next, you want to activate the subscriptions.

With Postman:

1. Copy the `activation_code` you received from the mock server.

With VS Code:

1. Paste and replace the `activation_code` in the `PATCH` request, and
2. Click `Send Request`.



That's it. Now you can keep creating endpoints, subscribing for various topics, keep publishing to `localhost:8172/v1/messaging/publish` endpoint, and observe them via Postman logs.



## License

Copyright (c) Alertbox Inc. All rights reserved.

The source code is license under the [MIT license](LICENSE).

