﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class SubjectLocation : GPSLocationBase
    {
        #region attributes
        // navigation properties
        public Country Country {get; set;}
        public Area Area {get; set;}
        public SubArea SubArea { get; set; }
        public List<ShootingLocation> ShootLocations { get; set; }
        #endregion

        #region constructors
        public SubjectLocation()
        {
        }
        #endregion
    }
}
