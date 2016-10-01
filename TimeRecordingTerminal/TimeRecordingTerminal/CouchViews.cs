using MyCouch;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// This class contains the JSON Views for CouchDB
    /// </summary>
    public class CouchViews
    {
        static MyCouchClient client_Buchungen = LocalDB.ClientBuilder(ConfigReader.getConfig());
        static MyCouchClient client_Karten = LocalDB.ClientBuilder(ConfigReader.getConfig(), true);
        /// <summary>
        /// Function in <see cref="CouchViews"/> to create a view which will show the <see cref="Record.kartenNummer"/> and the relating <see cref="Record"/>.
        /// </summary>
        public async static void createKartenNummerview()
        {
            var request = "{  \"_id\": \"_design/view\",  \"language\": \"javascript\",  \"views\": { \"KartenNummer\": { \"map\": \"function(doc) { emit(doc.kartenNummer, doc);}\"} }        }";
            MyCouch.Responses.DocumentHeaderResponse response1 = await client_Karten.Documents.PostAsync(new MyCouch.Requests.PostDocumentRequest(request));
        }
        /// <summary>
        /// Function in <see cref="CouchViews"/> to create a view which will show all not finished <see cref="Record"/>s. <seealso cref="Record.erledigt"/>
        /// </summary>
        public async static void createerledigtview()
        {
            MyCouch.Responses.DocumentHeaderResponse response1 = await client_Buchungen.Documents.PostAsync(new MyCouch.Requests.PostDocumentRequest("{  \"_id\": \"_design/view\",  \"language\": \"javascript\",  \"views\": { \"erledigt\": { \"map\": \"function(doc) { if(doc.erledigt == '0'){  emit(doc.kartenNummer);}}\" } }        }"));
        }
        /// <summary>
        /// Function in <see cref="CouchViews"/> to create a view which will show all finished <see cref="Record"/> <seealso cref="Record.erledigt"/>  
        /// </summary>
        public async static void createfertigeRecordsview()
        {
            MyCouch.Responses.DocumentHeaderResponse response1 = await client_Buchungen.Documents.PostAsync(new MyCouch.Requests.PostDocumentRequest("{  \"_id\": \"_design/view2\",  \"language\": \"javascript\",  \"views\": { \"fertigeRecords\": { \"map\": \"function(doc) { if(doc.erledigt == '1'){  emit(doc.kartenNummer);}}\" } }        }"));
        }
    }
}
