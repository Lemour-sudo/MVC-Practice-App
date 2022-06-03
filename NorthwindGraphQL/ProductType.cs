using GraphQL.Types; // ObjectGraphType<T>, IntGraphType, DecimalGraphType
using Packt.Shared; // Category, Product

namespace NorthwindGraphQL
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Name = "Product";
            Field(p => p.ProductID).Description("ID of the product.");
            Field(p => p.ProductName).Description("Name of the product.");
            Field(p => p.CategoryID, type: typeof(IntGraphType)).Description("CategoryID of the product.");
            Field(p => p.Category, type: typeof(CategoryType)).Description("Category of the product.");
            Field(p => p.UnitPrice, type: typeof(DecimalGraphType)).Description("Unit price of the product.");
            Field(p => p.UnitsInStock, type: typeof(IntGraphType)).Description("Units in stock of the product.");
            Field(p => p.UnitsOnOrder, type: typeof(IntGraphType)).Description("Units on order of the product.");
        }
    }
}