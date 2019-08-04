

//TOLIVE to be uncommendted when going live or testing
using System;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using ngFormey.Web.BusinessObjects;
using Microsoft.AspNetCore.Http;
using WebApplication1.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using UAParser;
using ngFormey.Web.Models;
using Salesforce.Force;
using Salesforce.Common;
using Salesforce.Common.Models.Xml;
//using DeveloperForce.NetCore.Force;
//using DeveloperForce.NetCore.Common;
using Microsoft.Extensions.Configuration;
using Myconfig;


namespace ngFormey.Web.Controllers
{
    // FUNCTIONS
    // ---------
    // GetMonthsBetween(DateTime from, DateTime to)
    // d(Guid id) - display form
    // isMultiSelectField(string itemType)
    // UploadFile(string bucketName, string filepath, string formID, int submissionID, string fileName, string inputID, string LabelName) - AWS S3 upload
    // HandleForm(IDictionary<string, string> SubmitFields, Models.FormeyEntityModel model, FormCollection formCollection, FormList FL) - process submitted form
    // FormRedirect(int id, int selectedProductId, string productOrderId)
    // FormConfirmation(int id, string msg)
    // NotifyTime(string FormId, Guid? LogId)
    // SendForm(FormCollection formCollection) - send email with link to saved form
    // NotifyInteraction(string Name, string Address)
    // SaveForm(IDictionary<string, string> SubmitFields, Models.FormeyEntityModel model, FormCollection formCollection, FormList FL) - process on clicking save form
    // UpdateForm(IDictionary<string, string> SubmitFields, Models.FormeyEntityModel model, FormCollection formCollection, FormList FL) - process clicking update
    // s(Guid id) - display saved form
    // t(Guid id) - display template form


    public class ViewDataUploadFilesResult
    {
        public string Name { get; set; }
        public int Length { get; set; }
    }


    public class FController : Controller
    {

        IConfiguration _configuration;

        

        
        public static int GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsBetween(to, from);

            var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                return monthDiff - 1;
            }
            else
            {
                return monthDiff;
            }
        }

        //private void buildSobject(string sfObjName, string sfLabelName, string sfLabelValue)
        //{
        //    SObjectList<SObject> sObjlis;
        //    bool hasvalue = dict.TryGetValue(sfObjName, out sObjlis);
        //    if (hasvalue)
        //    {
        //        sObjlis.Add(new SObject { { sfLabelName, sfLabelValue } });
        //    }
        //    else
        //    {
        //        dtBatch.Add(new SObject { { sfLabelName, sfLabelValue } });
        //        dict.Add(sfObjName, dtBatch);

        //    }
        //}

//        IDictionary<string, SObjectList<SObject>> dict = new Dictionary<string, SObjectList<SObject>>();


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~Salesforce Start~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        SObjectList<SObject> dtBatch = new SObjectList<SObject>();

        IDictionary<string, SObjectList<SObject>> dict = new Dictionary<string, SObjectList<SObject>>();

        private void buildSobject(string sfObjName, string sfLabelName, string sfLabelValue)
        {
            SObjectList<SObject> sObjlis;

            bool hasvalue = dict.TryGetValue(sfObjName, out sObjlis);
            if (hasvalue) //add to existing object
            {
                dict[sfObjName][0].Add(sfLabelName, sfLabelValue); //only need to keep on addding to the ONE object as only one would be added to the form
            }
            else // new object
            {
                SObject so = new SObject();
                so.Add(sfLabelName, sfLabelValue);
                dtBatch.Add(so);
                dict.Add(sfObjName, dtBatch);
            }
        }

        private SfCredential getSFCredentils(string fListId)
        {
            var userId = formsDB.FormLists
               .Where(c => c.FormListId.ToString() == fListId).First();

            if(userId.UserId != null)
            {
                var sfCredentials = formsDB.SfCredentials
                    .Where(c1 => c1.UserId.ToString() == userId.UserId).FirstOrDefault(); ;
              
                var sfc = new SfCredential() { userid = sfCredentials.Username, password = sfCredentials.Password, consumerSecrete = sfCredentials.Consumersecret, consumerKey = sfCredentials.Consumerkey };
                return sfc;
            }
            return null;
        }

        private class SfCredential
        {
            public string userid { get; set; }
            public string password { get; set; }
            public string consumerSecrete { get; set; }
            public string consumerKey { get; set; }
        }

        private static async Task ConnectSalesforce(IDictionary<string, SObjectList<SObject>> objlist, SfCredential sfObj)
        {

            //create auth client to retrieve token
            var auth = new AuthenticationClient();

            //get back URL and token
            await auth.UsernamePasswordAsync(sfObj.consumerKey, sfObj.consumerSecrete, sfObj.userid, sfObj.password);

            var instanceUrl = auth.InstanceUrl;
            var accessToken = auth.AccessToken;
            var apiVersion = auth.ApiVersion;

            var forceClient = new ForceClient(instanceUrl, accessToken, apiVersion);

            dynamic res = "";

            // SEND EACH OBJET TO SALESFORCE
            // NEED TO DETERMINE ORDER TO CREATE RELATIONSHIPS HERE
            foreach (KeyValuePair<string, SObjectList<SObject>> item in objlist)
            {
                var sfBatch = new List<SObjectList<SObject>> { item.Value };

                res = await forceClient.RunJobAndPollAsync(
                    item.Key,
                    BulkConstants.OperationType.Insert,
                    sfBatch);
            }
        }



        ngFormey.Web.Models.frmy01_DevContext formsDB = new ngFormey.Web.Models.frmy01_DevContext();


        [ResponseCache(Duration = 20, VaryByQueryKeys = new string[] { "id" })]
        [HttpGet]
        public ActionResult d(Guid id) // DISPLAY FORM
        {
            // var formUrl = new Uri(System.Configuration.ConfigurationManager.AppSettings["http://localhost:26715"]);


            ViewBag.isLiveServer = ConfigurationManager.AppSetting["AppSettings:isLiveServer"];

            var formUrl = new Uri("https://localhost:44384");  //26715
            ViewBag.submitURL = formUrl + "f/HandleForm";
            ViewBag.formURL = formUrl;

            //formsDB.Configuration.AutoDetectChangesEnabled = false;

            var form = formsDB.FormLists
                .Include("Rules")
                .Include("Calculations")
                .Include("FieldItems.FieldItemElements")
                .Where(x => x.FormListId == id).FirstOrDefault(); 

            StringBuilder sb = new StringBuilder();
            ViewBag.isSaved = false; // used to show hide update or save
            ViewBag.captcha = false;

            var CheckMembershipStatus = checkMembershipStatus(form.UserId);
            bool quotaExceeded = CheckMembershipStatus.quotaExceeded;
            ViewBag.quotaExceeded = quotaExceeded;
            ViewBag.outHtml = CheckMembershipStatus.message.ToString();
            ViewBag.freeTier = CheckMembershipStatus.freeTier;
            if (form.Status=="ARCHIVED")
            {
                ViewBag.Status = form.Status;
                return View(form);
            }

            if (!quotaExceeded)
            {
                ViewBag.id = id;
                ViewBag.FormId = id;

                var rulesCalc = FormBuildHelpers.rulesCalc(form);
                sb.Append(rulesCalc.ToString());
                //rulesCalc();

                ViewBag.script = sb.ToString();
                ViewBag.captcha = form.Captcha;
                if(form.Captcha=="True") { ViewBag.captchaCode = System.Configuration.ConfigurationManager.AppSettings["RECAPTCHA_client"]; }
                ViewBag.Message = form.Title;
                ViewBag.Save = form.Save;
                ViewBag.maxUpload = CheckMembershipStatus.userUploadPerForm;
                ViewBag.Theme = formUrl + "/Content/css/themes/" + form.Theme + "/bootstrap.min.css";
                ViewBag.BackgroundImage = form.BackgroundImage;
                ViewBag.FormWidth = form.FormWidth+"px";

                ViewBag.ThemeTitle = form.Theme;
                if (form.LabelAlign == "Top")
                { 
                    ViewBag.LabelAlign = "";
                    ViewBag.Row = "row";
                    ViewBag.offset = "";
                }
                else { 
                    ViewBag.LabelAlign = "form-horizontal";
                    ViewBag.Row = "";
                    ViewBag.offset = "col-md-offset-"+form.LabelWidth;
                }

                var gridMax = 12;
                var inputCols = gridMax - Convert.ToInt16(form.LabelWidth);

                
                ViewBag.inputAlign = inputCols;

                ViewBag.labelWidth = "col-md-12";
                ViewBag.inputWidth = "col-md-12";
                if (form.LabelAlign == "Left")
                {
                    ViewBag.labelWidth = "col-md-" + form.LabelWidth.ToString();
                    ViewBag.inputWidth = "col-md-" + inputCols.ToString();
                }
                //form.

                var userAgent = Request.Headers["User-Agent"];
                string uaString = Convert.ToString(userAgent[0]);
                var uaParser = Parser.GetDefault();
                ClientInfo c = uaParser.Parse(uaString);

                Guid logId = FormBuildHelpers.addToLogGeneral(id,c);
                ViewBag.logid = logId.ToString();

                //check if upload field is present
                ViewBag.hasFileUpload = false;
                if(form.FieldItems.Where(x => x.ElementType == "file").Count() > 0){
                    ViewBag.hasFileUpload = true;
                }

                
                return View(form);
            }

            return View();

        }

        private Boolean isMultiSelectField(string itemType)
        {
            if (itemType == "select" || itemType == "checkbox" || itemType == "radio") return true;

            return false;
        }


        void UploadFile(string bucketName, string filepath, string formID, int submissionID, string fileName, string inputID, string LabelName)
        {
            var etag="";
            try
            {
                AmazonS3Config config = new AmazonS3Config();
                //config.ServiceURL = "http://s3-us-west-2.amazonaws.com";
                config.ServiceURL = "http://s3.amazonaws.com";
               
                IAmazonS3 s3Client = new AmazonS3Client("AKIAITX2FLX7WTSHQYHA", "tFoaCLDlToxtfwui/L6EE6lyZPe/O5KNzgWSNEfu", config);


                //var s3Client = new AmazonS3Client();
                var request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    FilePath = filepath,
                    CannedACL = S3CannedACL.Private,
                    Key = formID + "/" + submissionID + "/" + fileName
                };
                var response = s3Client.PutObjectAsync(request);
                etag = response.Result.ETag;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                   // Log.Add(LogTypes.Debug, mediaId, "Please check the provided AWS Credentials.");
                }
                else
                {
                   // Log.Add(LogTypes.Debug, mediaId, String.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
                }
                //return String.Empty; //Failed
            }


            SubmitValues sv = new SubmitValues();

            sv.Value = fileName + "#" + etag;
            sv.FieldId = Convert.ToInt16(inputID);
            sv.SubmissionId = submissionID;
            sv.LabelName = LabelName;

            formsDB.SubmitValues.Add(sv);

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult HandleForm(IDictionary<string, string> SubmitFields, ngFormey.Web.Models.frmy01_DevContext model, IFormCollection formCollection, FormLists FL) //, HttpPostedFileBase uploadFile , IEnumerable<HttpPostedFileBase> files
        {
            // REDIRECT TO SAVE IF SAVED USED
            foreach (var _key in formCollection)
            {
                //var _value = formCollection[_key.Key.ToString()];
                if (_key.Key.ToString() == "SaveFF")
                {
                    SaveForm(SubmitFields, model, formCollection, FL);
                    return RedirectToRoute("form-confirmation", new
                    {
                        id = "2",//FS.SubmissionId,
                        msg = "Form was saved"//form.ConfirmationMessage
                    });
                }
                //var _value = formCollection[_key.Key.ToString()];
                if (_key.Key.ToString() == "UpdateFF")
                {
                    UpdateForm(SubmitFields, model, formCollection, FL);
                    return RedirectToRoute("form-confirmation", new
                    {
                        id = "2",//FS.SubmissionId,
                        msg = "Form was saved"//form.ConfirmationMessage
                    });
                }
            }



            FormSubmissions FS = new FormSubmissions();

            FS.DateAdded = DateTime.UtcNow; //get UTC date than convert to local time for user

            int paypalSelectedId = 0;
            string orderID = "";
            var formID = "";
            var logGeneralID = "";
            var SavedSubmissionId = "";
            var RefCountry = "";
            var RefCity = "";
            var RefIP = "";
            StringBuilder searchField = new StringBuilder();


            // Check for Salesforce Fields and add to list/dictionary
            List<int> list = new List<int>();
            Dictionary<int, string> dmap = new Dictionary<int, string>();
            foreach (var _key in formCollection)
            {
                //var _value = formCollection[_key.Value.ToString()];
                var _value = _key.Value;
                if (_key.Key.ToString().StartsWith("input"))
                {
                    var _value1 = _key.Value;
                    var _key1 = int.Parse(_key.Key.ToString().Split("_")[2]);
                    list.Add(_key1);
                    dmap.Add(_key1, _value1);
                }
            }

            // Get distinct elements and convert into a list again.
            List<int> distinct = list.Distinct().ToList();


            //Create SF mapping object list
            List<string> sfMappingList = new List<string>();
            string formListId =null;

            foreach (var itemId in distinct)
            {
                var sfObjectId = formsDB.FieldItems
                    .Where(t => t.FieldItemId == itemId).FirstOrDefault();
                if(sfObjectId != null  && sfObjectId.SfMappingObject != null && sfObjectId.SfMappingField != null)
                {
                    //Find all the required fields on the object
                    formListId = sfObjectId.FormListId.ToString();
                   
                    var dataRow = formsDB.SfFields
                    .Where(c => c.SfobjectId.ToString() == sfObjectId.SfMappingObject)
                    .Where(c => c.SffieldId.ToString() == sfObjectId.SfMappingField).First();

                    if(dmap.ContainsKey(itemId))
                    {
                        var val = dmap[itemId];
                        buildSobject(dataRow.SfobjectName, dataRow.Name, val);
                    }
                }
                            
            }
             
            SfCredential sfc = getSFCredentils(formListId);
            if (dict.Count > 0 && formListId != null)
            {
                //ConnectSalesforce(dict, sfc);
                Task.Run(() => ConnectSalesforce(dict, sfc)).Wait();

            }

            // Get ID and FORM NAME
           // var formListId = "";
            foreach (var _key in formCollection)
            {
                //var _value = formCollection[_key.Value.ToString()];
                var _value = _key.Value;
                if (_key.Key.ToString() == "FormId")
                {
                    //FS.FormId = Convert.ToInt16(_value); // ADD FORMID TO FORM SUBMISSIONS
                    FS.FormId = new Guid(_value);
                    FS.FormListId = new Guid(_value);
                    formID = FS.FormId.ToString();

                }
                if (_key.Key.ToString() == "FormName")
                {
                    FS.FormName = _value; // ADD FORMID TO FORM SUBMISSIONS
                }
               if (_key.Key.ToString() == "FormListId")
                {
                    formListId = _value; // ADD FORMID TO FORM SUBMISSIONS
                }
                if (_key.Key.ToString() == "logid")
                {
                    logGeneralID = _value; // ADD FORMID TO FORM SUBMISSIONS
                }

                if (_key.Key.ToString() == "SavedSubmissionId")
                {
                    SavedSubmissionId = _value; // ADD FORMID TO FORM SUBMISSIONS
                }
                
                if (_key.Key.ToString() == "Country")
                {
                    RefCountry = _value; // ADD FORMID TO FORM SUBMISSIONS
                }

                if (_key.Key.ToString() == "City")
                {
                    RefCity = _value; // ADD FORMID TO FORM SUBMISSIONS
                }

                if (_key.Key.ToString() == "IPAddress")
                {
                    RefIP = _value; // ADD FORMID TO FORM SUBMISSIONS
                }

            }

            //get total upload size of all files in BYTES
            var totalFileSize = 0;
            foreach (var file in Request.Form.Files)
            {
                // IFormFile hpf = Request.Form.Files[file.;
                totalFileSize += file.ContentType.Length;
            }
            FS.TotalFileSize = totalFileSize;

            //GET USER MEMBERSHIP DETAILS

            var form = formsDB.FormLists.Find(new Guid(formListId)); // GET FORM 

            //SOURCE //http://stackoverflow.com/questions/27764692/validating-recaptcha-2-no-captcha-recaptcha-in-asp-nets-server-side
            if (form.Captcha == "True") { // check if we need to check captcha
                string EncodedResponse = Request.Form["g-Recaptcha-Response"];
                bool IsCaptchaValid = (ReCaptchaClass.Validate(EncodedResponse) == "True" ? true : false);

                if (!IsCaptchaValid)//check if not valid otherwise continue
                {
                    //Valid Request
                }
            }

            var userId = form.UserId;
            var userSubscription = formsDB.Subscriptions.FirstOrDefault(c => c.ApplicationUserName == userId);
            var userSubscriptionPlanId = userSubscription.SubscriptionPlanId;
            var userSubscriptionPlan = formsDB.SubscriptionPlans.FirstOrDefault(c => c.Id == userSubscriptionPlanId);
            var userSubscriptionPlanName = userSubscriptionPlan.Name;

            var membershipType = formsDB.MembershipTypes.FirstOrDefault(x => x.Name.Equals(userSubscriptionPlanName));

            var userUploadPerForm = membershipType.AllowedUploadsPerForm;


            if (totalFileSize > (userUploadPerForm * 1024 * 1024))
            {

                ModelState.AddModelError("CustomError", "The size of the file should not exceed " + userUploadPerForm + " MB. <a class='bottomNav' onclick='history.go(-1); return false;' href='#'>Back</a><br />");
                //return View(model);
                //return View("d", FL);
                //return RedirectToAction(,"d", "F", FL);
                return RedirectToRoute("form-confirmation", new
                {
                    id = FS.SubmissionId,
                    msg = form.ConfirmationMessage
                });

            }

            ////////////////////////////////////
            // GET LAST FRIENDLYID (based on all user forms) and INCREMENT
            ////////////////////////////////////

            var lastSubmission = formsDB.FormSubmissions
                    .Where(t => t.FormList.UserId == userId)
                    .OrderByDescending(t => t.ClientFriendlyId).Take(1).ToList();
            if (lastSubmission.Count != 0) // check if not first submission for account
                FS.ClientFriendlyId = lastSubmission[0].ClientFriendlyId + 1;
            else
                FS.ClientFriendlyId = 1;

            formsDB.FormSubmissions.Add(FS);
            // ADD TO LOG GENERAL TIME FORM WAS SUBMITTED MAKES IT EASY TO PULL THIS OUT FOR INSIGHTS
            if (logGeneralID != null)
            {
                var log = formsDB.LogGenerals.Single(p => p.LogGeneralId == new Guid(logGeneralID));

                //var RefCountry = "";
                //var RefCity = "";
                //var RefIP = "";

                log.Country = RefCountry;
                log.City = RefCity;
     
                log.Ipaddress = RefIP;
                log.DateTimeSubmitted = DateTime.UtcNow;
                //   formsDB.LogGenerals.Add(LG);
            }
            formsDB.SaveChanges();

            var submissionID = FS.SubmissionId;

            // Start FILE UPLOAD
            foreach (var file in Request.Form.Files)
            {
                //string etag = "";

               // IFormFile hpf = Request.Files[file] as HttpPostedFileBase;

                if (file.ContentType.Length == 0)
                    continue;
                string savedFileName = Path.Combine(
                   AppDomain.CurrentDomain.BaseDirectory + "\\_S3FileUpload",
                   Path.GetFileName(file.FileName));
               // hpf.SaveAs(savedFileName);
                

                AmazonS3Config config = new AmazonS3Config();
                //config.ServiceURL = "http://s3-us-west-2.amazonaws.com";
                config.ServiceURL = "http://s3.amazonaws.com";
                IAmazonS3 s3Client = new AmazonS3Client("AKIAITX2FLX7WTSHQYHA", "tFoaCLDlToxtfwui/L6EE6lyZPe/O5KNzgWSNEfu", config);
               // Amazon.S3.IAmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client("AKIAITX2FLX7WTSHQYHA", "tFoaCLDlToxtfwui/L6EE6lyZPe/O5KNzgWSNEfu", config);

                try
                {

                    //MAKING THIS UPLOAD ASYNC
                    string[] arr_id = file.ToString().Split("_");
                    string inputid;
                    string inputType;
                    inputid = arr_id[2];
                    inputType = arr_id[1];
                    
                    var LabelName = HttpUtility.UrlDecode(formCollection["label_" + inputid + "_" + inputType]);

                    Task.Run(()=> UploadFile("formey", savedFileName, formID, submissionID, file.FileName, inputid, LabelName)).Wait();
                }
                catch (AmazonS3Exception e)
                {
                    Response.WriteAsync(e.Message);
                }
                catch (System.IO.FileNotFoundException e)
                {
                    Response.WriteAsync(file.FileName + ": " + e.Message);
                }

                //Deleting Locally Saved File
                if (System.IO.File.Exists(savedFileName))
                {
                    System.IO.File.Delete(savedFileName);
                }
            }
            // finish file upload

            // START - CRM VALUES

            var formList = formsDB.FormLists.Find(new Guid(formListId));

           // var formList = formsDB.FieldItems.f

            Contacts contact = new Contacts();
            bool hasCRMFields = false;
            foreach (var _key in formCollection)
            {
                string[] arr_id = _key.Key.ToString().Split("_");

                if (arr_id.Length > 4)
                { // if more than 4 fields than its a CRM field
                    var _value = formCollection[_key.Key.ToString()];
                    var proprtyName = arr_id[3] + "_" + arr_id[4];
                    //contact.GetType().GetProperty(proprtyName).SetValue(contact, _value, null);
                    hasCRMFields = true;
                }
            }

            if (hasCRMFields)
            {
                contact.ContactId = Guid.NewGuid();
                contact.CrmDateAdded = DateTime.UtcNow;
                contact.CrmOpportunity = "Initial Contact";
                contact.RelatedToSubmissionId = submissionID.ToString();
                contact.UserId = formList.UserId;// "Web"; //needs to be formIDs contactID
                formsDB.Contacts.Add(contact);

                try
                {
                    formsDB.SaveChanges();
                }
                
               /* catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }*/
                catch(InvalidOperationException dbEx)
                {
                    throw new Exception(dbEx.Message);
                }
                var globalSettingsC = formsDB.GlobalSettings.FirstOrDefault(x => x.ApplicationUserName == userId);
                globalSettingsC.ContactsUnreadCount = globalSettingsC.ContactsUnreadCount + 1;

            }
            // END - CRM VALUES

            // START - SUBMIT VALUES
            foreach (var _key in formCollection)
            {
                string[] arr_id = _key.Key.ToString().Split("_");
                string inputid;
                string inputType;
                if (arr_id[0] == "input")
                {
                    SubmitValues sv = new SubmitValues();

                    var _value = formCollection[_key.Key.ToString()];

                    if (arr_id[1] == "paypal")
                    {
                        orderID = Guid.NewGuid().ToString().Substring(0, 8);
                        SubmitProductOrders SPO = new SubmitProductOrders();
                        //SPO.PaidDate = DateTime.Now.ToString();
                        SPO.SubmissionId = FS.SubmissionId;
                        SPO.PayPalFieldId = Convert.ToInt16(arr_id[2]);
                        SPO.SubmitValuesId = Convert.ToInt16(_value);
                        SPO.OrderNo = orderID;
                        formsDB.SubmitProductOrders.Add(SPO);

                        paypalSelectedId = Convert.ToInt16(SPO.SubmitValuesId);

                    }
                    else
                    {
                        inputid = arr_id[2];
                        inputType = arr_id[1];
                        sv.Value = _value;
                        searchField.Append(_value).Append("|");
                        sv.FieldId = Convert.ToInt16(inputid);
                        sv.SubmissionId = FS.SubmissionId;
                        sv.LabelName = HttpUtility.UrlDecode(formCollection["label_" + inputid + "_" + inputType]);

                        formsDB.SubmitValues.Add(sv);
                    }
                }
            }
            //save search field in form submission
            var formSubmission = formsDB.FormSubmissions.Single(p => p.SubmissionId == FS.SubmissionId);
            formSubmission.SearchField = searchField.ToString();
            if(contact.ContactId != Guid.Empty)
                formSubmission.RelatedToContactId = contact.ContactId.ToString();

            formsDB.SaveChanges();
            // END - SUBMIT VALUES


            // START- PAYPAL REDIRECT
            //var form = formsDB.FormLists.Find(FS.FormId); // GET FORM 
            if (form.PayPalAccounts.Count != 0)
            {
                var payPalObj = form.PayPalAccounts.First();
                return RedirectToRoute("Form-Redirect", new
                {
                    id = FS.FormId,
                    selectedProductId = paypalSelectedId,
                    productOrderId = orderID
                });
            }
            // END- PAYPAL REDIRECT

            // START - SEND TO SUBMITTER (when an NotificationEmailFieldSubmitter is selected)
            //send to a designated email address
            //send to an entered email address by form submitter

            if (form.NotificationEmailFieldSubmitter > 0)
            {

                var submitterEmailTo = "";
                //get email field from form
                foreach (var _key in formCollection)
                {
                    string[] arr_id = _key.Key.ToString().Split("_");

                    if (arr_id[0] == "input")
                    {

                        if (arr_id[2] == form.NotificationEmailFieldSubmitter.ToString())
                        {
                            submitterEmailTo = formCollection[_key.Key.ToString()];
                        }
                    }
                }

                var emailTemplate = formsDB.EmailTemplates.Find(form.NotificationEmailFieldSubmitterTemplate); // GET EMAIL TEMPLATE

                //Find all dynamic template form fields
                //string[] templateFields = Regex.Split(emailTemplate.Content.ToString(), @"\[\[(.*?)\]\]");
                if (emailTemplate != null)
                { //confirm that template content exists
                    if (emailTemplate.Content != null)
                    {
                        MatchCollection templateFields = Regex.Matches(emailTemplate.Content.ToString(), @"\[\[(.*?)\]\]");
                        var templateFieldsList = templateFields.Cast<Match>().Select(match => match.Value).ToList();
                        //Look thru each match and find field in form.
                        foreach (Match s in templateFields)
                        {
                            foreach (var _key in formCollection)
                            {
                                string[] arr_id = _key.Key.ToString().Split("_");
                                if (arr_id[0] == "label")
                                {
                                    string templateField = s.ToString();
                                    
                                    var _hiddenFieldName = HttpUtility.UrlDecode(formCollection[_key.Key.ToString()]);
                                    var _HiddenFieldNameNoSpaces = _hiddenFieldName.Replace(" ", "");

                                    if (s.ToString().IndexOf(_hiddenFieldName) != -1) // see if template field Name matches form hidden label field name
                                    {
                                        var hiddenLabelFieldType = arr_id[2]; //get id of field
                                        var hiddenLabelFieldId = arr_id[1]; //get id of field
                                        var fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId + "_" + _HiddenFieldNameNoSpaces]); // get value of field
                                        if (fieldValue == null) //try getting it as a CRM field
                                            fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId + "_CRM_" + _HiddenFieldNameNoSpaces]); // get value of field
                                        emailTemplate.Content = emailTemplate.Content.Replace(templateField, fieldValue);
                                    }
                                }
                            }

                        }

                        if (emailTemplate.Subject != null) // check if any dynamic fields dropped into subject
                        {
                            MatchCollection templateSubjectFields = Regex.Matches(emailTemplate.Subject.ToString(), @"\[\[(.*?)\]\]");
                            var templateSubjectFieldsList = templateSubjectFields.Cast<Match>().Select(match => match.Value).ToList();
                            //Look thru each match and find field in form.
                            foreach (Match s in templateSubjectFields)
                            {
                                foreach (var _key in formCollection)
                                {
                                    string[] arr_id = _key.Key.ToString().Split("_");
                                    if (arr_id[0] == "label")
                                    {
                                        string templateField = s.ToString();
                                        var _hiddenFieldName = HttpUtility.UrlDecode(formCollection[_key.Key.ToString()]);
                                        var _HiddenFieldNameNoSpaces = _hiddenFieldName.Replace(" ", "");

                                        if (s.ToString().IndexOf(_hiddenFieldName) != -1) // see if template field Name matches form hidden label field name
                                        {
                                            var hiddenLabelFieldType = arr_id[2]; //get id of field
                                            var hiddenLabelFieldId = arr_id[1]; //get id of field
                                            var fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId + "_" + _HiddenFieldNameNoSpaces]); // get value of field
                                            if (fieldValue == null) //try getting it as a CRM field
                                                fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId + "_CRM_" + _HiddenFieldNameNoSpaces]); // get value of field
                                            emailTemplate.Subject = emailTemplate.Subject.Replace(templateField, fieldValue);
                                        }
                                    }
                                }

                            }
                        }

                        if (submitterEmailTo != "")
                        {
                            using (SmtpClient smtpServer = new SmtpClient())
                            {
                                MailMessage mail = new MailMessage();
                                //smtpServer.Credentials = new System.Net.NetworkCredential("userName", "password");
                                //smtpServer.Port = 25; // Gmail works on this port
                                //var mailTo = form.NotificationEmail;

                                mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FROM_EMAIL_ADDRESS"]);
                                mail.To.Add(submitterEmailTo);
                                mail.Subject = emailTemplate.Subject;
                                mail.Body = emailTemplate.Content;
                                mail.IsBodyHtml = true;


                                if (System.Configuration.ConfigurationManager.AppSettings["isLiveServer"] == "true")
                                {
                                    smtpServer.Send(mail); //TOLIVE
                                }
                            }
                        }
                    }
                }
                else
                {
                    EmailTemplates ET = new EmailTemplates();
                    ET.Subject = "You have submitted a form";
                    ET.Content = "This is to confirm that we have received your new submission.";

                    var mailTo = submitterEmailTo;

                    MailMessage mail = new MailMessage();

                    using (SmtpClient smtpServer = new SmtpClient())
                    { 

                        mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FROM_EMAIL_ADDRESS"]);
                        mail.To.Add(mailTo);
                        mail.Subject = ET.Subject;
                        mail.Body = ET.Content;
                        mail.IsBodyHtml = true;

                        if (System.Configuration.ConfigurationManager.AppSettings["isLiveServer"] == "true")
                        {
                            smtpServer.Send(mail); //TOLIVE
                        }
                    }
                }
            }
            // FINISH SEND NOTIFICATION


            
            // START - SEND EMAIL TO NotifcationEmail / OWNER
            if (form.NotificationEmail != null)
            {

                var emailTemplate = formsDB.EmailTemplates.Find(form.NotificationEmailTemplate); // GET EMAIL TEMPLATE

                if (emailTemplate != null)
                { //confirm that template content exists
                    if (emailTemplate.Content != null)
                    {
                        MatchCollection templateFields = Regex.Matches(emailTemplate.Content.ToString(), @"\[\[(.*?)\]\]");
                        var templateFieldsList = templateFields.Cast<Match>().Select(match => match.Value).ToList();
                        //Look thru each match and find field in form.
                        foreach (Match s in templateFields)
                        {
                            foreach (var _key in formCollection)
                            {
                                string[] arr_id = _key.Key.ToString().Split("_");
                                if (arr_id[0] == "label")
                                {
                                    string templateField = s.ToString();
                                    var _hiddenFieldName = HttpUtility.UrlDecode(formCollection[_key.Key.ToString()]);

                                    if (s.ToString().IndexOf(_hiddenFieldName) != -1) // see if template field Name matches form hidden label field name
                                    {
                                        var hiddenLabelFieldType = arr_id[2]; //get id of field
                                        var hiddenLabelFieldId = arr_id[1]; //get id of field
                                        var fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId]); // get value of field
                                        emailTemplate.Content = emailTemplate.Content.Replace(templateField, fieldValue);
                                    }
                                }
                            }
                        }

                        if (emailTemplate.Subject != null) // check if any dynamic fields dropped into subject
                        {
                            MatchCollection templateSubjectFields = Regex.Matches(emailTemplate.Subject.ToString(), @"\[\[(.*?)\]\]");
                            var templateSubjectFieldsList = templateSubjectFields.Cast<Match>().Select(match => match.Value).ToList();
                            //Look thru each match and find field in form.
                            foreach (Match s in templateSubjectFields)
                            {
                                foreach (var _key in formCollection)
                                {
                                    string[] arr_id = _key.Key.ToString().Split("_");
                                    if (arr_id[0] == "label")
                                    {
                                        string templateField = s.ToString();
                                        var _hiddenFieldName = HttpUtility.UrlDecode(formCollection[_key.Key.ToString()]);
                                        var _HiddenFieldNameNoSpaces = _hiddenFieldName.Replace(" ", "");

                                        if (s.ToString().IndexOf(_hiddenFieldName) != -1) // see if template field Name matches form hidden label field name
                                        {
                                            var hiddenLabelFieldType = arr_id[2]; //get id of field
                                            var hiddenLabelFieldId = arr_id[1]; //get id of field
                                            var fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId + "_" + _HiddenFieldNameNoSpaces]); // get value of field
                                            if (fieldValue == null) //try getting it as a CRM field
                                                fieldValue = HttpUtility.UrlDecode(formCollection["input_" + hiddenLabelFieldType + "_" + hiddenLabelFieldId + "_CRM_" + _HiddenFieldNameNoSpaces]); // get value of field
                                            emailTemplate.Subject = emailTemplate.Content.Replace(templateField, fieldValue);
                                        }
                                    }
                                }

                            }
                        }

                        var mailTo = form.NotificationEmail;

                        MailMessage mail = new MailMessage();

                        using (SmtpClient smtpServer = new SmtpClient())
                        {
                            mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FROM_EMAIL_ADDRESS"]);
                            mail.To.Add(mailTo);
                            mail.Subject = emailTemplate.Subject;
                            mail.Body = emailTemplate.Content;
                            mail.IsBodyHtml = true;

                            if (System.Configuration.ConfigurationManager.AppSettings["isLiveServer"] == "true")
                            {
                                smtpServer.Send(mail); //TOLIVE
                            }
                        }
                    }
                    else //no template created, below is a default that is sent out.
                    {
                        var mailTo = form.NotificationEmail;
                        var landingUrl = System.Configuration.ConfigurationManager.AppSettings["LANDING_URL"];
                        EmailTemplates ET = new EmailTemplates();
                        ET.Subject = "New form submission from " + mailTo + " via [Formey]";
                        ET.Content = "New submission from: " + mailTo + ". Go to <a href=" + landingUrl + ">" + landingUrl + "</a> to check your new submissions.";

                        MailMessage mail = new MailMessage();

                        using (SmtpClient smtpServer = new SmtpClient())
                        {
                            mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FROM_EMAIL_ADDRESS"]);
                            mail.To.Add(mailTo);
                            mail.Subject = ET.Subject;
                            mail.Body = ET.Content;
                            mail.IsBodyHtml = true;

                            if (System.Configuration.ConfigurationManager.AppSettings["isLiveServer"] == "true")
                            {
                                smtpServer.Send(mail); //TOLIVE
                            }
                        }
                    }

                }
            }
            // FINSH - SEND EMAIL TO NotifcationEmail



            /////////////////////////////////////////////////////////////////
            // START - UPDATE SubscriptionUsage Table with submission details, 
            // this is used later to check if subscriber has not exceeded 
            // subscription plan
            /////////////////////////////////////////////////////////////////
            // if current date is in range highest start/end then add submission/uploads
            DateTime DateNow = DateTime.UtcNow;//.Today;
            DateTime rStartDate = DateTime.Today;
            DateTime rEndDate = DateTime.Today;
            DateTime? userCurrentSubscriptionPaidStartDate;
            DateTime? userCurrentSubscriptionPaidEnd;

            var usageStartEnd = formsDB.SubscriptionUsages
                .Where(c => c.ApplicationUserName == userId)
                .Where(c => c.PeriodStart < DateNow)
                .Where(c => c.PeriodEnd > DateNow).FirstOrDefault();

            if (usageStartEnd == null) // new period, does not exist in subscription usage for current period
            {
                var currentDayOfMonth = DateNow.Day;


                if(userSubscription.PaidStart!=null&& userSubscription.PaidEnd != null) // subscription has entry for paidStart paidEnd
                {
                    userCurrentSubscriptionPaidStartDate = userSubscription.PaidStart;//.TrialEnd; //this gets us a starting date 
                    userCurrentSubscriptionPaidEnd = userSubscription.PaidEnd;
                }
                else //paid start/end not yet added to subscription or FREE account
                { 
                    userCurrentSubscriptionPaidStartDate = userSubscription.TrialEnd;// .PaidStart;//.TrialEnd; //this gets us a starting date 
                    userCurrentSubscriptionPaidEnd = DateTime.Parse(userSubscription.TrialEnd.ToString()).AddMonths(1);
                }

                var monthDiffToNow = GetMonthsBetween(DateTime.Parse(userCurrentSubscriptionPaidStartDate.ToString()), DateNow);

                if (monthDiffToNow > 0) // if not the month that user joined add subscription usage
                {
                    rStartDate = DateTime.Parse(userCurrentSubscriptionPaidStartDate.ToString()).AddMonths(monthDiffToNow);
                    rEndDate = DateTime.Parse(userCurrentSubscriptionPaidEnd.ToString()).AddMonths(monthDiffToNow);
                }
                else { //Use current date time to insert subscription usage, must be first time
                    if (userCurrentSubscriptionPaidStartDate!=null){
                        rStartDate = DateTime.Parse(userCurrentSubscriptionPaidStartDate.ToString());
                        //var subscribeMonth = rStartDate.Month;
                        //var subscribeDayOfMonth = rStartDate.Day;
                    }
                    if (userCurrentSubscriptionPaidEnd != null) { 
                        rEndDate = DateTime.Parse(userCurrentSubscriptionPaidEnd.ToString());
                    }
                }

                //GET ALLOWANCES
                var userSubmissionAllowance = membershipType.AllowedSubmissions;
                var userUploadAllowance = membershipType.AllowedUploads;

                //GET TOTAL CURRENT USERS FOR ACCOUNT
                var userCount = 0;
                var userInfo = formsDB.UserGroups.Where(x => x.UserName == userId).FirstOrDefault();
                if (userInfo != null)
                {
                    userCount = formsDB.UserGroups.Where(x => x.GroupId == userInfo.GroupId).Where(x => x.Deleted == false).Count();
                }

                SubscriptionUsages SU = new SubscriptionUsages();

                // get start of current period
                // get end of current period
                SU.ApplicationUserName = userId;
                SU.PeriodStart = rStartDate; // userCurrentSubscriptionPaidStartDate; // rStartDate;
                SU.PeriodEnd = rEndDate; // userSubscription.PaidEnd; //rEndDate;
                SU.Submissions = 1;
                SU.Uploads = totalFileSize / 1000000;
                SU.AllowedSubmissions = userSubmissionAllowance;
                SU.AllowedUploads = userUploadAllowance;
                SU.AllowedUploadsPerForm = userUploadPerForm;
                SU.AllowedUsers = membershipType.AllowedUsers;
                SU.UsersTotal = userCount; //GET CURRENT NUMBER OF USERS

                formsDB.SubscriptionUsages.Add(SU);
            }
            else // existing period
            {
                usageStartEnd.Submissions += 1;
                usageStartEnd.Uploads += (totalFileSize / 1000000); // divide by 1,000,000 to get MB
            }

            //
            var globalSettingsS = formsDB.GlobalSettings.FirstOrDefault(x => x.ApplicationUserName == userId);
            if (globalSettingsS != null)
                globalSettingsS.SubmitsUnreadCount = globalSettingsS.SubmitsUnreadCount + 1;


            /////////// IF SUBMITTING SAVED REMOVE IT /////////////////////
            if (SavedSubmissionId != "") // remove saved form and field values
            {
                var deleteFormSubmission = formsDB.SavedFormSubmissions.FirstOrDefault(
                                                x => x.SavedSubmissionId == new Guid(SavedSubmissionId)
                                            );

                if (deleteFormSubmission != null)
                { //check that there is a match
                    var deleteFormSubmitValues =
                        from details in formsDB.SavedSubmitValues
                        where details.SubmissionId == deleteFormSubmission.SubmissionId
                        select details;

                    foreach (var detail in deleteFormSubmitValues)
                    {
                        formsDB.SavedSubmitValues.Remove(detail);
                    }
                    formsDB.SavedFormSubmissions.Remove(deleteFormSubmission);
                }
            }
            ////////////////////////////////


            formsDB.SaveChanges();
            // if current date is not in range of highest start/end than add new start/end

            /////////////////////////////////////////////////////////////////
            // END - UPDATE SubscriptionUsage Table with submission details
            /////////////////////////////////////////////////////////////////
            //return RedirectToRoute("form-confirmation", new
            //{
            //    id = FS.SubmissionId,
            //    msg = HttpUtility.UrlEncode(form.ConfirmationMessage)
            //});
            
            return RedirectToRoute(new { controller = "f", action = "FormConfirmation", msg = HttpUtility.UrlEncode(form.ConfirmationMessage) });
        }

        public ActionResult FormRedirect(int id, int selectedProductId, string productOrderId)
        {
            var form = formsDB.FormLists.Find(id); // GET FORM 

            if (form != null)
            {
                if (form.PayPalAccounts != null)
                {
                    var product = formsDB.CartItemElements.Find(selectedProductId);

                    ViewBag.ItemName = product.ItemName;
                    ViewBag.Amount = product.Amount;
                    ViewBag.OrderId = productOrderId;

                    var payPalObj = form.PayPalAccounts.First();
                    return View(payPalObj);
                }
            }
            return RedirectToAction("Index", "Error");
        }

        public ActionResult FormConfirmation(int id, string msg)
        {
            if (msg == "") { msg = "Thanks, your submission has been sent"; };

            //ViewBag.Message = msg; // @MvcHtmlString.Create(HttpUtility.HtmlDecode(
            TempData["msg"] = HttpUtility.UrlDecode(msg);

            return View();
        }


        /// <summary>
        /// send a time stamp notification
        /// </summary>
        /// <param name="FormId"></param>
        /// <param name="LogId"></param>
        /// <value>test</value>
        [HttpPost]
        [AllowAnonymous]
        public void NotifyTime(string FormId, Guid? LogId)
        {
            if (!String.IsNullOrEmpty(FormId))
            {
                //add new form load to LogGeneral
                LogTimes LT = new LogTimes();
                LT.DateAdded = DateTime.UtcNow;
                LT.LogGeneralId = (Guid)LogId;
                //LT.LogTimeId = Guid.NewGuid();

                formsDB.LogTimes.Add(LT);
                formsDB.SaveChanges();
            }
            
        }

        /// <summary>
        /// send a form interaction record
        /// </summary>
        /// <param name="FormId"></param>
        /// <param name="LogId"></param>
        /// <value>test</value>
        /// TODO move this to LogGeneral, create InteractWithForm field as a date time to see when started to interact
        [HttpPost]
        [AllowAnonymous]
        public void Interact(string FormId, Guid? LogId)
        {
            if (!String.IsNullOrEmpty(FormId))
            {
                //add new interaction with form
                LogInteractWithForms LIWF = new LogInteractWithForms();
                LIWF.LogGeneralId = (Guid)LogId;

                formsDB.LogInteractWithForms.Add(LIWF);
                formsDB.SaveChanges();
            }

        }


        /// <summary>
        /// Send an email with details of a saved submission
        /// </summary>
        /// <param name="formCollection"></param>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendForm(FormCollection formCollection) //send email with link to saved form
        {
            //var test = "";
            
            var client = new HttpClient();

            // var emailAddress = formCollection.Get("email");
            //var savedSubmissionId = formCollection.Get("SavedSubmissionId");

            var emailAddress = "test@gmail.com";
            var savedSubmissionId = "11111111111111";

            client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["POSTAL_URL"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // HTTP GET
            HttpResponseMessage response = client.GetAsync("/api/values/EmailSavedSubmission?sId="
                + savedSubmissionId + "&e=" + emailAddress).Result; //+ model.Email);
            if (response.IsSuccessStatusCode)
            {
                // Product product = await response.Content.ReadAsAsync<Product>();
                // Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
            }
            
            


            //if (!String.IsNullOrEmpty(FormId))
            //{
            //    //add new form load to LogGeneral
            //    LogTime LT = new LogTime();
            //    LT.DateAdded = DateTime.UtcNow;
            //    LT.LogGeneralId = (Guid)LogId;
            //    //LT.LogTimeId = Guid.NewGuid();

            //    formsDB.LogTimes.Add(LT);
            //    formsDB.SaveChanges();
            //}
            return View();
        }

        [HttpPost]
        public string NotifyInteraction(string Name, string Address)
        {
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Address))
                //TODO: Save the data in database
                return "Thank you " + Name + ". Record Saved.";
            else
                return "Please complete the form.";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveForm(IDictionary<string, string> SubmitFields, ngFormey.Web.Models.frmy01_DevContext model, IFormCollection formCollection, FormLists FL) //, HttpPostedFileBase uploadFile
        {

            var formUrl = new Uri(System.Configuration.ConfigurationManager.AppSettings["FORM_URL"]);
            
            SavedFormSubmissions FS = new SavedFormSubmissions();

            FS.DateAdded = DateTime.UtcNow; //get UTC date than convert to local time for user
            FS.SavedSubmissionId = Guid.NewGuid();
            var formID = "";
            var logGeneralID = "";

            StringBuilder searchField = new StringBuilder();

            // Get ID and FORM NAME
            var formListId = "";
            foreach (var _key in formCollection)
            {
                var _value = formCollection[_key.Key.ToString()];
                if (_key.Key.ToString() == "Id")
                {
                    //FS.FormId = Convert.ToInt16(_value); // ADD FORMID TO FORM SUBMISSIONS
                    FS.FormId = new Guid(_value);
                    FS.FormListId = new Guid(_value);
                    formID = FS.FormId.ToString();

                }
                if (_key.Key.ToString() == "FormName")
                {
                    FS.FormName = _value; // ADD FORMID TO FORM SUBMISSIONS
                }
                if (_key.Key.ToString() == "FormListId")
                {
                    formListId = _value; // ADD FORMID TO FORM SUBMISSIONS
                }
                if (_key.Key.ToString() == "logid")
                {
                    logGeneralID = _value; // ADD FORMID TO FORM SUBMISSIONS
                }
            }

            //get total upload size of all files in BYTES
            var totalFileSize = 0;
            foreach (var file in Request.Form.Files)
            {
                // HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                totalFileSize += file.ContentType.Length;
            }
            FS.TotalFileSize = totalFileSize;

            //GET USER MEMBERSHIP DETAILS

            var form = formsDB.FormLists.Find(new Guid(formID)); // GET FORM 

            var userId = form.UserId;
            var userSubscription = formsDB.Subscriptions.FirstOrDefault(c => c.ApplicationUserName == userId);
            var userSubscriptionPlanId = userSubscription.SubscriptionPlanId;
            var userSubscriptionPlan = formsDB.SubscriptionPlans.FirstOrDefault(c => c.Id == userSubscriptionPlanId);
            var userSubscriptionPlanName = userSubscriptionPlan.Name;
            var membershipType = formsDB.MembershipTypes.FirstOrDefault(x => x.Name.Equals(userSubscriptionPlanName));
            var userUploadPerForm = membershipType.AllowedUploadsPerForm;

            if (totalFileSize > (userUploadPerForm * 1024 * 1024))
            {
                ModelState.AddModelError("CustomError", "The size of the upload file should not exceed " + userUploadPerForm + " MB. <a class='bottomNav' onclick='history.go(-1); return false;' href='#'>Back</a><br />");
                return View(model);
            }

            formsDB.SavedFormSubmissions.Add(FS);
            formsDB.SaveChanges();

            var submissionID = FS.SubmissionId; //NEW SAVED ID

            // START - SUBMIT VALUES
            foreach (var _key in formCollection)
            {
                string[] arr_id = _key.Key.ToString().Split("_");
                string inputid;
                string inputType;
                if (arr_id[0] == "input")
                {
                    SavedSubmitValues sv = new SavedSubmitValues();
                    var _value = formCollection[_key.Key.ToString()];
                    
                    inputid = arr_id[2];
                    inputType = arr_id[1];
                    sv.Value = _value;
                    searchField.Append(_value).Append("|");
                    sv.FieldId = Convert.ToInt16(inputid);
                    sv.SubmissionId = FS.SubmissionId;
                    sv.LabelName = HttpUtility.UrlDecode(formCollection["label_" + inputid + "_" + inputType]);

                    formsDB.SavedSubmitValues.Add(sv);
                }
            }

            formsDB.SaveChanges();

            TempData["SavedUrl"] = formUrl + "/f/s/" + FS.SavedSubmissionId;
            TempData["SavedSubmissionId"] = FS.SavedSubmissionId;
            TempData["msg"] = "Form saved.";
            // END - SUBMIT VALUES

            //return RedirectToRoute("form-confirmation", new
            //{
            //    id = FS.SubmissionId,
            //    confirmationMsg = form.ConfirmationMessage,
            //    isSaved = true
            //});
            //return RedirectToAction("MyIndex"); 
            TempData["isSaved"] = "true";
            return View("FormConfirmation");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateForm(IDictionary<string, string> SubmitFields, ngFormey.Web.Models.frmy01_DevContext model, IFormCollection formCollection, ngFormey.Web.Models.FormLists FL) //, HttpPostedFileBase uploadFile
        {

            var urlId = Request.Headers["Id"];
            Guid urlGuid = new Guid(urlId);

            var savedForm = formsDB.SavedFormSubmissions.FirstOrDefault(s => s.SavedSubmissionId == urlGuid); // GET FORM 
            var formId = savedForm.FormId;
            ViewBag.isSaved = true;

            var form = formsDB.FormLists.Find(formId); // GET FORM 

            var deleteSubmitValues = 
                    from details in formsDB.SavedSubmitValues
                    where details.SubmissionId == savedForm.SubmissionId
                    select details;

            foreach (var detail in deleteSubmitValues){
                formsDB.SavedSubmitValues.Remove(detail);
            }
            try{
                formsDB.SaveChanges();
            }
            catch (Exception e){
                Console.WriteLine(e);
            }



            StringBuilder searchField = new StringBuilder();


            //get total upload size of all files in BYTES
            var totalFileSize = 0;
            foreach (var file in Request.Form.Files)
            {
                //HttpPostedFileBase hpf = Request.Form.Files[file] as HttpPostedFileBase;
                totalFileSize += file.ContentType.Length;
            }
            savedForm.TotalFileSize = totalFileSize;

            formsDB.SaveChanges();


            // START - SUBMIT VALUES
            foreach (var _key in formCollection)
            {
                string[] arr_id = _key.Key.ToString().Split("_");
                string inputid;
                string inputType;
                if (arr_id[0] == "input")
                {
                    SavedSubmitValues sv = new SavedSubmitValues();
                    var _value = formCollection[_key.Key.ToString()];

                    inputid = arr_id[2];
                    inputType = arr_id[1];
                    sv.Value = _value;
                    searchField.Append(_value).Append("|");
                    sv.FieldId = Convert.ToInt16(inputid);
                    sv.SubmissionId = savedForm.SubmissionId;
                    sv.LabelName = HttpUtility.UrlDecode(formCollection["label_" + inputid + "_" + inputType]);

                    formsDB.SavedSubmitValues.Add(sv);
                }
            }

            formsDB.SaveChanges();
            // END - SUBMIT VALUES

            return RedirectToRoute("form-confirmation", new
            {
                id = savedForm.SubmissionId,
                msg = "Form Saved"
            });


        }

        /// <summary>
        /// display saved form reformat to reuse section in stand load and saved load
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult s(Guid id) // DISPLAY SAVED FORM
        {
            TempData["notFound"] = false;
            var savedForm = formsDB.SavedFormSubmissions.FirstOrDefault(s=>s.SavedSubmissionId == id); // GET FORM 
            if (savedForm == null) // check if there is a saved submission
            {
                TempData["msg"] = "Could not find saved form";
                TempData["notFound"] = true;
                return RedirectToRoute("form-confirmation", new
                {
                    id = "2",//FS.SubmissionId,
                    msg = "No Saved Submissions found" //form.ConfirmationMessage
                });
            }
            var formId = savedForm.FormId;
            ViewBag.isSaved = true;

            var form = formsDB.FormLists.Find(formId); // GET FORM 
            StringBuilder sb = new StringBuilder();
            ViewBag.SavedSubmissionId = savedForm.SavedSubmissionId;
            ViewBag.FormId = savedForm.FormId;
            ///////////CHECK MEMBERSHIP STATUS OF FORM OWNER//////////////
            bool quotaExceeded = false;
            ViewBag.quotaExceeded = quotaExceeded;
            DateTime DateNow = DateTime.UtcNow;//.Today;
            var currentDayOfMonth = DateNow.Day;
            var currentMonthOfYear = DateNow.Month;
            var returnSubmissionMonth = currentDayOfMonth;

            var userId = form.UserId;
            var userSubscriptionUsage = formsDB.SubscriptionUsages.OrderByDescending(x => x.PeriodStart).FirstOrDefault(c => c.ApplicationUserName == userId); // TODO get top 1 DESC
            int userUploadPerForm = 0;
            if (userSubscriptionUsage == null) //upload per form
            {
                var userSubscription = formsDB.Subscriptions.FirstOrDefault(c => c.ApplicationUserName == userId);
                var userSubscriptionPlanId = userSubscription.SubscriptionPlanId;
                var userSubscriptionPlan = formsDB.SubscriptionPlans.FirstOrDefault(c => c.Id == userSubscriptionPlanId);
                var userSubscriptionPlanName = userSubscriptionPlan.Name;
                var membershipType = formsDB.MembershipTypes.FirstOrDefault(x => x.Name.Equals(userSubscriptionPlanName));
                userUploadPerForm = membershipType.AllowedUploadsPerForm;
            }

            if (userSubscriptionUsage != null)
            {
                var allowedSubmissions = userSubscriptionUsage.AllowedSubmissions;
                var allowedUploads = userSubscriptionUsage.AllowedUploads;
                var allowedUploadsPerForm = userSubscriptionUsage.AllowedUploadsPerForm;
                var userPeriodSubmissions = userSubscriptionUsage.Submissions;
                var userPeriodUploads = userSubscriptionUsage.Uploads;
                userUploadPerForm = userSubscriptionUsage.AllowedUploadsPerForm;

                // Check Current User Subscription and Current Usage
                if (userPeriodSubmissions > allowedSubmissions)
                {
                    sb.Append("<h3>Subscription has exceeded submission quota.</h3>");
                    quotaExceeded = true;
                }

                if (userPeriodUploads > allowedUploads)
                {
                    sb.Append("<h3>Subscription has exceeded upload quota.</h3>");
                    quotaExceeded = true;
                }
                ViewBag.quotaExceeded = quotaExceeded;
                ViewBag.outHtml = sb.ToString();
            }
            ///////////FINISH CHECK MEMBERSHIP////////////////////////////
            //////////////////////////////////////////////////////////////

            if (!quotaExceeded)
            {
                StringBuilder buildActionField = new StringBuilder();
                ViewBag.id = id;

                var rulesCalc = FormBuildHelpers.rulesCalc(form);
                sb.Append(rulesCalc.ToString());

                ViewBag.script = sb.ToString();

                ViewBag.Message = form.Title;
                ViewBag.Save = form.Save;
                ViewBag.maxUpload = userUploadPerForm;
                ViewBag.Theme = "~/Content/css/themes/" + form.Theme + "/bootstrap.css";
                ViewBag.BackgroundImage = form.BackgroundImage;
                ViewBag.ThemeTitle = form.Theme;
                if (form.LabelAlign == "Top")
                {
                    ViewBag.LabelAlign = "";
                    ViewBag.Row = "row";
                    ViewBag.offset = "";
                }
                else
                {
                    ViewBag.LabelAlign = "form-horizontal";
                    ViewBag.Row = "";
                    ViewBag.offset = "col-md-offset-" + form.LabelWidth;
                }


                var gridMax = 12;
                var inputCols = gridMax - Convert.ToInt16(form.LabelWidth);

                //ViewBag.offset = form.LabelWidth;
                ViewBag.inputAlign = inputCols;

                ViewBag.labelWidth = "col-md-12";
                ViewBag.inputWidth = "col-md-12";
                if (form.LabelAlign == "Left")
                {
                    ViewBag.labelWidth = "col-md-" + form.LabelWidth.ToString();
                    ViewBag.inputWidth = "col-md-" + inputCols.ToString();
                }
                //form.
                var userAgent = Request.Headers["User-Agent"];
                string uaString = Convert.ToString(userAgent[0]);

                var uaParser = Parser.GetDefault();
                ClientInfo c = uaParser.Parse(uaString);

                Guid logId = FormBuildHelpers.addToLogGeneral(formId, c);
                ViewBag.logid = logId.ToString();
            }

        //    var savedValues = formsDB.SavedSubmitValues.Find(savedForm.SubmissionId); // GET FORM 

            var savedValues = (from m in formsDB.SavedSubmitValues
            where m.SubmissionId == savedForm.SubmissionId
            select m);


            //TODO is there a better way to match up?
            foreach (var fieldItem in form.FieldItems)
            {
                foreach (var savedItem in savedValues){
                    if (fieldItem.FieldItemId == savedItem.FieldId)
                        fieldItem.DefaultValue = savedItem.Value;
                }
            }

            return View("d",form);
        }



        [ResponseCache(Duration = 7200, VaryByQueryKeys = new string[] { "userId" })]
        public dynamic checkMembershipStatus(string userId) //was public static dynamic
        {
            //get user name
            //var formItemUser = formsDB.FormLists
            //    .Where(x => x.FormListId == formId)
            //    .Select(y => y.UserId).FirstOrDefault();

            dynamic q = new System.Dynamic.ExpandoObject();
            bool quotaExceeded = false;
            string message = "";
            ///////////CHECK MEMBERSHIP STATUS OF FORM OWNER//////////////

            // GET CURRENT PERIOD, IF IT DOESNT EXIST IT MEANS NO SUBMISSIONS HAVE OCCURED IN CURRENT PERIOD
            DateTime DateNow = DateTime.UtcNow;//.Today;
            var userSubscriptionUsage = formsDB.SubscriptionUsages
                .Where(c => c.ApplicationUserName == userId)
                .Where(c => c.PeriodStart < DateNow)
                .Where(c => c.PeriodEnd > DateNow).FirstOrDefault();

            var userSubscription = formsDB.Subscriptions.Include("SubscriptionPlan").FirstOrDefault(c => c.ApplicationUserName == userId);
            bool isFreeTier = false;
            if (userSubscription.SubscriptionPlanId == 5)
                isFreeTier = true;

            int userUploadPerForm = 0;
            if (userSubscriptionUsage == null) //upload per form
            {
               
                //var userSubscriptionPlanId = userSubscription.SubscriptionPlanId;
                //var userSubscriptionPlan = formsDB.SubscriptionPlans.FirstOrDefault(c => c.Id == userSubscriptionPlanId);
                //var userSubscriptionPlanName = userSubscriptionPlan.Name;
                var userSubscriptionPlanName = userSubscription.SubscriptionPlan.Name;
                var membershipType = formsDB.MembershipTypes.FirstOrDefault(x => x.Name.Equals(userSubscriptionPlanName));
                userUploadPerForm = membershipType.AllowedUploadsPerForm;
            }

            if (userSubscriptionUsage != null)
            {
                var allowedSubmissions = userSubscriptionUsage.AllowedSubmissions;
                var allowedUploads = userSubscriptionUsage.AllowedUploads;
                var allowedUploadsPerForm = userSubscriptionUsage.AllowedUploadsPerForm;
                var userPeriodSubmissions = userSubscriptionUsage.Submissions;
                var userPeriodUploads = userSubscriptionUsage.Uploads;
                userUploadPerForm = userSubscriptionUsage.AllowedUploadsPerForm;

                // Check Current User Subscription and Current Usage
                if (userPeriodSubmissions > allowedSubmissions)
                {
                    message = "<h3>Subscription has exceeded submission quota.</h3>";
                    quotaExceeded = true;
                }

                if (userPeriodUploads > allowedUploads)
                {
                    message = message + "<h3>Subscription has exceeded upload quota.</h3>";
                    quotaExceeded = true;
                }

            }
            ///////////FINISH CHECK MEMBERSHIP////////////////////////////
            //////////////////////////////////////////////////////////////
            q.freeTier = isFreeTier;
            q.message = message;
            q.quotaExceeded = quotaExceeded;
            q.userUploadPerForm = userUploadPerForm;

            return q;
        }


        //[OutputCache(Duration = 20, VaryByParam = "id")]
        [HttpGet]
        public ActionResult t(Guid id) // DISPLAY FORM
        {
            var formUrl = new Uri(System.Configuration.ConfigurationManager.AppSettings["FORM_URL"]);
            ViewBag.submitURL = "";// formUrl + "/f/HandleForm";
            ViewBag.formURL = "";// formUrl;

            //formsDB.Configuration.AutoDetectChangesEnabled = false;

            var form = formsDB.FormTemplates
                .Include("TemplateRules")
                .Include("TemplateCalculations")
                //.Include("FormTemplateField.FormTemplateFieldElements")
                .Where(x => x.TemplateId == id).FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            ViewBag.isSaved = false; // used to show hide update or save
            ViewBag.captcha = false;

            //var CheckMembershipStatus = checkMembershipStatus(form.UserId);
            //bool quotaExceeded = CheckMembershipStatus.quotaExceeded;
            //ViewBag.quotaExceeded = quotaExceeded;
            //ViewBag.outHtml = CheckMembershipStatus.message.ToString();

            //if (!quotaExceeded)
            //{
                ViewBag.id = id;
                ViewBag.FormId = id;
            //Convert.ChangeType(formss, FormList);
            //FormList emp = formss.Cast<FormList>();
            //FormList form = (form)object;
            //var test = formsDB.GetType();

              //  FormList form = (formss)object;
                var rulesCalc = FormBuildHelpers.templateRulesCalc(form);
                sb.Append(rulesCalc.ToString());
                //rulesCalc();

                ViewBag.script = sb.ToString();
                ViewBag.captcha = "false";// form.Captcha;
                if (ViewBag.captcha == "True") { ViewBag.captchaCode = System.Configuration.ConfigurationManager.AppSettings["RECAPTCHA_client"]; }
                ViewBag.Message = form.Title;
                ViewBag.Save = false;// form.Save;
                //ViewBag.maxUpload = CheckMembershipStatus.userUploadPerForm;
                ViewBag.Theme = formUrl + "/Content/css/themes/" + form.Theme + "/bootstrap.min.css";
                ViewBag.FormWidth = form.FormWidth + "px";
                ViewBag.BackgroundImage = form.BackgroundImage;
                ViewBag.ThemeTitle = form.Theme;
                if (form.LabelAlign == "Top")
                {
                    ViewBag.LabelAlign = "";
                    ViewBag.Row = "row";
                    ViewBag.offset = "";
                }
                else
                {
                    ViewBag.LabelAlign = "form-horizontal";
                    ViewBag.Row = "";
                    ViewBag.offset = "col-md-offset-" + form.LabelWidth;
                }


            var gridMax = 12;
                var inputCols = gridMax - Convert.ToInt16(form.LabelWidth);

                //ViewBag.offset = form.LabelWidth;
                ViewBag.inputAlign = inputCols;

                ViewBag.labelWidth = "col-md-12";
                ViewBag.inputWidth = "col-md-12";
                if (form.LabelAlign == "Left")
                {
                    if(form.LabelWidth!=null)
                        ViewBag.labelWidth = "col-md-" + form.LabelWidth.ToString();
                    ViewBag.inputWidth = "col-md-" + inputCols.ToString();
                }
                //form.

                //Guid logId = FormBuildHelpers.addToLogGeneral(id, Request.Browser);
                //ViewBag.logid = logId.ToString();

                //check if upload field is present
                ViewBag.hasFileUpload = false;
                //if (form.FormTemplateFields.Where(x => x.ElementType == "file").Count() > 0)
                //{
                //    ViewBag.hasFileUpload = true;
                //}


                return View(form);
            //}

           // return View();

        }

    }

}