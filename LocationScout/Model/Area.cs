using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class Area
    {
        #region attributes
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public List<SubArea> Subareas { get; set; }
        public List<Country> Countries { get; set; }
        #endregion
    }
}
