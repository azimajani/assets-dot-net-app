using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMAT_Inventory.Models;
using Newtonsoft.Json;

namespace SMAT_Inventory
{

    public class GlobalVariables
    {

        public static List<WebSetting> GetWebSettingValue = new List<WebSetting>();
        public static List<userpermissions> GetUserPermissionsValue = new List<userpermissions>();
        public static int AppUserId;
        public static string AppUserName;
        public static string AppUserEmail;
        public static string remember_token;
        public static string role;
        public static string AppUserPassword;
        public static string logintype;
        public static int AssetTransfer_AddReviewer;
        public static int AssetTransfer_Approver;
        public static int AssetTransfer_Receiver;

        public static string GetWebSetting(string tag, string DefaultValue = "")
        {

            //int Encrype = GlobalVariables.GetWebSettingValue.Where(item => item.tag == tag).Select(item => item.Encrype).FirstOrDefault();
            string Value = GlobalVariables.GetWebSettingValue.Where(item => item.tag == tag).Select(item => item.Value).FirstOrDefault();
            
            if (Value == null) { Value = DefaultValue; }
            return Value;
        }


        public static string GetPersonColumnValue(string person, string col)
        {
            string unescapedPerson = person.Replace("\\\"", "\"");
            // Deserialize the JSON string into a dictionary
            //var person1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(person);
            var personDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(unescapedPerson);
            if (personDict == null) { return ""; }
            if (personDict.TryGetValue(col, out object colout))
            {
                if (colout != null)
                {
                    return colout.ToString();
                }
            }

            return "";
        }

        public static string GetUserPermissions(string Module, string Permission)
        {
            string Value = null;

            if (Permission == "Add")
            {
                Value = GlobalVariables.GetUserPermissionsValue.Where(item => item.Module == Module).Select(item => item.AddPermission).FirstOrDefault();
            }
            if (Permission == "Edit")
            {
                Value = GlobalVariables.GetUserPermissionsValue.Where(item => item.Module == Module).Select(item => item.EditPermission).FirstOrDefault();
            }
            if (Permission == "View")
            {
                Value = GlobalVariables.GetUserPermissionsValue.Where(item => item.Module == Module).Select(item => item.ViewPermission).FirstOrDefault();
            }
            if (Permission == "Delete")
            {
                Value = GlobalVariables.GetUserPermissionsValue.Where(item => item.Module == Module).Select(item => item.DeletePermission).FirstOrDefault();
            }
            if (Value == null) { Value = null; }
            return Value;
        }
        /*public static Boolean GetWebConfigBoolean(string Name, Boolean DefaultValue = false)
        {

            int Encrype = GLOBALS.GetWebConfigValue.Where(item => item.Name == Name).Select(item => item.Encrype).FirstOrDefault();
            string Value = GLOBALS.GetWebConfigValue.Where(item => item.Name == Name).Select(item => item.Value).FirstOrDefault();
            Boolean rValue = false;

            if (Encrype == 1)
            {
                Value = EnDe.Decrypt(Value);
            }
            if (Value == null) { rValue = DefaultValue; }
            else if (Value.ToUpper() == "TRUE") { rValue = true; }

            return rValue;
        }*/
    }
}