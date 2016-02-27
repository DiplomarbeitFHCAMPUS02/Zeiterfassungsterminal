namespace TimeRecordingTerminal
{
    /// <summary>
    /// Record is a Class which stores the information for one Record.
    /// A Record runs threw two steps:
    ///     Step1: When a new KartenNummer arrives it will create a new Record with KartenNummer
    ///            ReaderIDKommen(ID of Terminal where action happened), Kommen(Time of arrival)
    ///            and erledigt will be false -> The Record is not finished now!
    ///     Step2: Record finishes when KartenNummer arrives second time.
    ///            ReaderIDGehen will be saved and the time Gehen(departure) and erledigt will be true. Record is completed!
    /// </summary>
    public class Record
    {
        

        //Buchung Step1: KartenNummer, ReaderIDKommen, !erledigt, Kommen
        //Buchung Step2: ReaderIDGehen, erledigt, Gehen
        /// <summary>
        /// KartenNummer is the Tag from the Card. StudentID is the ID from the Student.
        /// </summary>
        public string KartenNummer, StudentID;
        /// <summary>
        /// The ID from the Terminal where the User Arrived/Left
        /// </summary>
        public int ReaderIDKommen, ReaderIDGehen;
        /// <summary>
        /// Is the Record completed?
        /// </summary>
        public bool erledigt, gueltig;
        /// <summary>
        /// Arriving/Leavingtime
        /// </summary>
        public string Kommen, Gehen;


        /// <summary>
        /// This is the constructor of Record and it will create the first Record(<see cref="Record"/>).
        /// </summary>
        /// <param name="_KartenNummer"></param>
        /// <param name="_ReaderIDKommen"></param>
        /// <param name="_Kommen"></param>
        public Record (string _KartenNummer, int _ReaderIDKommen, string _Kommen)
        {
            erledigt = false;
            KartenNummer = _KartenNummer;
            ReaderIDKommen = _ReaderIDKommen;
            Kommen = _Kommen;
        }
        /// <summary>
        /// This is the function completeRecord from Record class and it will complete the Record(Step2 <see cref="Record"/>)
        /// </summary>
        /// <param name="_ReaderIDGehen"></param>
        /// <param name="_Gehen"></param>
        public void completeRecord(int _ReaderIDGehen, string _Gehen)
        {
            this.ReaderIDGehen = _ReaderIDGehen;
            this.Gehen = _Gehen;
            if (KartenNummer != null && Kommen != null && Gehen != null && Gehen.Contains("23:59"))
            {
                erledigt = true;

            }
        }
        
    }
}
