using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace rappel.models
{
    public class domicile
    {
        public string nom { get; set; }
        public double longitude {  get; set; }
        public double latitude { get; set; }
       public int iddomicile { get; set; }
    }
}
