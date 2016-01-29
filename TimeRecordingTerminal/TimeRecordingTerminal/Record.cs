using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecordingTerminal
{
    class Record
    {
        public string Kartennummer;
        public int ReaderIDKommen, ReaderIDGehen;
        public bool erledigt;
        public DateTime Kommen, Gehen;
        public Record (string _Kartennummer, int _ReaderIDKommen, int _ReaderIDGehen)
        {
            erledigt = false;
            Kartennummer = _Kartennummer;
            ReaderIDKommen = _ReaderIDKommen;
            ReaderIDGehen = _ReaderIDGehen;
        }
        
    }
}
