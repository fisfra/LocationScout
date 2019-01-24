using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class Area : Location
    {
        #region attributes
        public List<SubArea> SubAreas { get; set; }
        public List<Country> Countries { get; set; }
        public List<SubjectLocation> SubjectLocations { get; set; }
        #endregion
    }
}
