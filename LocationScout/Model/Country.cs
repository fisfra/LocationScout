using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class Country : Location
    {
        #region attributes
        //public List<Area> Areas { get; set; } 
        public virtual ICollection<Area> Areas { get; set; }
        //public List<SubArea>SubAreas { get; set; }     
        public virtual ICollection<SubArea> SubAreas { get; set; }
        //public List<SubjectLocation> SubjectLocations { get; set; }
        public virtual ICollection<SubjectLocation> SubjectLocations { get; set; }
        #endregion
    }
}
