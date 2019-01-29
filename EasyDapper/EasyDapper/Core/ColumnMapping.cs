namespace EasyDapper.Core
{
  public class ColumnMapping
  {
    public ColumnMapping(int index, string name)
    {
      Index = index;
      Name = name;
    }

    public int Index { get; private set; }

    public string Name { get; private set; }
  }
}
