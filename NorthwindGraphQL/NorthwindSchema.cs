using System;
using Microsoft.Extensions.DependencyInjection;
using GraphQL.Types; // Schema
using Packt.Shared;

namespace NorthwindGraphQL
{
    public class NorthwindSchema : Schema
    {
        public NorthwindSchema(IServiceProvider provider) : base(provider)
        {
            // Query = new GreetQuery();
            Query = new NorthwindQuery(provider.GetRequiredService<Northwind>());
        }
    }
}