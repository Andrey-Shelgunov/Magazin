using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int DisplayOrder { get; set; }

        public Category(int id, string name, int order)
        {
            CategoryId = id;
            CategoryName = name;
            DisplayOrder = order;
        }

        public override string ToString() => CategoryName;
    }
}
