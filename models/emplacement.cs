using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rappel.models
{
    public class emplacement
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public emplacement(double latitude, double longitude) { 
            Latitude = latitude;
            Longitude = longitude;
        
        }

       

    }
}
