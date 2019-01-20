using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocationScout.Model
{
    public class Photo : IEquatable<Photo>
    {
        #region attributes
        public long Id { get; set; }
        public byte[] ImageBytes { get; set; }

        public ShootingLocation ShootingLocation { get; set; }
        #endregion

        #region constructors
        public Photo()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Photo);
        }

        public bool Equals(Photo other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        public static bool operator ==(Photo photo1, Photo photo2)
        {
            return EqualityComparer<Photo>.Default.Equals(photo1, photo2);
        }

        public static bool operator !=(Photo photo1, Photo photo2)
        {
            return !(photo1 == photo2);
        }
        #endregion
    }
}
