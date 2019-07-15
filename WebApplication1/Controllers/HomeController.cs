using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace ngFormey.Web.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {

        ngFormey.Web.Models.frmy01_DevContext formsDB = new ngFormey.Web.Models.frmy01_DevContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var forms = formsDB.FormLists.ToList();

            return View(forms);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Form(int id)
        {
            //var album = storeDB.Albums.Find(id);
            //return View(album);
            ViewBag.Message = id;
            var form = formsDB.FormLists.Find(id);
            ViewBag.Message = form.Title;
            return View();
        }

        public ActionResult F(int id)
        {
            //var album = storeDB.Albums.Find(id);
            //return View(album);
            ViewBag.Message = id;
            var form = formsDB.FormLists.Find(id);
            ViewBag.Message = form.Title;
            return View();
        }


    }
}