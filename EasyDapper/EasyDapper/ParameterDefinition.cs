using System.Data;

namespace EasyDapper
{
    public class ParameterDefinition
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public ParameterDirection Direction { get; set; }

        public int Size { get; set; }

        public bool IsNullable { get; set; }

        public DbType DbType { get; set; }
    }
}