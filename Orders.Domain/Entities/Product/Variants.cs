using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orders.Domain.Entities
{
    public class Variants : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("factor")]
        public decimal Factor { get; set; }

        public static Variants[] RemoveDuplicates(IEnumerable<Variants> vars)
        {
            var newVariants = new List<Variants>();

            foreach (var v in vars)
            {
                if (!newVariants.Any(a => a.Name.Equals(v.Name)))
                    newVariants.Add(v);
            }

            return newVariants.ToArray();
        }
    }
}
