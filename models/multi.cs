using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace rappel.models
{
    public class DataService
    {
        public List<domicile> listdoms { get; set; }
        public List<messages> listmsg { get; set; } = new List<messages>();
        public List<MmsMessage> listmms { get; set; } = new List<MmsMessage>();
        public List<contact> listcont { get; set; }
        public int delai { get; set; } = 5;
        public int derindex { get; set; } = 0;
        public int derindexmms { get; set; } = 0;
        public int dermms { get; set; } = 0;
        public bool adom { get; set; } = true;
        public Color cback { get; set; } = Colors.DarkBlue;
        public Color ctxt { get; set; } = Colors.Black;
        public int ttxt { get; set; } = 18;
        public bool activer { get; set; } = true;

        public PermissionStatus permsms;

        public PermissionStatus permcontact;

        public PermissionStatus permgpsperm;

        public PermissionStatus permgpstemp;

    }
   


}
