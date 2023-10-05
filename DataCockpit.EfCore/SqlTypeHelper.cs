using System.Data;

namespace DataCockpit.EfCore
{
    internal class SqlTypeHelper
    {

        private static Dictionary<Type, SqlDbType> typeMap;

        static SqlTypeHelper()
        {
            typeMap = new Dictionary<Type, SqlDbType>();

            typeMap[typeof(string)] = SqlDbType.NVarChar;
            typeMap[typeof(char[])] = SqlDbType.NVarChar;
            typeMap[typeof(byte)] = SqlDbType.TinyInt;
            typeMap[typeof(short)] = SqlDbType.SmallInt;
            typeMap[typeof(int)] = SqlDbType.Int;
            typeMap[typeof(long)] = SqlDbType.BigInt;
            typeMap[typeof(byte[])] = SqlDbType.Image;
            typeMap[typeof(bool)] = SqlDbType.Bit;
            typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
            typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
            typeMap[typeof(decimal)] = SqlDbType.Money;
            typeMap[typeof(float)] = SqlDbType.Real;
            typeMap[typeof(double)] = SqlDbType.Float;
            typeMap[typeof(TimeSpan)] = SqlDbType.Time;
        }

        public static SqlDbType? GetDbType(Type giveType)
        {
            if (typeMap.ContainsKey(giveType))
            {
                return typeMap[giveType];
            }

            return null;
        }

        public static SqlDbType? GetDbType<T>()
        {
            return GetDbType(typeof(T));
        }
    }
}
