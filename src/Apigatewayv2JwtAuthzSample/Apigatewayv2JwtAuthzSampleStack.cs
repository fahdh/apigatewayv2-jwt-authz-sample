using Amazon.CDK;
using Amazon.CDK.AWS.APIGatewayv2;
using Amazon.CDK.AWS.Lambda;
using static Amazon.CDK.AWS.APIGatewayv2.CfnAuthorizer;

namespace Apigatewayv2JwtAuthzSample
{
    public class Apigatewayv2JwtAuthzSampleStack : Stack
    {
        internal Apigatewayv2JwtAuthzSampleStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            const string LambdaKey = "secure-lambda";
           // Create a lambda function that will execute the logic when the api is called.
            var function  = new Function(this, LambdaKey, new FunctionProps
            {
                Runtime = Runtime.NODEJS_12_X,
                Code = Code.FromAsset("lambdas"),
                Handler = "my-secure-lambda.handler"
            });

            // Add cors options. (if you intend to call this from a web app) 
            var cors = new CorsPreflightOptions
            {
                AllowCredentials = true,
                AllowHeaders = new string[] { "Authorization" },
                AllowMethods = new HttpMethod[] { HttpMethod.GET, HttpMethod.OPTIONS },
                AllowOrigins = new string[] { "http://localhost:4200" }
            };

            // create the http api.
            var api = new HttpApi(this, $"{LambdaKey}-API", new HttpApiProps
            {
                ApiName = "secure lambda api sample",
                CorsPreflight = cors
            });

            // add the JWT configuration for Issuer and Audience.
            var jwtConf = new JWTConfigurationProperty
            {
                Issuer = "https://yourouath2issuer",
                Audience = new string[] { "api://api1" }
            };

            // add the JWT Authorizer and attach the JWT configuration to it.
            var jwtAuthZ = new CfnAuthorizer(this, $"{LambdaKey}-jwt", new CfnAuthorizerProps
            {
                Name = "securelambda-jwt-authorizer",
                ApiId = api.HttpApiId,
                AuthorizerUri = api.Url,
                AuthorizerType = "JWT",
                IdentitySource = new string[] { "$request.header.Authorization" },
                JwtConfiguration = jwtConf
            });

            // create the integration between the http api and the lambda function.
            CfnIntegration integration = new CfnIntegration(this, $"{LambdaKey}-integration", new CfnIntegrationProps
            {
                ApiId = api.HttpApiId,
                IntegrationUri = function.FunctionArn,
                IntegrationType = "AWS_PROXY",
                //IntegrationMethod = "GET",
                PayloadFormatVersion = "2.0"
            });

            const string apiPath = @"/secureresource";

            // add a route to the api, attaching the JWT authorizer and targeting the integration.
            var cr = new CfnRoute(this, $"{LambdaKey}-route", new CfnRouteProps
            {
                ApiId = api.HttpApiId,
                RouteKey = $"GET {apiPath}",
                AuthorizationType = "JWT",
                AuthorizerId = jwtAuthZ.Ref,
                Target = $"integrations/{integration.Ref}"
            });

            // finally, add permissions so the http api can invoke the lambda for the api path.
            var resource = (CfnResource)api.Node.FindChild("Resource");
            
            function.AddPermission($"{LambdaKey}-permission", new Permission
            {
                Principal = new Amazon.CDK.AWS.IAM.ServicePrincipal("apigateway.amazonaws.com"),
                Action = "lambda:InvokeFunction",
                SourceArn = $"arn:aws:execute-api:{this.Region}:{this.Account}:{resource.Ref}/*/*{apiPath}"
            });
        }
    }
}
