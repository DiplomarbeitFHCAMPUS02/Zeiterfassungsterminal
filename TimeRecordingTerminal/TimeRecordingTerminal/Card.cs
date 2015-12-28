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
        public NFCCard(string _ID, string _CardID, string _StudentID, string _time)
        {
            ID_unique = _ID;
            CardID = _CardID;
            StudentID = _StudentID;
            time = _time;
        }
		
        public string ID_unique;
        public string CardID;
        public string StudentID;
        public string time;
        

    }
}
