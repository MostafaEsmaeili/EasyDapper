namespace EasyDapper.Core
{
  public abstract class ColumnSelectionBase
  {
    public Aggregation Aggregation { get; set; }

    public string Alias { get; set; }

    public string Name { get; set; }

    public string Table { get; set; }
  }
}
