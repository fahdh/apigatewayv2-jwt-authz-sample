# AWS CDK Project for Creating an AWS ApigatewayV2 with a JWT authorizer for a lambda function.

AWS docs https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-jwt-authorizer.html


Article explaining JWT Auhtorizer and its setup via the AWS Console.
https://auth0.com/blog/securing-aws-http-apis-with-jwt-authorizers/

## Project structure:
The cdk app is under /src
the lambda function that will be deployed is under /lambdas

## Steps:
Add the AWS.CDK.APIGatewayv2 package
```BASH
dotnet add package Amazon.CDK.AWS.APIGatewayv2
```

Add the AWS.CDK.Lambda package
```BASH
dotnet add package Amazon.CDK.AWS.Lambda
```

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template