using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeRecordingTerminal
{
    public class NFCCard
    {
		//In der Klasse NFCCard:
		//Constructor erstellen
		//Beim erstellen einer NFCCard Instanz werden mit dem Constructor die Werte festgelegt
        public NFCCard(string _Kartennummer, string _time)
        {
            Kartennummer = _Kartennummer;
            time = _time;
        }

        public string Kartennummer;
        public string time;
        

    }
}
