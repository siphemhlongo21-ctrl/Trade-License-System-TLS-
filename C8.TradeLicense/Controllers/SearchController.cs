using System.Linq;
using System.Web.Mvc;

using C8.TradeLicense.Models;
using C8.TradeLicense.DataAccessLayer;
using System.Web.Script.Serialization;

namespace C8.TradeLicense.Controllers
{
    public class SearchController : Controller
    {
        private TradeLicenseDbContext db = new TradeLicenseDbContext();
        private DA_Manual_NewEntities AutoCompletedb = new DA_Manual_NewEntities();
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        // GET: Search
        public JsonResult StreetName(string criteria)
        {

            var query = (from c in AutoCompletedb.MASTER_DA_GIS_DATA
                         where c.STRNAME.StartsWith(criteria)
                         orderby c.DISTRICT ascending
                         select new
                         {
                             STRNAME = c.STRNUM + " " + c.STRNAME
                       
                         }).Distinct().OrderBy(x => x.STRNAME).ToList();

            JsonResult result = Json(query);
            result.MaxJsonLength = 100675309;

            return Json(result.Data, JsonRequestBehavior.AllowGet);
      
          

        }
        public JsonResult Postalcode(string criteria)
        {
            //var query = (from c in AutoCompletedb.MASTER_DA_GIS_DATA
            //             where c.SUBURB.StartsWith(criteria)
            //             orderby c.SUBURB ascending
            //             select new
            //             {
            //                 ERf = c.ERF

            //             }).Distinct().FirstOrDefault();
            //GISOperations searchAddress = new GISOperations();
            //var GetRes = searchAddress.SearchAddressRequest(query.ERf);
            //// JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            //string result = javaScriptSerializer.Serialize(GetRes.GISResponseDetails);
            //result = result.Replace("null", "\"\"");
            ////JSONDataFile(result, "SearchAddress.json");
            //return Json(result);
            var query = (from c in AutoCompletedb.MASTER_DA_GIS_DATA
                         where c.SUBURB.StartsWith(criteria)
                         orderby c.SUBURB ascending
                         select new
                         {
                             PostalCode = c.PostalCode

                         }).Distinct().OrderBy(x => x.PostalCode).FirstOrDefault();

            JsonResult result = Json(query);
            result.MaxJsonLength = 100675309;

            return Json(result.Data, JsonRequestBehavior.AllowGet);



        }
        public JsonResult GetSubList(string criteria,string city)
       {
            if (city == "")
            {
                var query = (from c in AutoCompletedb.MASTER_DA_GIS_DATA
                             where c.SUBURB.StartsWith(criteria)
                             orderby c.SUBURB ascending
                             select new
                             {
                                 SUBURB = c.SUBURB

                             }).Distinct().OrderBy(x => x.SUBURB).ToList();
                JsonResult result = Json(query);
                result.MaxJsonLength = 100675309;

                return Json(result.Data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = (from c in AutoCompletedb.MASTER_DA_GIS_DATA
                             where c.DISTRICT.StartsWith(city)
                             orderby c.SUBURB ascending
                             select new
                             {
                                 SUBURB = c.SUBURB

                             }).Distinct().OrderBy(x => x.SUBURB).ToList();
                JsonResult result = Json(query);
                result.MaxJsonLength = 100675309;

                return Json(result.Data, JsonRequestBehavior.AllowGet);
            }

          
          



        }

        // GET: Search/Details/5
        public JsonResult GetAddress(int id)
        {


            var getAddress = (from N in AutoCompletedb.MASTER_DA_GIS_DATA
                              where N.ID == id
                              select new
                              {
                                  N.OBJECTID,
                                  N.REM,
                                  N.PORTION,
                                  N.ERF,
                                  N.FARMTOWNNA,
                                  N.STRNUM,
                                  N.STRNAME,
                                  N.STRTYPE,
                                  N.SUBURB,
                                  N.DISTRICT,
                                  N.CITY,
                                  N.WARDNO,
                                  N.PostalCode,
                                  N.ID
                              });
            JsonResult result = Json(getAddress);
            result.MaxJsonLength = 100675309;

            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        // GET: Search/Create
        public JsonResult GetCityList(string criteria)
        {

            var query = (from c in AutoCompletedb.MASTER_DA_GIS_DATA
                         where c.DISTRICT.StartsWith(criteria)
                         orderby c.DISTRICT ascending
                         select new
                         {
                             DISTRICT=  c.DISTRICT 
                         }).Distinct().ToList(); 
        
            JsonResult result = Json(query);
            result.MaxJsonLength = 100675309;
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
     


        // POST: Search/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Search/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Search/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Search/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Search/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
