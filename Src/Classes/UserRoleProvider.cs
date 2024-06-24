using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SimplifikasiFID.Models;
using SimplifikasiFID.ServiceSSO;

namespace SimplifikasiFID.Classes
{
    public class UserRoleProvider : RoleProvider
    {
        public SimplyFIDEntities db = new SimplyFIDEntities();

        public override string ApplicationName
        {
            get
            {
                return "";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            ArrayOfString res = new ArrayOfString();

            return res.ToArray();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                return ws.GetAllRoles(ApplicationName).ToArray();
            }
            finally
            {
                ws.Close();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                return ws.GetUsersInRole(ApplicationName, roleName).ToArray();
            }
            finally
            {
                ws.Close();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                return ws.IsUserInRole(username, ApplicationName, roleName);
            }
            finally
            {
                ws.Close();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
        }

        public override bool RoleExists(string roleName)
        {
            return GetAllRoles().Contains(roleName);
        }

        public bool CheckAllowModul(string userid, string modul, int access)
        {

            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                bool status = ws.GetAllowModul(userid, modul, access);
                return status;
            }
            finally
            {
                ws.Close();
            }

        }

        public bool CheckAllowExtendAccess(string userid, int Extid)
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                return ws.GetAccesExtend(userid, Extid);
            }
            finally
            {
                ws.Close();
            }
        }

        public string[] GetAccessForUser(string userid)
        {
            ArrayOfString access = new ArrayOfString();
            SSOWSSoapClient ws = new SSOWSSoapClient();

            List<dynamic> result = new List<dynamic>();
            var listacces = new int[] { 128, 160, 161, 162 };
            
            foreach (var ac in listacces)
            {
                var accessResult = new { UserId = userid, AccessExtendResult = ws.GetAccesExtend(userid, ac) };
                result.Add(accessResult);
            }

            try
            {
                return access.ToArray();
            }
            finally
            {
                ws.Close();
            }

        }

    }
}