using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class Country
    {
        #region attributes
        [Key]
        public string Name { get; set; }
        public List<Area> Areas { get; set; } // virtual enabled lazy loading
        #endregion
    }
}
