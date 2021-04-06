using Dapper;
using Newtonsoft.Json;
using Orders.Domain.Entities.ExProduct;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Orders.Infrastructure.Extensions
{
    public class DapperCustomizesTypeHandler : SqlMapper.TypeHandler<IEnumerable<Customize>>
    {
        public override void SetValue(IDbDataParameter parameter, IEnumerable<Customize> value)
        {
            parameter.Value = value.ToString();
        }

        public override IEnumerable<Customize> Parse(object value)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Customize>>((string)value);
        }
    }
}
