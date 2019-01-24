using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.Model
{
    public class SubArea : Location, IEquatable<SubArea>
    {
        #region attributes
        // navigation properties
        public List<Area> Areas { get; set; }
        public List<Country> Countries { get; set; }
        public List<SubjectLocation> SubjectLocation { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SubArea);
        }

        public bool Equals(SubArea other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        public static bool operator ==(SubArea area1, SubArea area2)
        {
            return EqualityComparer<SubArea>.Default.Equals(area1, area2);
        }

        public static bool operator !=(SubArea area1, SubArea area2)
        {
            return !(area1 == area2);
        }
        #endregion
    }
}
