# FundPortfolioManager
A sample project that showcases high level financial concepts of portfolio generation for various investment funds.Pipeline shows ETL of raw financial data and conversion to pdf portfolios with accessiblity in mind. Deployment includes for both IaaS azure and aws cloud. 
# Architecture used:
  * Monolithic
  * Three layers
## Tools used:
    * net sdk for pdf reactor non-commercial license
    * pdf reactor docker
    * .Net Core 3.1
    * Asp .Net Core
    * DevExpress for Asp.Net Core non-commercial license
## Development
  ### EF core migrations using Package Manager Console
	* Install-Package Microsoft.EntityFrameworkCore.Tools
	* Update-Package Microsoft.EntityFrameworkCore.Tools
	* Get-Help about_EntityFrameworkCore
  ### Implementation of TPL Library:PLINQ
  ### Implementation of RabbitMQ as pub-sub pattern
  ### Implementation of Background Service and Thread Channel
  ### Implementation of Azure Service Bus
  ### Implementation of AWS S3,SQS,SNS in .NetCore. Dependencies:
      - AWSSDK.Extensions.NetCore.Setup
      - AWSSDK.s3
      - AWSSDK.queue
  ### Implementation of Localization using Resource files in .NET
    - Middleware configuration and management
    - Routing configuration for locale
  ### Implementation of opionated Mongodb
  
## Deployment in Prod
 ### azure kubernetes
 ### amazon ecs,ecs cluster,ecs task definition

