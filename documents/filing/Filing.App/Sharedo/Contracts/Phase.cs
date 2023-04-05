using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filing.App.Sharedo.Contracts
{
    public record Phase(string SystemName, string Name)
    {
        public override string ToString()
        {
            return $"Phase SystemName:{SystemName}, Name:{Name}";
        }
    }
}
