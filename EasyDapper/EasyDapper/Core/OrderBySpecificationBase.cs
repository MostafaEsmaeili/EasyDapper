namespace EasyDapper.Core
{
    public abstract class OrderBySpecificationBase
    {
        public string Alias { get; set; }

        public OrderByDirection Direction { get; set; }

        public string Name { get; set; }

        public string Schema { get; set; }

        public string Table { get; set; }
    }
}