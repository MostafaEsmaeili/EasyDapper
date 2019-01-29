using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace EasyDapper.Core.CustomAttribute
{
  public static class CustomAttributeHandle
  {
    public static bool IsIdField(this PropertyInfo propertyInfo)
    {
      Attribute customAttribute = propertyInfo.GetCustomAttribute(typeof (DatabaseGeneratedAttribute));
      if (customAttribute != null)
        return ((DatabaseGeneratedAttribute) customAttribute).DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
      return propertyInfo.GetCustomAttribute(typeof (IdentityFieldAttribute)) != null;
    }

    public static string ColumnName(this PropertyInfo propertyInfo)
    {
      Attribute customAttribute = propertyInfo.GetCustomAttribute(typeof (ColumnAttribute));
      if (customAttribute != null)
        return ((ColumnAttribute) customAttribute).Name;
      return propertyInfo.Name;
    }

    public static bool IsIdField(this PropertyInfo propertyInfo, string idname)
    {
      return propertyInfo.GetCustomAttribute(typeof (IdentityFieldAttribute)) != null || propertyInfo.Name == idname;
    }

    public static string IdentityFieldStr<TEntity>(string oldId)
    {
      PropertyInfo propertyInfo = typeof (TEntity).GetProperties().Where(p => p.IsIdField()).FirstOrDefault();
      if (propertyInfo != null)
        return propertyInfo.Name;
      return oldId;
    }

    public static bool IsIdentityField<TEntity>(string oldId)
    {
      return typeof (TEntity).GetProperties().Where(p =>
      {
          if (p.IsIdField())
              return p.Name == oldId;
          return false;
      }).FirstOrDefault() != null;
    }

    [Obsolete("Obsolete, Please use IdentityFieldStr()")]
    public static string IdentityFiledStr<TEntity>(string oldId)
    {
      PropertyInfo propertyInfo = typeof (TEntity).GetProperties().Where(p => p.IsIdField()).FirstOrDefault();
      if (propertyInfo != null)
        return propertyInfo.Name;
      return oldId;
    }

    public static bool IsNonDBField(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetCustomAttribute(typeof (NonDatabaseFieldAttribute)) != null || propertyInfo.GetCustomAttribute(typeof (NotMappedAttribute)) != null;
    }

    public static bool IsDBField(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetCustomAttribute(typeof (SqlRepoDbFieldAttribute)) != null;
    }

    public static bool IsKeyField(this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetCustomAttribute(typeof (KeyFieldAttribute)) != null || propertyInfo.GetCustomAttribute(typeof (KeyAttribute)) != null;
    }

    public static bool IsKeyField<TEntity>(string oldId)
    {
      return typeof (TEntity).GetProperties().Where(p => p.IsKeyField()).FirstOrDefault() != null;
    }

    public static string FirstKeyFieldStr<TEntity>(string oldId)
    {
      PropertyInfo propertyInfo = typeof (TEntity).GetProperties().Where(p => p.IsKeyField()).FirstOrDefault();
      if (propertyInfo != null)
        return propertyInfo.Name;
      return oldId;
    }

    [Obsolete("Obsolete, Please use FirstKeyFieldStr()")]
    public static string FirstKeyFiledStr<TEntity>(string oldId)
    {
      PropertyInfo propertyInfo = typeof (TEntity).GetProperties().Where(p => p.IsKeyField()).FirstOrDefault();
      if (propertyInfo != null)
        return propertyInfo.Name;
      return oldId;
    }

    public static List<string> ListKeyFieldStr<TEntity>()
    {
      List<string> stringList = new List<string>();
      foreach (PropertyInfo propertyInfo in typeof (TEntity).GetProperties().Where(p => p.IsKeyField()))
        stringList.Add(propertyInfo.Name);
      return stringList;
    }

    public static string DbTableName<TEntity>()
    {
      Attribute customAttribute1 = typeof (TEntity).GetCustomAttribute(typeof (TableAttribute));
      if (customAttribute1 != null)
        return (customAttribute1 as TableAttribute).Name;
      Attribute customAttribute2 = typeof (TEntity).GetCustomAttribute(typeof (TableNameAttribute));
      if (customAttribute2 != null)
        return (customAttribute2 as TableNameAttribute).TableName;
      return typeof (TEntity).Name;
    }

    public static string DbTableSchema<TEntity>()
    {
      Attribute customAttribute = typeof (TEntity).GetCustomAttribute(typeof (TableSchemaAttribute));
      if (customAttribute != null)
        return (customAttribute as TableSchemaAttribute).TableSchema;
      return "dbo";
    }

    public static string DbTableName<TEntity>(this TEntity entity)
    {
      Attribute customAttribute1 = typeof (TEntity).GetCustomAttribute(typeof (TableAttribute));
      if (customAttribute1 != null)
        return (customAttribute1 as TableAttribute).Name;
      Attribute customAttribute2 = typeof (TEntity).GetCustomAttribute(typeof (TableNameAttribute));
      if (customAttribute2 != null)
        return (customAttribute2 as TableNameAttribute).TableName;
      return typeof (TEntity).Name;
    }

    public static string DbTableSchemae<TEntity>(this TEntity entity)
    {
      Attribute customAttribute1 = typeof (TEntity).GetCustomAttribute(typeof (TableAttribute));
      if (customAttribute1 != null)
        return (customAttribute1 as TableAttribute).Schema;
      Attribute customAttribute2 = typeof (TEntity).GetCustomAttribute(typeof (TableSchemaAttribute));
      if (customAttribute2 != null)
        return (customAttribute2 as TableSchemaAttribute).TableSchema;
      return "dbo";
    }
  }
}
