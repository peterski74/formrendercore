using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ngFormey.Web.Extensions;
using System.Globalization;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace ngFormey.Web.Controllers
{
  

    public class paypalController : Controller
    {
        WebApplication1.Models.frmy01_DevContext formsDB = new WebApplication1.Models.frmy01_DevContext();

        public ActionResult Confirmed(Guid id, string token, string payerId)
        {
            
            return Content("ata");
        }

    }
}