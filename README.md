# Comparator

With the comparator, you can easily compare two objects with regard to their overall popularity and optional, 
user-defined aspects, such as performance, design, security etc.. 
To achieve that, the application uses an extensive elastic search data set to retrieve an array of sentences
that relate to the given query data and analyses them using a custom-built classifier as well as the 
natural-language processor by IBM watson to determine which object is perceived to be better.

## Contents

1. [Architecture](#architecture)
2. [Requirements](#requirements)
3. [Installation](#installation)
4. [Execution](#execution)
5. [API Documentation](#api-documentation)

## Architecture
The comparator consists of a [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1) backend as well as a mobile frontend written in [Dart](https://dart.dev) using the
[Flutter](https://flutter.dev) framework by Google. The repository of the flutter client can be found [here](https://github.com/P1NHE4D/ComparatorClient).

## Requirements
As already mentioned in the introduction, the comparator uses an elastic search data set and the natural language processing service
by IBM Watson. Hence, in order to use the backend properly, you need a IBM Watson account and a working api key for the 
natural language processing service. You can create an IBM watson account [here]().

Moreover, you also need an elastic search account that is entitled to access the elastic search cluster provided by the University of Hamburg.
Still, if you don't have access to the elastic search cluster, you can also create or use your own elastic search index as a data source.

## Installation
This section guides you through the process of installing the comparator backend on Max and/or Linux. If you are using a different operating
system, please refer to the [official documentation](https://docs.microsoft.com/en-us/dotnet/core/install/) on how to install the required tools mentioned below.

If you are looking for a guide on how to install the application on your mobile device, please refer to the [readme of the comparator client](https://github.com/P1NHE4D/ComparatorClient) 
repository.

### Install .NET Core and ASP.NET Core
The comparator backend is based on ASP.NET Core 3.1. After cloning the repository, execute the following commands
to install the required software:

Linux
```bash
# Install .NET Core 3.1 SDK
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-3.1
```

Mac
```bash
# Install .NET Core 3.1 SDK
brew install dotnet-sdk
```

### Config File
If you are using your own elastic search index, replace the url and defaultIndex accordingly.
```json
{
  "watson": {
    "apikey": "watson api key",
    "url": "watson url"
  },
  "ElasticSearch": {
    "user":"username",
    "password": "password",
    "url": "http://ltdemos.informatik.uni-hamburg.de",
    "defaultIndex": "depcc-index"
  }
}
```

### Execution
Execute the following command to start the backend:
```bash
dotnet run Comparator.csproj
```

The server should now run on [https://localhost:5001](https://localhost:5001).

### API Documentation
As soon as the server is up and running, you can find the API documentation by
navigating to [https://localhost:5001/docs/web-api](https://localhost:5001/docs/web-api). This will lead you to the
swagger UI where you can send demo queries to the API endpoint. It also provides
you with an extensive documentation on the types of the parameters used in the
query and the query result.