using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class Country
    {
        #region attributes
        private List<Area> _allAreas;

        public Country(string name)
        {
            Name = name;
        }
        #endregion

        #region contructors
        public Country(string name, List<Area> allAreas)
        {
            Name = name;
            _allAreas = allAreas;
        }

        public string Name { get; set; }
        internal List<Area> AllAreas { get => _allAreas; private set => _allAreas = value; }
        #endregion

    }
}
