using System.Data;
using System.Globalization;
using Dapper;

namespace Milestone05_WebAPI.Repositories;

public class DateTypeHandler : SqlMapper.TypeHandler<DateTime>
{
    private const string Format = "yyyy-MM-dd";

    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString(Format, CultureInfo.InvariantCulture);
    }

    public override DateTime Parse(object value)
    {
        string text = value.ToString() ?? "";
        return DateTime.ParseExact(text.Substring(0, 10), Format, CultureInfo.InvariantCulture);
    }
}
