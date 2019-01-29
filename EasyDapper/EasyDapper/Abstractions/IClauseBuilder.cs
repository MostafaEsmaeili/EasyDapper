namespace EasyDapper.Abstractions
{
  public interface IClauseBuilder
  {
    string Sql();

    bool IsClean { get; set; }
  }
}
