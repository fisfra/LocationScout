using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class SubArea
    {
        #region attributes
        [Key]
        public string Name { get; set; }
        public virtual List<Area> Areas { get; set; }
        #endregion
    }
}
