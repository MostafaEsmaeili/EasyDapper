using System.Collections.Generic;
using EasyDapper.Enum;

namespace EasyDapper
{
    /// <summary>
    /// This component is used to parameterize the values in the  Where Statement
    /// </summary>
    public class PartialSelectSelectParameterDefinition : ParameterDefinition
    {
        public ExpressionSide ExpressionSide { get; set; }
        public string Params { get; set; }
    }

    public struct CustomSqlWriteExpressionResult
    {

        public CustomSqlWriteExpressionResult(string sql, string @operator, PartialSelectSelectParameterDefinition left = null,
            PartialSelectSelectParameterDefinition right = null)
        {
            Operator = @operator;
            Sql = sql.Replace("' + '", "")
                .Replace("'+'", "");
            Parameters = new List<PartialSelectSelectParameterDefinition>();
            if (left!=null)
            {
                left.ExpressionSide = ExpressionSide.Left;
                Parameters.Add(left);
            }

            if (right == null) return;
            right.ExpressionSide = ExpressionSide.Right;
            Parameters.Add(right);
        }

        public string Sql { get; set; }
        public string Operator { get;}
        public List<PartialSelectSelectParameterDefinition> Parameters{ get; }

    }
}