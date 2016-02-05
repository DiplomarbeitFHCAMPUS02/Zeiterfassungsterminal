using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecordingTerminal
{
    class Record
    {
        //Buchung Step1: Kartennummer, ReaderIDKommen, !erledigt, Kommen
        //Buchung Step2: ReaderIDGehen, erledigt, Gehen
        public string Kartennummer;
        public int ReaderIDKommen, ReaderIDGehen;
        public bool erledigt;
        public string Kommen, Gehen;
        public Record (string _Kartennummer, int _ReaderIDKommen, string _Kommen)
        {
            erledigt = false;
            Kartennummer = _Kartennummer;
            ReaderIDKommen = _ReaderIDKommen;
            Kommen = _Kommen;
        }

        public void completeRecord(int _ReaderIDGehen, string _Gehen)
        {
            this.ReaderIDGehen = _ReaderIDGehen;
            this.Gehen = _Gehen;
            if (Kartennummer != null && Kommen != null && Gehen != null)
            {
                erledigt = true;
            }
        }
        
    }
}
