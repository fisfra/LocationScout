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
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Area> Areas { get; set; } 
        public List<SubArea>SubAreas { get; set; }
        #endregion
    }
}
