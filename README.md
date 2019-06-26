# Cognitive Pipeline

Azure serverless-based architecture to process files through a cognitive pipeline with real-time-communication callback

|Service|Build|Release|
|-|-|-|
|CognitivePipeline.Functions|[![Build Status](https://gbb-appinnovation.visualstudio.com/CognitivePipeline/_apis/build/status/mohamedsaif.CognitivePipeline?branchName=master)](https://gbb-appinnovation.visualstudio.com/CognitivePipeline/_build/latest?definitionId=26&branchName=master)|![Release Status](https://gbb-appinnovation.vsrm.visualstudio.com/_apis/public/Release/badge/a1da55f8-e784-413b-a0fb-465cdd253ac9/1/1)|
|CognitivePipeline.RTC|[![Build Status](https://gbb-appinnovation.visualstudio.com/CognitivePipeline/_apis/build/status/mohamedsaif.CognitivePipeline?branchName=master)](https://gbb-appinnovation.visualstudio.com/CognitivePipeline/_build/latest?definitionId=26&branchName=master)|![Release Status](https://gbb-appinnovation.vsrm.visualstudio.com/_apis/public/Release/badge/a1da55f8-e784-413b-a0fb-465cdd253ac9/1/2)|

## Overview

A customer requested assistance in creating a processing pipeline for uploaded files leveraging mainly [Azure Cognitive Services](https://azure.microsoft.com/en-us/services/cognitive-services/) to attach rich attributes to that file. These data can be then used to search the data and/or plot rich visual interfaces.

Solution requirements:

1. Support multiple file types (images, videos, PDF,...)
2. Support multi-step processing per file (like performing face detection, OCR and object detection on a single image)
3. Operations must be async
4. Support having call back when the async processing of a file is finished
5. Scalable implementation to have low cost when idling and adequate power when needed
6. Expandable architecture to add additional capabilities in the future
7. Ability to easily search of the attributes generated from the cognitive processing pipeline
8. Both build and release must be automated via solid DevOps CI/CD pipelines

## Architecture

The start of the show is [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/) 🤩! with cost efficiency and scalability requirements met without worrying too much about the infrastructure behind the scene.

Later on I will discuss the Azure components in more details, for now let's have a look at the architecture:

![Architecture](res/architecture.png)

Let's discuss the steps for the 2 main scenarios:

### 1. Cognitive Pipeline Processing

Below is the steps highlighted in the architecture diagram showing what is happing when a user submit a new file with instructions for processing.

1. User submit a new Cognitive File request ([CognitiveFile](src/CognitivePipeline/CognitivePipeline.Functions/Models/CognitiveStep.cs) json + file data) to [NewCognitiveReq](src/CognitivePipeline/CognitivePipeline.Functions/Functions/NewCognitiveReq.cs) function (assuming that the client already authenticated with Azure AD and have the appropriate API Management subscription key)
2. ```NewCognitiveReq``` function would validate then upload file to storage and update Cosmos Db with the new cognitive instructions.
3. NewCognitiveReq function will then put a new message in ```newreq``` storage queue for the async file processing.
4. A [durable function](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview) orchestrator named [CognitivePipelineOrchestrator_QueueStart](src/CognitivePipeline/CognitivePipeline.Functions/Orchestrator/CognitivePipelineOrchestrator.cs#L34-L52) is triggered by the ```newreq``` queue new cognitive file processing request.
5. Using [Fan-Out-Fan-In durable function pattern](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-concepts#patterns), the durable function orchestrator will:
   1. Loop through all requested [CognitiveStep](src/CognitivePipeline/CognitivePipeline.Functions/Models/CognitiveStep.cs)s to a create a new thread to run that particular instruction.
   2. Each cognitive activity will finish execution in parallel and report back to the orchestrator with the results.
   3. The function orchestrator will wait for all steps to finish via ```Task.WaitAll(parallelTasks);```
6. Finally the orchestrator will send the final result to [CognitivePipeline_Callback](src/CognitivePipeline/CognitivePipeline.Functions/Orchestrator/CognitivePipelineOrchestrator.cs#L183-L194) to commit the updates to the Cosmos Db

### 2. Clients Real-Time-Communication

Leveraging the powerful [Cosmos Db Change Feed with Azure Function](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-concept-azure-functions) and [SignalR](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-overview) to add real-time-communication functionality to the registered clients.

Below are the steps highlighted in the architecture diagram:

1. As SignalR in [Serverless mode](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-concept-serverless-development-config), we need to avail a [Negotiate](src/CognitivePipeline/CognitivePipeline.RTC/RTC/SignalRNegotiator.cs) endpoint which will provide the client with SignalR access token. I used Http triggered Azure Function to accomplish that beautifully.
2. Client would then establish a connection to SignalR service (via calling the negotiate endpoint). A client application requires a valid access token to connect to Azure SignalR Service obtained from the negotiate endpoint.
3. Finally with a Change Feed lease connected to an Azure Function named [CognitiveFilesDbFeedProcessor](src/CognitivePipeline/CognitivePipeline.RTC/Functions/CognitiveFilesDbFeedProcessor.cs), it will listen to the updates and push it to the SignalR Hub. SignalR will then push the update to all connected clients to the Hub.

I've included in the repo a simple JS client that leverages SignalR JS SDK library that would look something like:

![JS Client](res/jsclient.png)

## Azure Components

You will need to provision the following services:

- 2 Azure Function Apps (one for processing pipeline and one for RTC callback)
- 1 Storage Account
- 1 Cosmos Db
- 1 API Management (OPTIONAL) - You can check out the [new serverless](https://azure.microsoft.com/en-us/blog/announcing-azure-api-management-for-serverless-architectures/) hosting plan 🎉
- 1 SignalR Service (in serverless mode)
- 1 Cognitive Service 
- 1 Face Cognitive Service (OPTIONAL)
- 1 Azure Search Index (OPTIONAL)
- 1 Azure AD (Or Azure AD B2C) tenant (OPTIONAL)
- 1 Azure KeyVault (OPTIONAL) - [GlobalSettings.cs](src/CognitivePipeline/CognitivePipeline.Functions/Utils/GlobalSettings.cs) is designed to allow fetching the settings from either the environment or Azure Key Vault.

You need the following local.settings.json to run locally or set them up in Environment Variables in the Function App configuration.

### CognitivePipeline.Functions

```js
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "Env": "Development",
    "cosmosDbEndpoint": "YOURDBENDPOINT",
    "cosmosDbKey": "YOUDBKEY",
    "cognitivePipelineStorageConnection": "STORAGECONNECTION",
    "computerVisionKey": "YOURKEY",
    "computerVisionEndpoint": "YOURENDPOINT",
    "faceKey": "YOURKEY",
    "faceEndpoint": "YOUENDPOINT"
  }
}
```

### CognitivePipeline.RTC

```js
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SignalRConnectionString": "YOURSIGNALRCONNECTION",
    "CogntiveFilesDbConnectionString": "COSMOSDBCONNECTIONSTRING"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "http://localhost:8080",
    "CORSCredentials": true
  }
}
```

## Automated CI/CD Pipelines

Using [Azure DevOps pipelines](https://github.com/marketplace/azure-pipelines) integration with GitHub, I have fully automated build and release pipelines.

![AzurePipelines](res/azurepipelines.png)

You can check the pipelines definition to build and unit test the solutions here: [azure-pipelines.yml](azure-pipelines.yml)

![AzurePipelines](res/azurepipelines-test.png)

Also this is a view of how I'm releasing to my Function App:

![AzurePipelines](res/azurepipelines-release.png)

## Testing The Solution 

Ohh so you downloaded the code immediately and want to start testing it out! Let's check what you need to know to make the magic happens.

### Azure Functions Local Debugging

Azure Function has a great local debugging experience in my opinion, just make sure you have the tools installed.

I must admit, I use the full Visual Studio in building this project, but it is perfectly fine to do that in the amazing VS Code as well.

I used a hack to allow me to launch multiple Azure Function debugging session. In Visual Studio you can set the solution to have multiple startup project and configure one of the projects to use a different port for debugging.

>**NOTE** You can read more about [multiple function debugging here](https://medium.com/microsoftazure/debugging-multiple-azure-functions-apps-at-the-same-time-c1040c8ab51c) written by the one and only Jim Bennett

![Multiple-Project-Start](res/function-multistart.png)

![Multiple-Project-Start](res/function-portchange.png)

Make sure you have your local settings and the related Azure services provisioned before hitting F5.

### Postman - Calling Functions Endpoints

You have your eyes staring at a fully healthy and running Azure Functions, it is time to hit these endpoints with well formed requests using Postman!

I've included in this repo a Postman collection export under [Postman-APIs](src/Postman-APIs/CognitivePipeline.postman_collection.json) folder. Just import it and let the testing begin.

![Postman](res/postman.png)

### JavaScript/Vue Client

I wanted a simple clean client that can connect to the SignalR service and show cool list of all the cognitive files processing in real-time.

Under **src/Clients/Client.JS** you will find few simple HTML, CSS and js files.

By using VS Code terminal or any command line tool, you can execute the following very well known npm commands:

```js
npm install
```

The above command will install all the referenced modules (it is like dotnet restore).

```js
npm start
```

Will launch the app in the browser at http://localhost:8080.

>**NOTE:** It is a good practice to open the development tools in the browser to have a look at the console logs.

Now you are ready, just start posting files using Postman and see how the client log a new entry when the file is submitted and replace it with updated results when the durable orchestrator finishes updating the database with all the cognitive goodness.

## Roadmap

Currently I'm hoping to finish working on:
1.  Completing both API Management and Azure Active Directory integration 👌
2.  Adjust SignalR usage to allow different users to listening to only the events published under their account 🤷‍
3.  Introduce new cognitive steps capabilities (especially the [Azure Video Indexer](https://docs.microsoft.com/en-us/azure/media-services/video-indexer/) service)
4.  Add Azure Search
3.  Generate rich Xamarin Mobile Client that can upload images and connect to SignalR for getting real-time-update about the processing status 🐱‍💻
4.  Any other cool ideas that might arise 😎

## About the project

I tried to make sure I cover all aspects and best practices while building this project, but all included architecture, code, documentation and any other artifact represent my personal opinion only. Think of it as a suggestion of how a one way things can work.

Keep in mind that this is a work-in-progress, I will continue to contribute to it when I can.

All constructive feedback is welcomed 🙏

## Support

You can always create issue, suggest an update through PR or direct message me on [Twitter](https://twitter.com/mohamedsaif101).

## Authors

|      ![Photo](res/mohamed-saif.jpg)            |
|:----------------------------------------------:|
|                 **Mohamed Saif**               |
|     [GitHub](https://github.com/mohamedsaif)   |
|  [Twitter](https://twitter.com/mohamedsaif101) |
|         [Blog](http://blog.mohamedsaif.com)    |