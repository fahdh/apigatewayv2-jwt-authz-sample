using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apigatewayv2JwtAuthzSample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new Apigatewayv2JwtAuthzSampleStack(app, "Apigatewayv2JwtAuthzSampleStack");
            app.Synth();
        }
    }
}
