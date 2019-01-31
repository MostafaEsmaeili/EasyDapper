using EasyDapper.Core.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace EesyDapper.Core.CustomAttribute
{
    public static class CustomAttributeHandle
    {
        public static bool IsIdField(this PropertyInfo propertyInfo)
        {
            var customAttribute = propertyInfo.GetCustomAttribute(typeof(DatabaseGeneratedAttribute));
            if (customAttribute != null)
                return ((DatabaseGeneratedAttribute) customAttribute).DatabaseGeneratedOption ==
                       DatabaseGeneratedOption.Identity;
            return propertyInfo.GetCustomAttribute(typeof(IdentityFieldAttribute)) != null;
        }

        public static string ColumnName(this PropertyInfo propertyInfo)
        {
            var customAttribute = propertyInfo.GetCustomAttribute(typeof(ColumnAttribute));
            if (customAttribute != null)
                return ((ColumnAttribute) customAttribute).Name;
            return propertyInfo.Name;
        }

        public static string ColumnName<TEntity>(this string propertyName)
        {
            var element = typeof(TEntity).GetProperties().Where(p => p.Name == propertyName).FirstOrDefault();
            if (element == null)
                return propertyName;
            var customAttribute = element.GetCustomAttribute(typeof(ColumnAttribute));
            if (customAttribute != null)
                return ((ColumnAttribute) customAttribute).Name;
            return element.Name;
        }

        public static string IdentityFieldStr<TEntity>(string oldId)
        {
            var propertyInfo = typeof(TEntity).GetProperties().Where(p => p.IsIdField()).FirstOrDefault();
            if (propertyInfo != null)
                return propertyInfo.Name;
            return oldId;
        }

        public static bool IsIdentityField<TEntity>(string oldId)
        {
            return typeof(TEntity).GetProperties().Where(p =>
            {
                if (p.IsIdField())
                    return p.Name == oldId;
                return false;
            }).FirstOrDefault() != null;
        }

        [Obsolete("拼写错误，请使用 IdentityFieldStr()")]
        public static string IdentityFiledStr<TEntity>(string oldId)
        {
            var propertyInfo = typeof(TEntity).GetProperties().Where(p => p.IsIdField()).FirstOrDefault();
            if (propertyInfo != null)
                return propertyInfo.Name;
            return oldId;
        }

        public static bool IsNonDBField(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute(typeof(NonDatabaseFieldAttribute)) != null ||
                   propertyInfo.GetCustomAttribute(typeof(NotMappedAttribute)) != null;
        }

        public static bool IsDBField(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute(typeof(SqlRepoDbFieldAttribute)) != null;
        }

        public static bool IsKeyField<TEntity>(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == "Id" && IdKeyIsExclude<TEntity>())
                return false;
            return propertyInfo.GetCustomAttribute(typeof(KeyFieldAttribute)) != null ||
                   propertyInfo.GetCustomAttribute(typeof(KeyAttribute)) != null;
        }

        public static bool IsKeyField<TEntity>(string oldId)
        {
            return typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()).FirstOrDefault() != null;
        }

        public static string FirstKeyFieldStr<TEntity>(string oldId)
        {
            var propertyInfo = typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()).FirstOrDefault();
            if (propertyInfo != null)
                return propertyInfo.Name;
            return oldId;
        }

        public static List<string> ListKeyFieldStr<TEntity>()
        {
            var stringList = new List<string>();
            foreach (var propertyInfo in typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()))
                stringList.Add(propertyInfo.Name);
            return stringList;
        }

        public static string DbTableName<TEntity>()
        {
            var customAttribute1 = typeof(TEntity).GetCustomAttribute(typeof(TableAttribute));
            if (customAttribute1 != null)
                return (customAttribute1 as TableAttribute).Name;
            var customAttribute2 = typeof(TEntity).GetCustomAttribute(typeof(TableNameAttribute));
            if (customAttribute2 != null)
                return (customAttribute2 as TableNameAttribute).TableName;
            return typeof(TEntity).Name;
        }

        public static bool IdKeyIsExclude<TEntity>()
        {
            if (typeof(TEntity).GetCustomAttribute(typeof(TableAttribute)) != null)
                return false;
            var customAttribute = typeof(TEntity).GetCustomAttribute(typeof(TableNameAttribute));
            if (customAttribute != null)
                return (customAttribute as TableNameAttribute).ExcludeIdKey;
            return false;
        }

        public static string DbTableSchema<TEntity>()
        {
            var customAttribute = typeof(TEntity).GetCustomAttribute(typeof(TableSchemaAttribute));
            if (customAttribute != null)
                return (customAttribute as TableSchemaAttribute).TableSchema;
            return "dbo";
        }

        public static string DbTableName<TEntity>(this TEntity entity)
        {
            var customAttribute1 = typeof(TEntity).GetCustomAttribute(typeof(TableAttribute));
            if (customAttribute1 != null)
                return (customAttribute1 as TableAttribute).Name;
            var customAttribute2 = typeof(TEntity).GetCustomAttribute(typeof(TableNameAttribute));
            if (customAttribute2 != null)
                return (customAttribute2 as TableNameAttribute).TableName;
            return typeof(TEntity).Name;
        }

        public static string DbTableSchemae<TEntity>(this TEntity entity)
        {
            var customAttribute1 = typeof(TEntity).GetCustomAttribute(typeof(TableAttribute));
            if (customAttribute1 != null)
                return (customAttribute1 as TableAttribute).Schema;
            var customAttribute2 = typeof(TEntity).GetCustomAttribute(typeof(TableSchemaAttribute));
            if (customAttribute2 != null)
                return (customAttribute2 as TableSchemaAttribute).TableSchema;
            return "dbo";
        }
    }
}