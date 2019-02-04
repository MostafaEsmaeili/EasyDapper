using System;
using System.Collections.Generic;
using System.Data;

namespace EasyDapper
{
    public static class ClrTypeToSqlDbTypeMapper
    {
        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ClrTypeToSqlDbTypeMapper"/> class.
        /// </summary>
        static ClrTypeToSqlDbTypeMapper()
        {
            CreateClrTypeToSqlTypeMaps();
        }

        #endregion

        #region Public  Members

        /// <summary>
        /// Gets the mapped SqlDbType for the specified CLR type.
        /// </summary>
        /// <param name="clrType">The CLR Type to get mapped SqlDbType for.</param>
        /// <returns></returns>
        public static DbType GetSqlDbTypeFromClrType(Type clrType)
        {
            if (!_clrTypeToSqlTypeMaps.ContainsKey(clrType))
            {
                throw new ArgumentOutOfRangeException("clrType", @"No mapped type found for " + clrType);
            }

            DbType result;
            _clrTypeToSqlTypeMaps.TryGetValue(clrType, out result);
            return result;
        }

        #endregion

        #region Private Members

        private static void CreateClrTypeToSqlTypeMaps()
        {
            _clrTypeToSqlTypeMaps = new Dictionary<Type, DbType>
            {
                {typeof (bool), DbType.Binary},
                {typeof (bool?), DbType.Binary},
                {typeof (byte), DbType.Byte},
                {typeof (byte?), DbType.Byte},
                {typeof (string), DbType.String},
                {typeof (DateTime), DbType.DateTime},
                {typeof (DateTime?), DbType.DateTime},
                {typeof (short), DbType.Int16},
                {typeof (short?), DbType.Int16},
                {typeof (int), DbType.Int32},
                {typeof (int?), DbType.Int32},
                {typeof (long), DbType.Int64},
                {typeof (long?), DbType.Int64},
                {typeof (decimal), DbType.Decimal},
                {typeof (decimal?), DbType.Decimal},
                {typeof (double), DbType.Double},
                {typeof (double?), DbType.Double},
                {typeof (float), DbType.Double},
                {typeof (float?), DbType.Double},
                {typeof (TimeSpan), DbType.Time},
                {typeof (Guid), DbType.String},
                {typeof (Guid?), DbType.String},
                {typeof (byte[]), DbType.Binary},
                {typeof (byte?[]), DbType.Binary},
                {typeof (char[]), DbType.String},
                {typeof (char?[]), DbType.String}
            };
        }

        private static Dictionary<Type, DbType> _clrTypeToSqlTypeMaps; // = new 

        #endregion
    }
}