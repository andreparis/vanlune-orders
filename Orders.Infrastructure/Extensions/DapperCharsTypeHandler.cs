using Dapper;
using Newtonsoft.Json;
using Orders.Domain.Entities.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Orders.Infrastructure.Extensions
{
    public class DapperCharsTypeHandler : SqlMapper.TypeHandler<IEnumerable<Characters>>
    {
        public override void SetValue(IDbDataParameter parameter, IEnumerable<Characters> value)
        {
            parameter.Value = value.ToString();
        }

        public override IEnumerable<Characters> Parse(object value)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Characters>>((string)value);
        }
    }
}
