using GraphQL.Types; // ObjectGraphType<T>, ListGraphType<T>
using Packt.Shared; // Category

namespace NorthwindGraphQL
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType()
        {
            Name = "Category";
            Field(c => c.CategoryID).Description("ID of the category.");
            Field(c => c.CategoryName).Description("Name of the category.");
            Field(c => c.Description).Description("Description of the category.");
            Field(c => c.Products, type: typeof(ListGraphType<ProductType>)).Description("Products in the category.");
        }
    }
}