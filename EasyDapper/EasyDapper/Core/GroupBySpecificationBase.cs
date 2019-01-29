namespace EasyDapper.Core
{
  public abstract class GroupBySpecificationBase
  {
    public string Alias { get; set; }

    public string Name { get; set; }

    public string Schema { get; set; }

    public string Table { get; set; }
  }
}
