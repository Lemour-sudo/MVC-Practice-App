using Packt.Shared;

namespace NorthwindMVC.Models
{
    public class GraphQLProducts
    {
        public class DataProducts
        {
            public Product[]? Products { get; set; }
        }

        public DataProducts? Data { get; set; }
    }
}