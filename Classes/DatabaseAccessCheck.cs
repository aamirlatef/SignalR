using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace SignalRInbox.Classes
{
    public static class DatabaseAccessCheck
    {
        public static bool DoesUserHavePermission()
        {
            try
            {
                SqlClientPermission clientPermission = new SqlClientPermission(PermissionState.Unrestricted);
                clientPermission.Demand();

                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}