using GraphQL; // GetArgument extension method
using GraphQL.Types; // ObjectGraphType, QueryArguments, QueryArgument<T>
using Microsoft.EntityFrameworkCore; // Include extension method
using Packt.Shared;

namespace NorthwindGraphQL
{
    public class NorthwindQuery : ObjectGraphType
    {
        public NorthwindQuery(Northwind db)
        {
            Field<ListGraphType<CategoryType>>(
                name: "categories",
                description: "A query type that returns a list of all categories.",
                resolve: context => db.Categories.Include(c => c.Products)
            );

            Field<CategoryType>(
                name: "category",
                description: "A query type that returns a category using its Id.",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "categoryID" }
                ),
                resolve: context => {
                    var category = db.Categories.Find(context.GetArgument<int>("categoryID"));
                    db.Entry(category).Collection(c => c.Products).Load();
                    return category;
                }
            );

            Field<ListGraphType<ProductType>>(
                name: "products",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "categoryID" }
                ),
                resolve: context => {
                    var category = db.Categories.Find(context.GetArgument<int>("categoryID"));
                    db.Entry(category).Collection(c => c.Products).Load();
                    return category.Products;
                }
            );
        }
    }
}