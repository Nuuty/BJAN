using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WebService.svc or WebService.svc.cs at the Solution Explorer and start debugging.
    public class WebService : IWebService
    {
        public DbC DbC { get; set; } = new DbC();


        public void InsertData(string id, string value)
        {
            int toiletId;
            int light;
            if (int.TryParse(id, out toiletId) && int.TryParse(value, out light))
            {
                Status status = new Status
                {
                    Id = 0,
                    InsertedDatetime = DateTime.Now,
                    State = light < 230,
                    ToiletId = toiletId
                };
                DbC.Status.Add(status);
                DbC.SaveChanges();
            }
        }

        public bool GetStateById(string id)
        {
            int toiletId;
            if (!int.TryParse(id, out toiletId))
            {
                throw new WebFaultException<string>("", HttpStatusCode.NotFound);
            }
            try
            {
                return DbC.Status.OrderByDescending(status => status.Id)
                            .Where(x => x.ToiletId == toiletId)
                            .Select(status => status.State)
                            .First();
            }
            catch (Exception)
            {
                throw new WebFaultException<string>("", HttpStatusCode.NotFound);
            }
        }
    }
}
