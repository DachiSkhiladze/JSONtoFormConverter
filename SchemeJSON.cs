using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBGParser
{
    public class SchemeJSON
    {
        public string id { get; set; }
        public string type { get; set; }
        public List<Field> fields { get; set; }
    }

    public class Field
    {
        public Field()
        {
            fields = new List<Field>();
        }
        public string id { get; set; }
        public string type { get; set; }
        public List<Field>? fields { get; set; }
    }
}
