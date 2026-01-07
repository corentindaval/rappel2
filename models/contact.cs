using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rappel.models
{
    public class contact
    {
       public string id { get; set; }
       public string prefix { get; set; }
        public  string prenom { get; set; }
        public  string sprenom { get; set; }
        public  string nom { get; set; }
        public  string suffix { get; set; }
        public  string surnom { get; set; }
        public  List<ContactPhone> numeros { get; set; }
        public  List<ContactEmail> emails { get; set; }
    }
}
