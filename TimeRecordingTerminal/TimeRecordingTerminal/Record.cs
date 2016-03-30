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
        /// KartenID is the unique ID of a card.
        /// </summary>
        public string kartenID;
        /// <summary>
        /// KartenNummer is the Tag from the Card.
        /// </summary>
        public string kartenNummer;
        /// <summary>
        /// studentID is the ID from the Student who owns the Card.
        /// </summary>
        public string studentID;
        /// <summary>
        /// The ID from the Terminal where the user arrived.
        /// </summary>
        public int readerIDKommen;
        /// <summary>
        /// The ID from the Terminal where the user left.
        /// </summary>
        public int readerIDGehen;
        /// <summary>
        /// Is the Record completed?
        /// </summary>
        public bool erledigt;
        /// <summary>
        /// Is the Record valid?
        /// </summary>
        public bool gueltig;
        /// <summary>
        /// Time of arrival.
        /// </summary>
        public string kommen;
        /// <summary>
        /// Time of leaving.
        /// </summary>
        public string gehen;

        /// <summary>
        /// This is the constructor of Record and it will create the first Record(<see cref="Record"/>).
        /// </summary>
        /// <param name="_kartenID"><see cref="Record.kartenID"/></param>
        /// <param name="_KartenNummer"><see cref="Record.kartenNummer"/></param>
        /// <param name="_ReaderIDKommen"><see cref="Record.readerIDGehen"/></param>
        /// <param name="_Kommen"><see cref="Record.kommen"/></param>
        public Record (string _KartenNummer, int _ReaderIDKommen, string _Kommen)
        {
            erledigt = false;
            kartenNummer = _KartenNummer;
            readerIDKommen = _ReaderIDKommen;
            kommen = _Kommen;
            gueltig = false;
        }
        /// <summary>
        /// This is the function completeRecord from <see cref="Record"/> class and it will complete the <see cref="Record"/>(Step2 <see cref="Record"/>)
        /// </summary>
        /// <param name="_ReaderIDGehen"><see cref="Record.readerIDGehen"/></param>
        /// <param name="_Gehen"><see cref="Record.gehen"/></param>
        public void completeRecord(int _ReaderIDGehen, string _Gehen)
        {
            this.readerIDGehen = _ReaderIDGehen;
            this.gehen = _Gehen;
            if (kartenNummer != null && kommen != null && gehen != null && !gehen.Contains("23:59"))
            {
                erledigt = true;
                gueltig = true;
            }
            if (kartenNummer != null && kommen != null && gehen != null && !gehen.Contains("23:59"))
            {
                erledigt = true;
                gueltig = false;
            }
        }
        
    }
}
