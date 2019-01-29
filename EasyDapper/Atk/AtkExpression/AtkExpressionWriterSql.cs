using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Atk.AtkExpression
{
  public class AtkExpressionWriterSql<T> : ExpressionVisitor
  {
    private static readonly char[] splitters = new char[2]
    {
      '\n',
      '\r'
    };
    private static readonly char[] special = new char[3]
    {
      '\n',
      '\n',
      '\\'
    };
    private string _tableAlias = "_table_Alias_";
    private string _leftbracket = "";
    private string _rightbracket = "";
    private int indent = 2;
    private int depth = 0;
    private string atkWhereResult = string.Empty;
    private string atkOrdeRsult = string.Empty;
    private int atkOrderTime;
    private int atkWhereTime;
    private AtkExpSqlType atkRead = AtkExpSqlType.atkWhere;
    private TextWriter writer;

    public int AtkOrderTime
    {
      get
      {
        return atkOrderTime;
      }
      set
      {
        atkOrderTime = value;
      }
    }

    public int AtkWhereTime
    {
      get
      {
        return atkWhereTime;
      }
      set
      {
        atkWhereTime = value;
      }
    }

    protected AtkExpressionWriterSql(TextWriter writer)
    {
      this.writer = writer;
    }

    private static void Write(TextWriter writer, Expression expression)
    {
      new AtkExpressionWriterSql<T>(writer).Visit(expression);
    }

    private static string Write(TextWriter writer, Expression expression, AtkExpSqlType atkSql, string leftBracket, string rightBracket)
    {
      expression = AtkPartialEvaluator.Eval(expression);
      AtkExpressionWriterSql<T> expressionWriterSql = new AtkExpressionWriterSql<T>(writer);
      expressionWriterSql._leftbracket = leftBracket;
      expressionWriterSql._rightbracket = rightBracket;
      expressionWriterSql.atkRead = atkSql;
      expressionWriterSql.Visit(expression);
      string empty = string.Empty;
      switch (atkSql)
      {
        case AtkExpSqlType.atkWhere:
          return Regex.Replace(expressionWriterSql.atkWhereResult, "and\\s?$", "");
        case AtkExpSqlType.atkOrder:
          return Regex.Replace(expressionWriterSql.atkOrdeRsult, ",\\s?$", "");
        default:
          return string.Empty;
      }
    }

    private static string WriteToString(Expression expression)
    {
      StringWriter stringWriter = new StringWriter();
      Write(stringWriter, expression);
      return stringWriter.ToString();
    }

    public static string AtkWhereWriteToString(Expression expression, AtkExpSqlType atkSql, string leftBracket = "", string rightBracket = "")
    {
      return Write(new StringWriter(), expression, atkSql, leftBracket, rightBracket).Replace("' + '", "").Replace("'+'", "");
    }

    protected int IndentationWidth
    {
      get
      {
        return indent;
      }
      set
      {
        indent = value;
      }
    }

    protected void WriteLine(Indentation style)
    {
      writer.WriteLine();
    }

    protected void Write(string text)
    {
      switch (atkRead)
      {
        case AtkExpSqlType.atkWhere:
          atkWhereResult += text;
          break;
        case AtkExpSqlType.atkOrder:
          atkOrdeRsult += text;
          break;
      }
      writer.Write(text);
    }

    protected void Indent(Indentation style)
    {
      if (style == Indentation.Inner || style != Indentation.Outer)
        return;
      Debug.Assert(depth >= 0);
    }

    protected virtual string GetOperator(ExpressionType type)
    {
      switch (type)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
          return "+";
        case ExpressionType.And:
          return "&";
        case ExpressionType.AndAlso:
          return "and";
        case ExpressionType.Coalesce:
          return "??";
        case ExpressionType.Divide:
          return "/";
        case ExpressionType.Equal:
          return "=";
        case ExpressionType.ExclusiveOr:
          return "^";
        case ExpressionType.GreaterThan:
          return ">";
        case ExpressionType.GreaterThanOrEqual:
          return ">=";
        case ExpressionType.LeftShift:
          return "<<";
        case ExpressionType.LessThan:
          return "<";
        case ExpressionType.LessThanOrEqual:
          return "<=";
        case ExpressionType.Modulo:
          return "%";
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
          return "*";
        case ExpressionType.Negate:
        case ExpressionType.NegateChecked:
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          return "-";
        case ExpressionType.Not:
          return " NOT ";
        case ExpressionType.NotEqual:
          return "!=";
        case ExpressionType.Or:
          return "Or";
        case ExpressionType.OrElse:
          return "Or";
        case ExpressionType.RightShift:
          return ">>";
        default:
          return null;
      }
    }

    protected override Expression VisitBinary(BinaryExpression b)
    {
      Write("(");
      if (b.NodeType == ExpressionType.Power)
      {
        Write("POWER(");
        VisitValue(b.Left);
        Write(", ");
        VisitValue(b.Right);
        Write(")");
      }
      else if (b.NodeType == ExpressionType.Coalesce)
      {
        Write("COALESCE(");
        VisitValue(b.Left);
        Write(", ");
        Expression right;
        BinaryExpression binaryExpression;
        for (right = b.Right; right.NodeType == ExpressionType.Coalesce; right = binaryExpression.Right)
        {
          binaryExpression = (BinaryExpression) right;
          VisitValue(binaryExpression.Left);
          Write(", ");
        }
        VisitValue(right);
        Write(")");
      }
      else if (b.NodeType == ExpressionType.LeftShift)
      {
        Write("(");
        VisitValue(b.Left);
        Write(" * POWER(2, ");
        VisitValue(b.Right);
        Write("))");
      }
      else if (b.NodeType == ExpressionType.RightShift)
      {
        Write("(");
        VisitValue(b.Left);
        Write(" / POWER(2, ");
        VisitValue(b.Right);
        Write("))");
      }
      else
      {
        IsRight = false;
        Visit(b.Left);
        Write(" ");
        Write(GetOperator(b.NodeType));
        Write(" ");
        IsRight = true;
        Visit(b.Right);
        IsRight = false;
      }
      Write(")");
      return b;
    }

    protected override Expression VisitUnary(UnaryExpression u)
    {
      switch (u.NodeType)
      {
        case ExpressionType.ArrayLength:
          Visit(u.Operand);
          Write(".Length");
          break;
        case ExpressionType.Convert:
        case ExpressionType.ConvertChecked:
          Visit(u.Operand);
          break;
        case ExpressionType.UnaryPlus:
          Visit(u.Operand);
          break;
        case ExpressionType.Quote:
          Visit(u.Operand);
          break;
        case ExpressionType.TypeAs:
          Visit(u.Operand);
          Write(" as ");
          Write(GetTypeName(u.Type));
          break;
        default:
          Write(GetOperator(u.NodeType));
          Visit(u.Operand);
          break;
      }
      return u;
    }

    protected virtual string GetTypeName(Type type)
    {
      string str = type.Name.Replace('+', '.');
      int length1 = str.IndexOf('`');
      if (length1 > 0)
        str = str.Substring(0, length1);
      if (type.IsGenericType || type.IsGenericTypeDefinition)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(str);
        stringBuilder.Append("<");
        Type[] genericArguments = type.GetGenericArguments();
        int index = 0;
        for (int length2 = genericArguments.Length; index < length2; ++index)
        {
          if (index > 0)
            stringBuilder.Append(",");
          if (type.IsGenericType)
            stringBuilder.Append(GetTypeName(genericArguments[index]));
        }
        stringBuilder.Append(">");
        str = stringBuilder.ToString();
      }
      return str;
    }

    protected override Expression VisitConditional(ConditionalExpression c)
    {
      Visit(c.Test);
      WriteLine(Indentation.Inner);
      Write("? ");
      Visit(c.IfTrue);
      WriteLine(Indentation.Same);
      Write(": ");
      Visit(c.IfFalse);
      Indent(Indentation.Outer);
      return c;
    }

    protected override IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
    {
      int index = 0;
      for (int count = original.Count; index < count; ++index)
      {
        VisitBinding(original[index]);
        if (index < count - 1)
        {
          Write(",");
          WriteLine(Indentation.Same);
        }
      }
      return original;
    }

    protected override Expression VisitConstant(ConstantExpression c)
    {
      if (c.Value == null)
        Write("null");
      else if (c.Type == typeof (DateTime))
      {
        Write("'");
        Write(((DateTime) c.Value).ToString("yyyy-MM-dd HH:mm:ss"));
        Write("'");
      }
      else if (c.Type == typeof (Guid))
      {
        Write("'");
        Write(c.Value.ToString());
        Write("'");
      }
      else
      {
        switch (Type.GetTypeCode(c.Value.GetType()))
        {
          case TypeCode.Boolean:
            Write((bool) c.Value ? "1" : "0");
            break;
          case TypeCode.Single:
          case TypeCode.Double:
            string str = c.Value.ToString();
            if (!str.Contains('.'))
              str += ".0";
            Write(str);
            break;
          case TypeCode.DateTime:
            Write("'");
            Write(((DateTime) c.Value).ToString("yyyy-MM-dd HH:mm:ss"));
            Write("'");
            break;
          case TypeCode.String:
            Write("'");
            Write(c.Value.ToString().Replace("'", "\""));
            Write("'");
            break;
          default:
            Write(c.Value.ToString());
            break;
        }
      }
      return c;
    }

    protected override ElementInit VisitElementInitializer(ElementInit initializer)
    {
      if (initializer.Arguments.Count > 1)
      {
        Write("{");
        int index = 0;
        for (int count = initializer.Arguments.Count; index < count; ++index)
        {
          Visit(initializer.Arguments[index]);
          if (index < count - 1)
            Write(", ");
        }
        Write("}");
      }
      else
        Visit(initializer.Arguments[0]);
      return initializer;
    }

    protected override IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
    {
      int index = 0;
      for (int count = original.Count; index < count; ++index)
      {
        VisitElementInitializer(original[index]);
        if (index < count - 1)
        {
          Write(",");
          WriteLine(Indentation.Same);
        }
      }
      return original;
    }

    protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
    {
      int index = 0;
      for (int count = original.Count; index < count; ++index)
        Visit(original[index]);
      return original;
    }

    protected override Expression VisitInvocation(InvocationExpression iv)
    {
      Write("Invoke(");
      WriteLine(Indentation.Inner);
      VisitExpressionList(iv.Arguments);
      Write(", ");
      WriteLine(Indentation.Same);
      Visit(iv.Expression);
      WriteLine(Indentation.Same);
      Write(")");
      Indent(Indentation.Outer);
      return iv;
    }

    protected override Expression VisitLambda(LambdaExpression lambda)
    {
      if (lambda.Body.NodeType == ExpressionType.MemberAccess)
      {
        if (lambda.Body.Type == typeof (bool) && atkRead == AtkExpSqlType.atkWhere)
        {
          Write(((MemberExpression) lambda.Body).Member.Name + " = 1");
        }
        else
        {
          Visit(lambda.Body);
          return lambda;
        }
      }
      Visit(lambda.Body);
      return lambda;
    }

    protected override Expression VisitListInit(ListInitExpression init)
    {
      Visit(init.NewExpression);
      Write(" {");
      WriteLine(Indentation.Inner);
      VisitElementInitializerList(init.Initializers);
      WriteLine(Indentation.Outer);
      Write("}");
      return init;
    }

    protected override Expression VisitMemberAccess(MemberExpression m)
    {
      if (m.Member.DeclaringType == typeof (string))
      {
        Write(_tableAlias + _leftbracket + AtkTypeHelper.GetColumnAlias<T>(m.Member.Name) + _rightbracket);
        if (m.Member.Name == "Length")
        {
          Write("LEN(");
          Visit(m.Expression);
          Write(")");
          return m;
        }
      }
      else if (m.Member.DeclaringType.IsGenericType && m.Member.DeclaringType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
      {
        if (m.Member.Name == "HasValue")
        {
          Write("(");
          Visit(m.Expression);
          Write(" IS NOT NULL)");
          return m;
        }
      }
      else if (m.Member.DeclaringType == typeof (DateTime) || m.Member.DeclaringType == typeof (DateTimeOffset))
      {
        Write(_tableAlias + _leftbracket + AtkTypeHelper.GetColumnAlias<T>(m.Member.Name) + _rightbracket);
        switch (m.Member.Name)
        {
          case "Day":
            Write("DAY(");
            Visit(m.Expression);
            Write(")");
            return m;
          case "DayOfWeek":
            Write("(DATEPART(weekday, ");
            Visit(m.Expression);
            Write(") - 1)");
            return m;
          case "DayOfYear":
            Write("(DATEPART(dayofyear, ");
            Visit(m.Expression);
            Write(") - 1)");
            return m;
          case "Hour":
            Write("DATEPART(hour, ");
            Visit(m.Expression);
            Write(")");
            return m;
          case "Millisecond":
            Write("DATEPART(millisecond, ");
            Visit(m.Expression);
            Write(")");
            return m;
          case "Minute":
            Write("DATEPART(minute, ");
            Visit(m.Expression);
            Write(")");
            return m;
          case "Month":
            Write("MONTH(");
            Visit(m.Expression);
            Write(")");
            return m;
          case "Second":
            Write("DATEPART(second, ");
            Visit(m.Expression);
            Write(")");
            return m;
          case "Year":
            Write("YEAR(");
            Visit(m.Expression);
            Write(")");
            return m;
        }
      }
      else if (IsRight)
        Write("@" + m.Member.Name);
      else
        Write(_tableAlias + _leftbracket + AtkTypeHelper.GetColumnAlias<T>(m.Member.Name) + _rightbracket);
      return base.VisitMemberAccess(m);
    }

    protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
    {
      Write(_tableAlias + _leftbracket + AtkTypeHelper.GetColumnAlias<T>(assignment.Member.Name) + _rightbracket);
      Write(" = ");
      Visit(assignment.Expression);
      return assignment;
    }

    protected override Expression VisitMemberInit(MemberInitExpression init)
    {
      Visit(init.NewExpression);
      Write(" {");
      WriteLine(Indentation.Inner);
      VisitBindingList(init.Bindings);
      WriteLine(Indentation.Outer);
      Write("}");
      return init;
    }

    protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
    {
      Write(_tableAlias + _leftbracket + AtkTypeHelper.GetColumnAlias<T>(binding.Member.Name) + _rightbracket);
      Write(" = {");
      WriteLine(Indentation.Inner);
      VisitElementInitializerList(binding.Initializers);
      WriteLine(Indentation.Outer);
      Write("}");
      return binding;
    }

    protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
    {
      Write(_tableAlias + _leftbracket + AtkTypeHelper.GetColumnAlias<T>(binding.Member.Name) + _rightbracket);
      Write(" = {");
      WriteLine(Indentation.Inner);
      VisitBindingList(binding.Bindings);
      WriteLine(Indentation.Outer);
      Write("}");
      return binding;
    }

    protected override Expression VisitMethodCall(MethodCallExpression m)
    {
      string lower = m.Method.Name.ToLower();
      string str1 = lower;
      if (!(str1 == "where"))
      {
        if (str1 == "orderby" || str1 == "orderbydescending" || (str1 == "thenbydescending" || str1 == "thenby"))
        {
          if (atkRead == AtkExpSqlType.atkOrder)
          {
            ++AtkOrderTime;
          }
          else
          {
            Visit(m.Arguments[0]);
            return m;
          }
        }
      }
      else if (atkRead == AtkExpSqlType.atkWhere)
      {
        ++AtkWhereTime;
      }
      else
      {
        Visit(m.Arguments[0]);
        return m;
      }
      if (m.Arguments.Count <= 1)
        ;
      if (m.Method.DeclaringType == typeof (string))
      {
        switch (m.Method.Name)
        {
          case "Concat":
            IList<Expression> expressionList = m.Arguments;
            if (expressionList.Count == 1 && expressionList[0].NodeType == ExpressionType.NewArrayInit)
              expressionList = ((NewArrayExpression) expressionList[0]).Expressions;
            int index = 0;
            for (int count = expressionList.Count; index < count; ++index)
            {
              if (index > 0)
                Write(" + ");
              Visit(expressionList[index]);
            }
            return m;
          case "Contains":
            Write("(");
            Visit(m.Object);
            Write(" LIKE '%' + ");
            Visit(m.Arguments[0]);
            Write(" + '%')");
            return m;
          case "EndsWith":
            Write("(");
            Visit(m.Object);
            Write(" LIKE '%' + ");
            Visit(m.Arguments[0]);
            Write(")");
            return m;
          case "IndexOf":
            Write("(CHARINDEX(");
            Visit(m.Arguments[0]);
            Write(", ");
            Visit(m.Object);
            if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof (int))
            {
              Write(", ");
              Visit(m.Arguments[1]);
              Write(" + 1");
            }
            Write(") - 1)");
            return m;
          case "IsNullOrEmpty":
            Write("(");
            Visit(m.Arguments[0]);
            Write("  IS NULL OR ");
            Visit(m.Arguments[0]);
            Write(" = '')");
            return m;
          case "IsNullOrWhiteSpace":
            Write("(");
            Visit(m.Arguments[0]);
            Write("  IS NULL OR ");
            Write("RTRIM(");
            Visit(m.Arguments[0]);
            Write(") = '')");
            return m;
          case "Remove":
            Write("STUFF(");
            Visit(m.Object);
            Write(", ");
            Visit(m.Arguments[0]);
            Write(" + 1, ");
            if (m.Arguments.Count == 2)
              Visit(m.Arguments[1]);
            else
              Write("8000");
            Write(", '')");
            return m;
          case "Replace":
            Write("REPLACE(");
            Visit(m.Object);
            Write(", ");
            Visit(m.Arguments[0]);
            Write(", ");
            Visit(m.Arguments[1]);
            Write(")");
            return m;
          case "StartsWith":
            Write("(");
            Visit(m.Object);
            Write(" LIKE ");
            Visit(m.Arguments[0]);
            Write(" + '%'))");
            return m;
          case "Substring":
            Write("SUBSTRING(");
            Visit(m.Object);
            Write(", ");
            Visit(m.Arguments[0]);
            Write(" + 1, ");
            if (m.Arguments.Count == 2)
              Visit(m.Arguments[1]);
            else
              Write("8000");
            Write(")");
            return m;
          case "ToLower":
            Write("LOWER(");
            Visit(m.Object);
            Write(")");
            return m;
          case "ToUpper":
            Write("UPPER(");
            Visit(m.Object);
            Write(")");
            return m;
          case "Trim":
            Write("RTRIM(LTRIM(");
            Visit(m.Object);
            Write("))");
            return m;
        }
      }
      else if (m.Method.DeclaringType == typeof (DateTime))
      {
        switch (m.Method.Name)
        {
          case "AddDays":
            Write("DATEADD(DAY,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "AddHours":
            Write("DATEADD(HH,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "AddMilliseconds":
            Write("DATEADD(MS,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "AddMinutes":
            Write("DATEADD(MI,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "AddMonths":
            Write("DATEADD(MM,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "AddSeconds":
            Write("DATEADD(SS,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "AddYears":
            Write("DATEADD(YYYY,");
            Visit(m.Arguments[0]);
            Write(",");
            Visit(m.Object);
            Write(")");
            return m;
          case "op_Subtract":
            if (m.Arguments[1].Type == typeof (DateTime))
            {
              Write("DATEDIFF(");
              Visit(m.Arguments[0]);
              Write(", ");
              Visit(m.Arguments[1]);
              Write(")");
              return m;
            }
            break;
        }
      }
      else if (m.Method.DeclaringType == typeof (Decimal))
      {
        switch (m.Method.Name)
        {
          case "Add":
          case "Divide":
          case "Multiply":
          case "Remainder":
          case "Subtract":
            Write("(");
            VisitValue(m.Arguments[0]);
            Write(" ");
            Write(GetOperator(m.Method.Name));
            Write(" ");
            VisitValue(m.Arguments[1]);
            Write(")");
            return m;
          case "Ceiling":
          case "Floor":
            Write(m.Method.Name.ToUpper());
            Write("(");
            Visit(m.Arguments[0]);
            Write(")");
            return m;
          case "Negate":
            Write("-");
            Visit(m.Arguments[0]);
            Write("");
            return m;
          case "Round":
            if (m.Arguments.Count == 1)
            {
              Write("ROUND(");
              Visit(m.Arguments[0]);
              Write(", 0)");
              return m;
            }
            if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof (int))
            {
              Write("ROUND(");
              Visit(m.Arguments[0]);
              Write(", ");
              Visit(m.Arguments[1]);
              Write(")");
              return m;
            }
            break;
          case "Truncate":
            Write("ROUND(");
            Visit(m.Arguments[0]);
            Write(", 0, 1)");
            return m;
        }
      }
      else if (m.Method.DeclaringType == typeof (Math))
      {
        switch (m.Method.Name)
        {
          case "Abs":
          case "Acos":
          case "Asin":
          case "Atan":
          case "Ceiling":
          case "Cos":
          case "Exp":
          case "Floor":
          case "Log10":
          case "Sign":
          case "Sin":
          case "Sqrt":
          case "Tan":
            Write(m.Method.Name.ToUpper());
            Write("(");
            Visit(m.Arguments[0]);
            Write(")");
            return m;
          case "Atan2":
            Write("ATN2(");
            Visit(m.Arguments[0]);
            Write(", ");
            Visit(m.Arguments[1]);
            Write(")");
            return m;
          case "Log":
            if (m.Arguments.Count != 1)
              break;
            goto case "Abs";
          case "Pow":
            Write("POWER(");
            Visit(m.Arguments[0]);
            Write(", ");
            Visit(m.Arguments[1]);
            Write(")");
            return m;
          case "Round":
            if (m.Arguments.Count == 1)
            {
              Write("ROUND(");
              Visit(m.Arguments[0]);
              Write(", 0)");
              return m;
            }
            if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof (int))
            {
              Write("ROUND(");
              Visit(m.Arguments[0]);
              Write(", ");
              Visit(m.Arguments[1]);
              Write(")");
              return m;
            }
            break;
          case "Truncate":
            Write("ROUND(");
            Visit(m.Arguments[0]);
            Write(", 0, 1)");
            return m;
        }
      }
      if (m.Method.Name == "ToString")
      {
        if (m.Object.Type != typeof (string))
        {
          Write("CONVERT(NVARCHAR, ");
          Visit(m.Object);
          Write(")");
        }
        else
          Visit(m.Object);
        return m;
      }
      if (!m.Method.IsStatic && m.Method.Name == "CompareTo" && m.Method.ReturnType == typeof (int) && m.Arguments.Count == 1)
      {
        Write("(CASE WHEN ");
        Visit(m.Object);
        Write(" = ");
        Visit(m.Arguments[0]);
        Write(" THEN 0 WHEN ");
        Visit(m.Object);
        Write(" < ");
        Visit(m.Arguments[0]);
        Write(" THEN -1 ELSE 1 END)");
        return m;
      }
      if (m.Method.IsStatic && m.Method.Name == "Compare" && m.Method.ReturnType == typeof (int) && m.Arguments.Count == 2)
      {
        Write("(CASE WHEN ");
        Visit(m.Arguments[0]);
        Write(" = ");
        Visit(m.Arguments[1]);
        Write(" THEN 0 WHEN ");
        Visit(m.Arguments[0]);
        Write(" < ");
        Visit(m.Arguments[1]);
        Write(" THEN -1 ELSE 1 END)");
        return m;
      }
      if (m.Arguments.Count > 1)
        WriteLine(Indentation.Outer);
      base.VisitMethodCall(m);
      if (m.Arguments.Count > 1)
      {
        string str2 = lower;
        if (str2 == "orderbydescending" || str2 == "thenbydescending")
          atkOrdeRsult += " Desc";
        if (atkRead == AtkExpSqlType.atkOrder)
          atkOrdeRsult += ",";
        else
          atkWhereResult += " and ";
        WriteLine(Indentation.Outer);
      }
      return m;
    }

    protected Expression VisitValue(Expression expr)
    {
      if (!IsPredicate(expr))
        return expr;
      Write("CASE WHEN (");
      Visit(expr);
      Write(") THEN 1 ELSE 0 END");
      return expr;
    }

    protected override NewExpression VisitNew(NewExpression nex)
    {
      if (nex.Constructor.DeclaringType == typeof (DateTime))
      {
        if (nex.Arguments.Count == 3)
        {
          Write("Convert(DateTime, ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[0]);
          Write(") + '/' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[1]);
          Write(") + '/' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[2]);
          Write("))");
          return nex;
        }
        if (nex.Arguments.Count == 6)
        {
          Write("Convert(DateTime, ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[0]);
          Write(") + '/' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[1]);
          Write(") + '/' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[2]);
          Write(") + ' ' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[3]);
          Write(") + ':' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[4]);
          Write(") + ':' + ");
          Write("Convert(nvarchar, ");
          Visit(nex.Arguments[5]);
          Write("))");
          return nex;
        }
      }
      return base.VisitNew(nex);
    }

    protected override Expression VisitNewArray(NewArrayExpression na)
    {
      Write("new ");
      Write(GetTypeName(AtkTypeHelper.GetElementType(na.Type)));
      Write("[] {");
      if (na.Expressions.Count > 1)
        WriteLine(Indentation.Inner);
      VisitExpressionList(na.Expressions);
      if (na.Expressions.Count > 1)
        WriteLine(Indentation.Outer);
      Write("}");
      return na;
    }

    protected override Expression VisitParameter(ParameterExpression p)
    {
      return p;
    }

    protected override Expression VisitTypeIs(TypeBinaryExpression b)
    {
      Visit(b.Expression);
      Write(" is ");
      Write(GetTypeName(b.TypeOperand));
      return b;
    }

    protected override Expression VisitUnknown(Expression expression)
    {
      Write(expression.ToString());
      return expression;
    }

    protected virtual bool IsBoolean(Type type)
    {
      return type == typeof (bool) || type == typeof (bool?);
    }

    protected virtual bool IsPredicate(Expression expr)
    {
      switch (expr.NodeType)
      {
        case ExpressionType.And:
        case ExpressionType.AndAlso:
        case ExpressionType.Or:
        case ExpressionType.OrElse:
          return IsBoolean(expr.Type);
        case ExpressionType.Call:
          return IsBoolean(expr.Type);
        case ExpressionType.Equal:
        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
        case ExpressionType.NotEqual:
        case (ExpressionType) 1009:
        case (ExpressionType) 1010:
        case (ExpressionType) 1013:
        case (ExpressionType) 1014:
          return true;
        case ExpressionType.Not:
          return IsBoolean(expr.Type);
        default:
          return false;
      }
    }

    protected virtual string GetOperator(string methodName)
    {
      string str = methodName;
      if (str == "Add")
        return "+";
      if (str == "Subtract")
        return "-";
      if (str == "Multiply")
        return "*";
      if (str == "Divide")
        return "/";
      if (str == "Negate")
        return "-";
      if (str == "Remainder")
        return "%";
      return null;
    }

    protected virtual string GetOperator(UnaryExpression u)
    {
      switch (u.NodeType)
      {
        case ExpressionType.Negate:
        case ExpressionType.NegateChecked:
          return "-";
        case ExpressionType.UnaryPlus:
          return "+";
        case ExpressionType.Not:
          return IsBoolean(u.Operand.Type) ? "NOT" : "~";
        default:
          return "";
      }
    }

    protected virtual string GetOperator(BinaryExpression b)
    {
      switch (b.NodeType)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
          return "+";
        case ExpressionType.And:
        case ExpressionType.AndAlso:
          return IsBoolean(b.Left.Type) ? "AND" : "&";
        case ExpressionType.Divide:
          return "/";
        case ExpressionType.Equal:
          return "=";
        case ExpressionType.ExclusiveOr:
          return "^";
        case ExpressionType.GreaterThan:
          return ">";
        case ExpressionType.GreaterThanOrEqual:
          return ">=";
        case ExpressionType.LeftShift:
          return "<<";
        case ExpressionType.LessThan:
          return "<";
        case ExpressionType.LessThanOrEqual:
          return "<=";
        case ExpressionType.Modulo:
          return "%";
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
          return "*";
        case ExpressionType.NotEqual:
          return "<>";
        case ExpressionType.Or:
        case ExpressionType.OrElse:
          return IsBoolean(b.Left.Type) ? "OR" : "|";
        case ExpressionType.RightShift:
          return ">>";
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          return "-";
        default:
          return "";
      }
    }

    protected enum Indentation
    {
      Same,
      Inner,
      Outer
    }

    internal enum DbExpressionType
    {
      Table = 1000, // 0x000003E8
      ClientJoin = 1001, // 0x000003E9
      Column = 1002, // 0x000003EA
      Select = 1003, // 0x000003EB
      Projection = 1004, // 0x000003EC
      Entity = 1005, // 0x000003ED
      Join = 1006, // 0x000003EE
      Aggregate = 1007, // 0x000003EF
      Scalar = 1008, // 0x000003F0
      Exists = 1009, // 0x000003F1
      In = 1010, // 0x000003F2
      Grouping = 1011, // 0x000003F3
      AggregateSubquery = 1012, // 0x000003F4
      IsNull = 1013, // 0x000003F5
      Between = 1014, // 0x000003F6
      RowCount = 1015, // 0x000003F7
      NamedValue = 1016, // 0x000003F8
      OuterJoined = 1017, // 0x000003F9
      Insert = 1018, // 0x000003FA
      Update = 1019, // 0x000003FB
      Delete = 1020, // 0x000003FC
      Batch = 1021, // 0x000003FD
      Function = 1022, // 0x000003FE
      Block = 1023, // 0x000003FF
      If = 1024, // 0x00000400
      Declaration = 1025, // 0x00000401
      Variable = 1026 // 0x00000402
    }
  }
}
