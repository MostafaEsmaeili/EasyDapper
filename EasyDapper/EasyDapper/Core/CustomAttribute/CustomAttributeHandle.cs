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
            var customAttribute = propertyInfo.GetCustomAttribute(typeof(DatabaseGeneratedAttribute));
            if (customAttribute != null)
                return ((DatabaseGeneratedAttribute) customAttribute).DatabaseGeneratedOption ==
                       DatabaseGeneratedOption.Identity;
            return propertyInfo.GetCustomAttribute(typeof(IdentityFieldAttribute)) != null;
        }

        public static string ColumnName(this PropertyInfo propertyInfo)
        {
            var customAttribute = propertyInfo.GetCustomAttribute(typeof(ColumnAttribute));
            return customAttribute != null ? ((ColumnAttribute) customAttribute).Name : propertyInfo.Name;
        }

        public static string ColumnName<TEntity>(this string propertyName)
        {
            var element = typeof(TEntity).GetProperties().FirstOrDefault(p => p.Name == propertyName);
            if (element == null)
                return propertyName;
            var customAttribute = element.GetCustomAttribute(typeof(ColumnAttribute));
            if (customAttribute != null)
                return ((ColumnAttribute) customAttribute).Name;
            return element.Name;
        }

        public static string IdentityFieldStr<TEntity>(string oldId)
        {
            var propertyInfo = typeof(TEntity).GetProperties().FirstOrDefault(p => p.IsIdField());
            return propertyInfo != null ? propertyInfo.Name : oldId;
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

        [Obsolete("Obsolete, Use IdentityFieldStr()")]
        public static string IdentityFiledStr<TEntity>(string oldId)
        {
            var propertyInfo = typeof(TEntity).GetProperties().FirstOrDefault(p => p.IsIdField());
            return propertyInfo != null ? propertyInfo.Name : oldId;
        }

        public static bool IsNonDbField(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute(typeof(NonDatabaseFieldAttribute)) != null ||
                   propertyInfo.GetCustomAttribute(typeof(NotMappedAttribute)) != null;
        }
        /// <summary>
        /// By default, all properties of a class are considered to be database columns. But it can be customized with this attribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static bool IsDbField(this PropertyInfo propertyInfo)
        {
            return true;
          //  return propertyInfo.GetCustomAttribute(typeof(SqlRepoDbFieldAttribute)) != null;
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
            return typeof(TEntity).GetProperties().FirstOrDefault(p => p.IsKeyField<TEntity>()) != null;
        }

        public static string FirstKeyFieldStr<TEntity>(string oldId)
        {
            var propertyInfo = typeof(TEntity).GetProperties().FirstOrDefault(p => p.IsKeyField<TEntity>());
            return propertyInfo != null ? propertyInfo.Name : oldId;
        }

        public static List<string> ListKeyFieldStr<TEntity>()
        {
            return typeof(TEntity).GetProperties().Where(p => p.IsKeyField<TEntity>()).Select(propertyInfo => propertyInfo.Name).ToList();
        }

        public static string DbTableName<TEntity>()
        {
            var customAttribute1 = typeof(TEntity).GetCustomAttribute(typeof(TableAttribute));
            if (customAttribute1 != null)
                return (customAttribute1 as TableAttribute)?.Name;
            var customAttribute2 = typeof(TEntity).GetCustomAttribute(typeof(TableNameAttribute));
            return customAttribute2 != null ? (customAttribute2 as TableNameAttribute)?.TableName : typeof(TEntity).Name;
        }

        public static bool IdKeyIsExclude<TEntity>()
        {
            if (typeof(TEntity).GetCustomAttribute(typeof(TableAttribute)) != null)
                return false;
            var customAttribute = typeof(TEntity).GetCustomAttribute(typeof(TableNameAttribute));
            return customAttribute != null && ((TableNameAttribute) customAttribute).ExcludeIdKey;
        }

        public static string DbTableSchema<TEntity>()
        {
            var customAttribute = typeof(TEntity).GetCustomAttribute(typeof(TableAttribute));
            return customAttribute != null ? (customAttribute as TableAttribute)?.Schema : "dbo";
        }

        public static string DbTableName<TEntity>(this TEntity entity)
        {
            var customAttribute1 = typeof(TEntity).GetCustomAttribute(typeof(TableAttribute));
            if (customAttribute1 != null)
                return (customAttribute1 as TableAttribute)?.Name;
            var customAttribute2 = typeof(TEntity).GetCustomAttribute(typeof(TableNameAttribute));
            if (customAttribute2 != null)
                return (customAttribute2 as TableNameAttribute)?.TableName;
            return typeof(TEntity).Name;
        }

      
    }
}