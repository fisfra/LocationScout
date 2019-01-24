using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class Location
    {
        #region attributes
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region constructors
        public Location()
        {
        }
        #endregion
    }
}
