using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Atk.AtkExpression
{
  public static class AtkPartialEvaluator
  {
    public static Expression Eval(Expression expression)
    {
      return Eval(expression, null);
    }

    public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
    {
      if (fnCanBeEvaluated == null)
        fnCanBeEvaluated = CanBeEvaluatedLocally;
      return SubtreeEvaluator.Eval(Nominator.Nominate(fnCanBeEvaluated, expression), expression);
    }

    private static bool CanBeEvaluatedLocally(Expression expression)
    {
      return expression.NodeType != ExpressionType.Parameter;
    }

    private class SubtreeEvaluator : ExpressionVisitor
    {
      private HashSet<Expression> candidates;

      private SubtreeEvaluator(HashSet<Expression> candidates)
      {
        this.candidates = candidates;
      }

      internal static Expression Eval(HashSet<Expression> candidates, Expression exp)
      {
        return new SubtreeEvaluator(candidates).Visit(exp);
      }

      protected override Expression Visit(Expression exp)
      {
        if (exp == null)
          return null;
        if (candidates.Contains(exp))
          return Evaluate(exp);
        return base.Visit(exp);
      }

      private Expression Evaluate(Expression e)
      {
        Type type = e.Type;
        if (e.NodeType == ExpressionType.Convert && AtkTypeHelper.GetNonNullableType(((UnaryExpression) e).Operand.Type) == AtkTypeHelper.GetNonNullableType(type))
          e = ((UnaryExpression) e).Operand;
        if (e.NodeType == ExpressionType.Constant)
        {
          ConstantExpression constantExpression = (ConstantExpression) e;
          if (e.Type != type && AtkTypeHelper.GetNonNullableType(e.Type) == AtkTypeHelper.GetNonNullableType(type))
            e = Expression.Constant(constantExpression.Value, type);
          return e;
        }
        MemberExpression memberExpression = e as MemberExpression;
        if (memberExpression != null)
        {
          ConstantExpression expression = memberExpression.Expression as ConstantExpression;
          if (expression != null)
            return Expression.Constant(memberExpression.Member.GetValue(expression.Value), type);
        }
        if (type.IsValueType)
          e = Expression.Convert(e, typeof (object));
        return Expression.Constant(Expression.Lambda<Func<object>>(e, Array.Empty<ParameterExpression>()).Compile()(), type);
      }
    }

    private class Nominator : ExpressionVisitor
    {
      private Func<Expression, bool> fnCanBeEvaluated;
      private HashSet<Expression> candidates;
      private bool cannotBeEvaluated;

      private Nominator(Func<Expression, bool> fnCanBeEvaluated)
      {
        candidates = new HashSet<Expression>();
        this.fnCanBeEvaluated = fnCanBeEvaluated;
      }

      internal static HashSet<Expression> Nominate(Func<Expression, bool> fnCanBeEvaluated, Expression expression)
      {
        Nominator nominator = new Nominator(fnCanBeEvaluated);
        nominator.Visit(expression);
        return nominator.candidates;
      }

      protected override Expression VisitConstant(ConstantExpression c)
      {
        return base.VisitConstant(c);
      }

      protected override Expression Visit(Expression expression)
      {
        if (expression != null)
        {
          bool cannotBeEvaluated = this.cannotBeEvaluated;
          this.cannotBeEvaluated = false;
          base.Visit(expression);
          if (!this.cannotBeEvaluated)
          {
            if (fnCanBeEvaluated(expression))
              candidates.Add(expression);
            else
              this.cannotBeEvaluated = true;
          }
          this.cannotBeEvaluated |= cannotBeEvaluated;
        }
        return expression;
      }
    }
  }
}
