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
        public NFCCard(string _CardNumber, string _time)
        {
            CardNumber = _CardNumber;
            time = _time;
        }

        public string CardNumber;
        public string time;
        

    }
}
