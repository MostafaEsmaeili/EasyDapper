namespace EasyDapper.Abstractions
{
    public interface IClauseBuilder
    {
        bool IsClean { get; set; }
        string Sql();
    }
}