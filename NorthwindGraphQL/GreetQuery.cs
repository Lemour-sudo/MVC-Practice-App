using GraphQL.Types; // ObjectGraphType

namespace NorthwindGraphQL
{
    public class GreetQuery : ObjectGraphType
    {
        public GreetQuery()
        {
            Field<StringGraphType>(
                name: "greet",
                description: "A query type that greets the world.",
                resolve: context => "Hello, world!"
            );
        }
    }
}