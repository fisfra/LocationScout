using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class Area
    {
        #region
        private string _name;
        private List<SubArea> _allSubareas;
        #endregion

        #region contructor
        public Area(string areaName, List<SubArea> allSubareas)
        {
            Name = areaName;
            AllSubareas = allSubareas;
        }

        public string Name { get => _name; private set => _name = value; }
        internal List<SubArea> AllSubareas { get => _allSubareas; private set => _allSubareas = value; }
        #endregion
    }
}
