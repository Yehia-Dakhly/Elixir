using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Governorate : BaseEntity<int>
    {
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public ICollection<City> Cities { get; set; } = new HashSet<City>();
    }
}
