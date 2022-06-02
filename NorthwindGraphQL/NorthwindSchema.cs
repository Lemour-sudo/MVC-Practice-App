using System;
using GraphQL.Types; // Schema

namespace NorthwindGraphQL
{
    public class NorthwindSchema : Schema
    {
        public NorthwindSchema(IServiceProvider provider) : base(provider)
        {
            Query = new GreetQuery();
        }
    }
}