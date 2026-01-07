using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rappel.models
{
    public class MmsMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public List<string> MediaPaths { get; set; } = new();
        public string Sender { get; set; } // nouvel attribut
    }

    public interface IMmsService
    {
        List<MmsMessage> GetMmsMessages();
    }
}
