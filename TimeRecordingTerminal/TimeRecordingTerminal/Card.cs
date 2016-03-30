using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// Card is a Class which stores information for a valid Card.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The unique ID for the <see cref="Card"/> entry in the Database.
        /// </summary>
        public string id;
        /// <summary>
        /// The "Kartennummer" of the <see cref="Card"/>.
        /// </summary>
        public string kartenNummer;
        /// <summary>
        /// The StudentID which the <see cref="Card"/> refers to.
        /// </summary>
        public string studentID;
        /// <summary>
        /// The datetime where the <see cref="Card"/> has been activated.
        /// </summary>
        public string ausgestelltAm;
        /// <summary>
        /// The constructor of <see cref="Card"/> will create a new <see cref="Card"/>.
        /// </summary>
        /// <param name="_KartenNummer"><see cref="Card.kartenNummer"/></param>
        /// <param name="_StudentID"><see cref="Card.studentID"/></param>
        /// <param name="_ID"><see cref="Card.id"/></param>
        public Card(string _ID, string _KartenNummer,string _StudentID)
        {
            id = _ID;
            kartenNummer = _KartenNummer;
            studentID = _StudentID;
        }
    }
}
