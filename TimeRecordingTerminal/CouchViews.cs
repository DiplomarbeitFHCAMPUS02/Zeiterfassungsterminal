using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCouch;

namespace TimeRecordingTerminal
{
    public class CouchViews
    {
        static MyCouchClient client_Buchungen = LocalDB.ClientBuilder(ConfigReader.getConfig());
        static MyCouchClient client_Karten = LocalDB.ClientBuilder(ConfigReader.getConfig(), true);
        public async static void createKartenNummerview()
        {
            var request = "{  \"_id\": \"_design/view\",  \"language\": \"javascript\",  \"views\": { \"KartenNummer\": { \"map\": \"function(doc) {n emit(doc.KartenNummer, doc.StudentID);n}n}\"} }        }";
            MyCouch.Responses.DocumentHeaderResponse response1 = await client_Karten.Documents.PostAsync(new MyCouch.Requests.PostDocumentRequest(request));
        }
        public async static void createerledigtview()
        {
            MyCouch.Responses.DocumentHeaderResponse response1 = await client_Buchungen.Documents.PostAsync(new MyCouch.Requests.PostDocumentRequest("{  \"_id\": \"_design/view\",  \"language\": \"javascript\",  \"views\": { \"erledgt\": { \"map\": \"function(doc) {n if(doc.erledigt == '0')n{n  emit(doc.kartennummer);n}n}\" } }        }"));
        }
    }
}
