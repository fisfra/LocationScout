using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    internal class SubArea
    {
        #region attributes
        private string _name;
        #endregion

        #region constructors
        public SubArea(string name)
        {
            Name = name;
        }

        public string Name { get => _name; private set => _name = value; }
        #endregion
    }
}
