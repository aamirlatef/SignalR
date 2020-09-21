using SignalRInbox.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace SignalRInbox
{
    public partial class home : System.Web.UI.Page
    {
        private string queryString
        {
            get
            {
                string query = Request.QueryString["query"];
                if (!string.IsNullOrEmpty(query))
                    return query;
                else
                    return 0.ToString();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            hdnEmpNo.Value = queryString;
        }
        protected void Notify_Click(object sender, EventArgs e)
        {
            notificationsHub notificationsHub = new notificationsHub();
            notificationsHub.SendMessage("3106166", "This is a Test Message");
        }

        [WebMethod]
        public static IEnumerable<NotificationData> GetData(int EmpNo)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT OUTSTANDING.ID FROM OUTSTANDING LEFT OUTER JOIN PT_WF_Inbox ON OUTSTANDING.ID = PT_WF_Inbox.ApplicationId WHERE OUTSTANDING.ShowNotification = 0 and PT_WF_Inbox.Emp_No = @EmpNo", connection))
                {
                    command.Notification = null;
                    command.Parameters.Add("@EmpNo", SqlDbType.Int);
                    command.Parameters["@EmpNo"].Value = EmpNo; 

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var reader = command.ExecuteReader())
                        return reader.Cast<IDataRecord>()
                            .Select(x => new NotificationData()
                            {
                                popupMessage = $"Request no. {x.GetInt64(0)} received at {DateTime.Now.ToString("dd-MM-yyyy hh:mm")}",
                                notificationMessage = $"Request no. {x.GetInt64(0)} received at {DateTime.Now.ToString("dd-MM-yyyy hh:mm")}",
                            }).ToList();
                }
            }
        }
        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //NotificationsHub.Show();
        }        
    }
    public class NotificationData
    {
        public string popupMessage { get; set; }
        public string notificationMessage { get; set; }
    }
}