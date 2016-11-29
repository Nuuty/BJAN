using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        private static Dictionary<int, bool> stateDictionary = new Dictionary<int, bool>();
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
                if (!stateDictionary.ContainsKey(toiletId) || stateDictionary[toiletId] != status.State)
                {
                    DbC.Status.Add(status);
                    DbC.SaveChanges();
                    stateDictionary[toiletId] = status.State;
                }
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

        public List<ReturnItem> GetStateForAll()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(@"
            SELECT b.Toiletid, Status FROM Status JOIN(
            SELECT Toiletid, Max(Id) as a FROM Status GROUP BY ToiletId) as b
            on Status.Id = b.a
            ", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<ReturnItem> states = new List<ReturnItem>();
            while (reader.Read())
            {
                ReturnItem returnItem = new ReturnItem
                {
                    id = reader.GetInt32(0),
                    state = reader.GetBoolean(1)
                };
               states.Add(returnItem);
            }
            return states;
        }

        public Dictionary<int, List<StatisicValues>> Statistic()
        {
            String sql = @"
select count(stat.LightOn) as antal, TId, stat.InsertDate
from (
	select Status as LightOn, ToiletId as TId, CAST(InsertedDatetime  AS DATE) as InsertDate 
	from Status
	where Status = 1
	) as stat
	group by stat.TId, stat.InsertDate
";

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString);
            conn.Open();

            SqlCommand cmd = new SqlCommand(sql);
            SqlDataReader reader = cmd.ExecuteReader();
            Dictionary<int, List<StatisicValues>> returnData = new Dictionary<int, List<StatisicValues>>();


            while (reader.Read())
            {
                if (!returnData.ContainsKey(reader.GetInt32(1)))
                {
                    returnData.Add(reader.GetInt32(1), new List<StatisicValues>());
                }
                returnData[reader.GetInt32(1)].Add(new StatisicValues()
                {
                    date = reader.GetDateTime(2),
                    value = reader.GetInt32(1)
                });
            }



            return returnData;


        }
    }

    public class StatisicValues
    {
        public DateTime date;
        public int value;
    }
}
