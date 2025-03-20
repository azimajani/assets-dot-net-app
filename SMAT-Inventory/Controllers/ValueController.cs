using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//using System.Globalization;
//using System.IO;
//using System.Text;
//using SMAT_Inventory.Models;
using SMAT_Inventory.Dapper;
//using Dapper;

//using OfficeOpenXml; //Assuming you are using EPPlus for Excel operations
//using System.Data;
//using System.Data.SqlClient;
//using SMAT_Inventory;
//using Newtonsoft.Json;
//using System.Net.Http;
//using System.Web.Routing;
//using DinkToPdf;
//using DinkToPdf.Contracts;

//using CsvHelper;
//using CsvHelper.Configuration;
//using System.Web.Configuration;
//using System.Security.Principal;

//using System.Net;
using System.Web.Http;
//using System.Web.Http.Results;
//using System.Runtime.InteropServices.ComTypes;
//using static System.Data.Entity.Infrastructure.Design.Executor;
//using System.Dynamic;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using System.Data.Entity.Infrastructure;
using SMAT_Inventory.Models;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Windows.Interop;
using Microsoft.Ajax.Utilities;
using System.Web.Http.Results;


namespace SMAT_Inventory.Controllers
{
    public class ValueController : ApiController
    {
        public bool GetuserBytoken(string token)
        {
            bool dataTF = false;
            int tmpresult = 0;
            using (DAPPER_DATA_SERVICE dapperTF = new DAPPER_DATA_SERVICE())
            {
                dapperTF.AddStringParameter("token", token, 500);
                // List to hold key-value pairs for each row
                var result = dapperTF.query_multiple("GetUserBytoken");
                if (result.hasRows())
                {
                    tmpresult = result.Read<int>().SingleOrDefault(); 
                    if (tmpresult == 1) { dataTF= true; }
                }
            }
           return dataTF;
        }

        [System.Web.Http.Route("api/branch/{id:int}/{token}")]
        public IHttpActionResult GetBranch(int id, string token)
        {

            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();
            List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("id", id);
                    // List to hold key-value pairs for each row
                    var retult = dapper.query_multiple("APIBranchSingleGet");

                    dataa = retult.Read<dynamic>().ToList();

                    foreach (var row in dataa)
                    {
                        // Dictionary to hold key-value pairs for the current row
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        // Loop through each property (key-value pair) in the dynamic object
                        foreach (var column in (IDictionary<string, object>)row)
                        {
                            string key = column.Key;        // The column name
                            object value = column.Value;    // The column value

                            // Print or process the key and value
                            if (key == "zone" && value is string)
                            {
                                // Create an object for zone field
                                rowData[key] = new { name = value };
                            }
                            else
                            {
                                // Add other key-value pairs as they are
                                rowData[key] = value;
                            }
                        }
                        resultData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success=true,
                data = resultData
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/branch/{token}")]
        public IHttpActionResult GetBranchList(string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();
            List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {

                    var retult = dapper.query_multiple("APIBranchList");

                    dataa = retult.Read<dynamic>().ToList();
                    foreach (var row in dataa)
                    {
                        // Dictionary to hold key-value pairs for the current row
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        // Loop through each property (key-value pair) in the dynamic object
                        foreach (var column in (IDictionary<string, object>)row)
                        {
                            string key = column.Key;        // The column name
                            object value = column.Value;    // The column value

                            // Print or process the key and value
                            if (key == "zone" && value is string)
                            {
                                // Create an object for zone field
                                rowData[key] = new { name = value };
                            }
                            else
                            {
                                // Add other key-value pairs as they are
                                rowData[key] = value;
                            }
                        }
                        resultData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = resultData
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/branch")]
        [HttpPost]
        public IHttpActionResult BranchSaveAPI([FromBody] branchModel model,string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }
            if (model == null)
            {
                return Ok(new { success = false, message = "Invalid Data." });
            }

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    AssetsController assetsController = new AssetsController();
                    var result = assetsController.branchsave(model);
                    var jsonData = result as dynamic;

                    if (jsonData.success == true)
                    {
                        return Ok(new { success = true, message = "Data saved successfully." });
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Data not saved."});
                    }
                }
            }
            catch (Exception ex)
            {
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }
        }

        [HttpPost]
        [HttpGet]
        [System.Web.Http.Route("api/userlogin/{username}/{password}")]
        public IHttpActionResult UserLogin(string username, string password)
        {
            List<dynamic> data = new List<dynamic>();


            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddUnicodeStringParameter("UserName", username, 255);
                    dapper.AddUnicodeStringParameter("Password", password, 255);

                    var retult = dapper.query_multiple("UserAuth");

                    data = retult.Read<dynamic>().ToList();
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            /*var combinedResult = new
            {
                EN = data
            };*/
            return Json(new
            {
                success = true,
                data = data
            });
        }

        [System.Web.Http.Route("api/users/{id:int}/{token}")]
        public IHttpActionResult GetUser(int id, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();
            List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("id", id);

                    var retult = dapper.query_multiple("APIGetUser");

                    dataa = retult.Read<dynamic>().ToList();
                    foreach (var row in dataa)
                    {
                        // Dictionary to hold key-value pairs for the current row
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        // Loop through each property (key-value pair) in the dynamic object
                        foreach (var column in (IDictionary<string, object>)row)
                        {
                            string key = column.Key;        // The column name
                            object value = column.Value;    // The column value

                            // Print or process the key and value
                            if (key == "roles" && value is string)
                            {
                                // Create an object for zone field
                                rowData[key] = new { name = value };
                            }
                            else
                            {
                                // Add other key-value pairs as they are
                                rowData[key] = value;
                            }
                        }
                        resultData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = resultData
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/users/{token}")]
        public IHttpActionResult GetUserList(string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();
            List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var retult = dapper.query_multiple("APIGetUserList");

                    dataa = retult.Read<dynamic>().ToList();
                    foreach (var row in dataa)
                    {
                        // Dictionary to hold key-value pairs for the current row
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        // Loop through each property (key-value pair) in the dynamic object
                        foreach (var column in (IDictionary<string, object>)row)
                        {
                            string key = column.Key;        // The column name
                            object value = column.Value;    // The column value

                            // Print or process the key and value
                            if (key == "roles" && value is string)
                            {
                                // Create an object for zone field
                                rowData[key] = new { name = value };
                            }
                            else
                            {
                                // Add other key-value pairs as they are
                                rowData[key] = value;
                            }
                        }
                        resultData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = resultData
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/zone/{id:int}/{token}")]
        public IHttpActionResult GetZone(int id, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();


            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("id", id);


                    var retult = dapper.query_multiple("APIGetZone");

                    dataa = retult.Read<dynamic>().ToList();
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = dataa
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/zone/{token}")]
        public IHttpActionResult GetZoneList(string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }
            List<dynamic> dataa = new List<dynamic>();


            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var retult = dapper.query_multiple("APIGetZoneList");

                    dataa = retult.Read<dynamic>().ToList();
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = dataa
            };
            return (IHttpActionResult)Json(combinedResult);
        }


        [System.Web.Http.Route("api/zone/{token}")]
        [HttpPost]
        public IHttpActionResult ZoneSaveAPI([FromBody] zoneModel model, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }
            if (model == null)
            {
                return Ok(new { success = false, message = "Invalid Data." });
            }

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    AssetsController assetsController = new AssetsController();
                    var result = assetsController.zoneasave(model);
                    var jsonData = result as dynamic;

                    if (jsonData.success == true)
                    {
                        return Ok(new { success = true, message = "Data saved successfully." });
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Data not saved."  });
                    }
                }
            }
            catch (Exception ex)
            {
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }
        }

        [System.Web.Http.Route("api/assets/{id:int}/{token}")]
        public IHttpActionResult GetAsset(int id, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();
            List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("id", id);

                    var retult = dapper.query_multiple("APIGetAsset");

                    dataa = retult.Read<dynamic>().ToList();
                    foreach (var row in dataa)
                    {
                        // Dictionary to hold key-value pairs for the current row
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        // Loop through each property (key-value pair) in the dynamic object
                        foreach (var column in (IDictionary<string, object>)row)
                        {
                            string key = column.Key;        // The column name
                            object value = column.Value;    // The column value

                            // Print or process the key and value
                            if (key == "branch" && value is string)
                            {
                                // Create an object for zone field
                                rowData[key] = new { name = value };
                            }
                            else
                            {
                                // Add other key-value pairs as they are
                                rowData[key] = value;
                            }
                        }
                        resultData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = resultData
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/assets/{token}")]
        public IHttpActionResult GetAssetList(string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();
            List<Dictionary<string, object>> resultData = new List<Dictionary<string, object>>();

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var retult = dapper.query_multiple("APIGetAssetList");

                    dataa = retult.Read<dynamic>().ToList();
                    foreach (var row in dataa)
                    {
                        // Dictionary to hold key-value pairs for the current row
                        Dictionary<string, object> rowData = new Dictionary<string, object>();

                        // Loop through each property (key-value pair) in the dynamic object
                        foreach (var column in (IDictionary<string, object>)row)
                        {
                            string key = column.Key;        // The column name
                            object value = column.Value;    // The column value

                            // Print or process the key and value
                            if (key == "branch" && value is string)
                            {
                                // Create an object for zone field
                                rowData[key] = new { name = value };
                            }
                            else
                            {
                                // Add other key-value pairs as they are
                                rowData[key] = value;
                            }
                        }
                        resultData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = resultData
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/assets/{token}")]
        [HttpPost]
        public IHttpActionResult AssetSaveAPI([FromBody] assetsModel model, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }
            if (model == null)
            {
                return Ok(new { success = false, message = "Invalid Data." });
            }

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    AssetsController assetsController = new AssetsController();
                    var result = assetsController.assetsave(model);
                    var jsonData = result as dynamic;

                    if (jsonData.success == true)
                    {
                        return Ok(new { success = true, message = "Data saved successfully." });
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Data not saved." });
                    }
                }
            }
            catch (Exception ex)
            {
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }
        }

        [System.Web.Http.Route("api/groups/{id:int}/{token}")]
        public IHttpActionResult GetGroup(int id, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }
            List<dynamic> dataa = new List<dynamic>();


            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    dapper.AddIntegerParameter("id", id);


                    var retult = dapper.query_multiple("APIGetGroup");

                    dataa = retult.Read<dynamic>().ToList();
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = dataa
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/groups/{token}")]
        public IHttpActionResult GetGroupList(string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }

            List<dynamic> dataa = new List<dynamic>();


            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    var retult = dapper.query_multiple("APIGetGroupList");

                    dataa = retult.Read<dynamic>().ToList();
                }
            }
            catch (Exception ex)
            {
                //dataa = new List<dynamic>();
                //sysExceptions.logException(ex.Message, ex.StackTrace);
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }

            var combinedResult = new
            {
                success = true,
                data = dataa
            };
            return (IHttpActionResult)Json(combinedResult);
        }

        [System.Web.Http.Route("api/groups/{token}")]
        [HttpPost]
        public IHttpActionResult GroupsSaveAPI([FromBody] groupModel model, string token)
        {
            if (!GetuserBytoken(token))
            {
                var combinedResultTF = new
                {
                    success = true,
                    message = "Invalid Token"
                };
                return (IHttpActionResult)Json(combinedResultTF);
            }
            if (model == null)
            {
                return Ok(new { success = false, message = "Invalid Data." });
            }

            try
            {
                using (DAPPER_DATA_SERVICE dapper = new DAPPER_DATA_SERVICE())
                {
                    AssetsController assetsController = new AssetsController();
                    var result = assetsController.groupsave(model);
                    var jsonData = result as dynamic;

                    if (jsonData.success == true)
                    {
                        return Ok(new { success = true, message = "Data saved successfully." });
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Data not saved." });
                    }
                }
            }
            catch (Exception ex)
            {
                var combinedResultex = new
                {
                    success = false,
                    message = ex.Message
                };
                return (IHttpActionResult)Json(combinedResultex);
            }
        }
    }

}



