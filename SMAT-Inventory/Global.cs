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
        public static int AppUserId=20;
        public static string AppUserName="Zeesshan";
        public static string AppUserEmail = "zeeshan.cna@gmail.com";
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