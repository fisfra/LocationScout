using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    internal class GPSCoordinatesHelper
    {
        public enum CoordinatesPosition
        {
            N, E, S, W
        }

        public double Degrees { get; set; }
        public double Minutes { get; set; }
        public double Seconds { get; set; }
        public CoordinatesPosition Position { get; set; }

        public GPSCoordinatesHelper() { }

        public static CoordinatesPosition GetPosition(double? value, E_CoordinateType type)
        {
            CoordinatesPosition cp;
            switch (type)
            {
                case E_CoordinateType.Longitude:
                    cp = (value > 0) ? CoordinatesPosition.E : CoordinatesPosition.W;
                    break;
                case E_CoordinateType.Latitude:
                    cp = (value < 0) ? CoordinatesPosition.N : CoordinatesPosition.S;
                    break;
                default:
                    Debug.Assert(false);
                    throw (new Exception("Unknown enum value in GPSConverter::Convert"));
            }

            return cp;
        }

        public GPSCoordinatesHelper(double? value, CoordinatesPosition position)
        {
            //sanity
            if (value < 0 && position == CoordinatesPosition.N)
                position = CoordinatesPosition.S;
            //sanity
            if (value < 0 && position == CoordinatesPosition.E)
                position = CoordinatesPosition.W;
            //sanity
            if (value > 0 && position == CoordinatesPosition.S)
                position = CoordinatesPosition.N;
            //sanity
            if (value > 0 && position == CoordinatesPosition.W)
                position = CoordinatesPosition.E;

            var decimalValue = Convert.ToDecimal(value);

            decimalValue = Math.Abs(decimalValue);

            var degrees = Decimal.Truncate(decimalValue);
            decimalValue = (decimalValue - degrees) * 60;

            var minutes = Decimal.Truncate(decimalValue);
            var seconds = (decimalValue - minutes) * 60;

            Degrees = Convert.ToDouble(degrees);
            Minutes = Convert.ToDouble(minutes);
            Seconds = Math.Round(Convert.ToDouble(seconds), 2);
            Position = position;
        }

        public GPSCoordinatesHelper(double degrees, double minutes, double seconds, CoordinatesPosition position)
        {
            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Position = position;
        }

        public double ToDouble()
        {
            var result = (Degrees) + (Minutes) / 60 + (Seconds) / 3600;
            return Position == CoordinatesPosition.W || Position == CoordinatesPosition.S ? -result : result;
        }

        public override string ToString()
        {
            return Degrees + "º " + Minutes + "' " + Seconds + "''" + Position;
        }

        public string ToGoogleMapsString()
        {
            // get the initial string
            var value = ToString();

            // replace the "," by "."
            value = value.Replace(',', '.');

            // converts for an url string (e.g. replace spaces by %20)
            value = System.Uri.EscapeDataString(value);

            return value;
        }
    }
}
