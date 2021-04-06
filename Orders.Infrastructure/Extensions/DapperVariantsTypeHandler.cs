using Dapper;
using Newtonsoft.Json;
using Orders.Domain.Entities;
using System.Data;

namespace Orders.Infrastructure.Extensions
{
    public class DapperVariantsTypeHandler : SqlMapper.TypeHandler<Variants>
    {
        public override void SetValue(IDbDataParameter parameter, Variants value)
        {
            parameter.Value = value.ToString();
        }

        public override Variants Parse(object value)
        {
            return JsonConvert.DeserializeObject<Variants>((string)value);
        }
    }
}
