using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace rappel.models
{
    public class Active
    {
        private bool _status;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                    OnMyVariableChanged();
                }
            }
        }
               protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }

               private void OnMyVariableChanged()
               {
                    // Fonction qui s'active lorsque MyVariable est modifiée
                 


                }


    }
}
