using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Common
{
    public static class SqlColumnTypes
    {

        public static string Decimal(int precision = 18, int scale = 2)
            => $"decimal({precision},{scale})";

        public static string Varchar(int length)
            => $"varchar({length})";

        public static string Nvarchar(int length)
            => $"nvarchar({length})";

        public static string Date() => "date";

        public static string Money() => "money";
        public static string Json() => "json";
    }
}
