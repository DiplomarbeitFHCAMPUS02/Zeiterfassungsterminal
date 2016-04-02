using System;
using System.Collections.Generic;
using System.Threading;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// The Class <see cref="CardLock"/> includes a system to lock specific Cards (stored as <see cref="Record"/>) and unlock them after <see cref="Config.cardlocktime"/> has expired.
    /// </summary>
    public class CardLock
    {
        /// <summary>
        /// This List contains all locked Cards (stored as <see cref="Record"/>).
        /// </summary>
        public static List<Record> lockedcards = new List<Record>();
        /// <summary>
        /// This is a function in <see cref="CardLock"/>. It will check if a Card (stored as <see cref="Record"/>) is locked.
        /// </summary>
        /// <param name="record"><see cref="Record"/></param>
        /// <returns>True if the Card is locked.</returns>
        public static bool iscardlocked(Record record)
        {
            foreach(Record r in lockedcards)
            {
                if(r.kartenNummer == record.kartenNummer)
                {
                    return true;
                }
            }
            record.kommen = DateTime.Now.ToString();
            lockedcards.Add(record);
            return false;
        }
        /// <summary>
        /// This is a function in <see cref="CardLock"/> which checks if the <see cref="Config.cardlocktime"/> of a Card (stored as <see cref="Record"/>) has already expired or not. If the <see cref="Config.cardlocktime"/> has expired, it will be removed from <see cref="lockedcards"/>.
        /// </summary>
        public static void cardlockchecker()
        {
            Config config = ConfigReader.getConfig();
            while (true)
            {
                try
                {
                    foreach (Record r in lockedcards)
                    {

                        try
                        {
                            DateTime cardexpiretime = DateTime.Parse(r.kommen);
                            cardexpiretime.AddSeconds(config.cardlocktime);
                            if (cardexpiretime <= DateTime.Now)
                            {
                                lockedcards.Remove(r);
                            }
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine(e.Message);
                        }

                        
                    }
                    Thread.Sleep(1000 * 30);
                }
                catch (Exception e)
                {

                }
                
            }
        }
    }
}
