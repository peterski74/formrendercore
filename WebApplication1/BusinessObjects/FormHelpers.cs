
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ngFormey.Web.Extensions;
using System.Globalization;
using Newtonsoft.Json;
using WebApplication1.Models;
using UAParser;

namespace ngFormey.Web.BusinessObjects
{
    public class FormBuildHelpers
    {

        public static string rulesCalc(FormLists form)
        //public static string rulesCalc(IEnumerable<dynamic> forms)
        {

            //FormList form = (forms)obj;
            WebApplication1.Models.frmy01_DevContext formsDB = new WebApplication1.Models.frmy01_DevContext();

            StringBuilder buildActionField = new StringBuilder();

            StringBuilder sb = new StringBuilder();
            /////////////////////////////
            // RULES ////////////////////
            /////////////////////////////
            // TODO - rules not being applied on load
            // DONE CONDITION
            //      - is equal
            //      - is not equal
            // DONE FIELD TYPES 
            //      - RADIO
            //      - CHECKBOX
            //      - SELECT

            foreach (var rule in form.Rules)
            {
                //sb.Clear();
                buildActionField.Clear();

                var conditionField = formsDB.FieldItems.Find(rule.ConditionField);
                if (conditionField != null && rule.ActionField != null)
                { //CHECK: Could have added rule but not added anything to the fields of a rule
                    var conditionFieldType = conditionField.ElementType;

                    string action = rule.Action;
                    //string multiSelectItemId = "";

                    ////////////////////////////
                    // BUILD Condition Field used later in the code
                    ////////////////////////////
                    string buildConditionField = "input_" + conditionFieldType + "_" + rule.ConditionField;

                   // if (conditionField.CRM_Mapping.Trim()!="" )
                   // {
                        //buildConditionField = buildConditionField + "_" + conditionField.CRM_Mapping;
                   // }
                    ////////////////////////////////////
                    // Get the fields to Action(show/hide), this can be multiple
                    //string[] arrActionFields;
                    ////////////////////////////////////
                    string[] actionFields = rule.ActionField.Split(',');
                    int i = 0;
                    foreach (string field in actionFields)
                    {
                        buildActionField.Append("#item_" + field);
                        if (actionFields.Length > 1 && i != actionFields.Length - 1) buildActionField.Append(",");
                        i++;
                    }

                    //hide action field/s 
                    string visA = ""; //set visibility item based on action
                    string visB = "";
                    if (action == "Show")
                    {
                        sb.Append("$('" + buildActionField + "').fadeOut('slow');");
                        visA = ".fadeIn('slow')";
                        visB = ".fadeOut('slow')";

                    }
                    //attach event listener to condition field if action HIDE
                    if (action == "Hide")
                    {
                        visA = ".fadeOut('slow')";
                        visB = ".fadeIn('slow')";
                    }

                    string conditionRule = "";
                    if (rule.Condition.IndexOf("Not") != -1 || rule.Condition.IndexOf("Empty") != -1)
                    {
                        conditionRule = "!";
                    }

                    var conditionItemBuild = "";

                    // EMPTY / FILLED LOGIC
                    if (rule.Condition.IndexOf("Empty") != -1 || rule.Condition.IndexOf("Filled") != -1)
                    {   // select single item no match value
                        if (conditionFieldType == "radio" || conditionFieldType == "checkbox")
                            conditionItemBuild = "$(\"input[name='" + buildConditionField + "']\")";
                        else //select
                        {
                            if (conditionFieldType == "select")
                                conditionItemBuild = "$(\"#" + buildConditionField + " :selected\").val()!='' ";
                            if (conditionFieldType == "textbox" || conditionFieldType == "email")
                                conditionItemBuild = "$(\"#" + buildConditionField + "\").val().length!=0";
                        }

                    }
                    else //check for specific item in multi item
                    {
                        if (conditionFieldType == "radio" || conditionFieldType == "checkbox")
                            conditionItemBuild = "$(\"input[name='" + buildConditionField + "'][value='" + rule.ConditionMatchValue + "']\")";
                        else //select
                            conditionItemBuild = "$(\"#" + buildConditionField + " option[value='" + rule.ConditionMatchValue + "']\").is(':selected')";
                    }

                    // CONTAINS VALUE / NOT CONTAIN VALUE
                    if (rule.Condition.IndexOf("Contain") != -1)
                    {
                        conditionItemBuild = "$(\"#" + buildConditionField + "\").val().indexOf('" + rule.ConditionMatchValue + "')!=-1 ";
                    }

                    /////////////////////////////////////
                    // ADD EVENT LISTENERS 
                    /////////////////////////////////////
                    if (conditionFieldType == "radio") //if RADIO 
                    {
                        sb.AppendLine("$(\"input[name='" + buildConditionField + "']\")");
                        sb.AppendLine(".on( 'click', function() { ");
                        //sb.AppendLine("if( " + conditionRule + );
                        sb.AppendLine("if( " + conditionRule + conditionItemBuild);
                        sb.AppendLine(".is(':checked')) { $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else { $('" + buildActionField + "')" + visB + "}  });");
                    }

                    if (conditionFieldType == "select")  //if SELECT 
                    {
                        sb.AppendLine("$(\"#" + buildConditionField + "\").on( 'change', function() { ");
                        //sb.AppendLine("if( " + conditionRule + "$(\"#" + buildConditionField + " option[value='" + rule.ConditionMatchValue + "']\")");
                        //sb.AppendLine("if( " + conditionRule + "$(\"#" + buildConditionField + " \")");
                        sb.AppendLine("if( " + conditionRule + conditionItemBuild + ")");
                        sb.AppendLine("{  $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else {  $('" + buildActionField + "')" + visB + "}  });");
                    }

                    if (conditionFieldType == "checkbox") // if CHECKBOX
                    {
                        sb.AppendLine("$(\"input[name='" + buildConditionField + "']\")");
                        sb.AppendLine(".on( 'click', function() { ");
                        sb.AppendLine("if( " + conditionRule + conditionItemBuild);
                        sb.AppendLine(".is(':checked')) { $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else { $('" + buildActionField + "')" + visB + "}  });");
                    }

                    if (conditionFieldType == "textbox" || conditionFieldType == "email") // if TEXTBOX
                    {
                        sb.AppendLine("$(\"input[name='" + buildConditionField + "']\")");
                        sb.AppendLine(".on( 'keyup', function() { ");
                        sb.AppendLine("if( " + conditionRule + "(" + conditionItemBuild + "))");
                        sb.AppendLine("{ ");
                        sb.AppendLine("     $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("  else { ");
                        sb.AppendLine("     $('" + buildActionField + "')" + visB + "}  ");
                        sb.AppendLine("});");

                        sb.AppendLine("function " + buildConditionField + "_" + action + "() { ");
                        sb.AppendLine("if( " + conditionRule + "(" + conditionItemBuild + ")");
                        sb.AppendLine(") { $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else { $('" + buildActionField + "')" + visB + "}} " + buildConditionField + "_" + action + "();");

                    }

                } //end CHECK

            };
            //end rules

            /////////////////////////////
            // CALCULATIONS ////////////////////
            /////////////////////////////

            foreach (var calc in form.Calculations)
            {

                if (!String.IsNullOrEmpty(calc.Formula))
                { //check formula is not empty
                    var formula = " $('#" + calc.ResultFieldOut + "').val(";
                    var start = "";
                    string[] formulaArray = calc.Formula.Split('#');

                    foreach (var item in formulaArray)
                    {
                        if (item.IndexOf("input_") != -1)
                        {
                            if ((item.IndexOf("_checkbox") != -1))
                            {
                                start += "var " + item + "=0; ";
                                start += "$('input:checkbox[name=" + item + "]:checked').each(function()";
                                start += "    {";
                                start += "if(isNaN(parseFloat($(this).val() ))) { " + item + "+=0;}";
                                start += " else { " + item + "+=parseFloat($(this).val()) }"; // else get the value
                                //start += item + "+= $(this).val();";
                                start += "    });";
                                formula += "parseFloat(" + item + ")";

                                var field = item.Split("_");
                                var fieldId = Convert.ToInt32(field[2]);


                                var formFieldElements = formsDB.FieldItemElements
                                                  .Where(c => c.FieldItemId == fieldId);

                                // var last = formFieldElements.Last();
                                foreach (var element in formFieldElements)
                                {
                                    var fieldElement = item + "_" + element.FieldItemElementId;
                                    sb.AppendLine("$('#" + fieldElement + "').change(function() { calc(); }); ");
                                }
                            }
                            else
                            {
                                sb.AppendLine("$('#" + item + "').change(function() { calc(); }); ");

                                start += "var " + item + "=0; ";
                                start += "if(isNaN(parseFloat($('#" + item + "').val() ))) { " + item + "=0;}";
                                start += " else { " + item + "=parseFloat($('#" + item + "').val()) }"; // else get the value

                                formula += "parseFloat(" + item + ")";
                            }
                        }
                        else //if (item.IndexOf("<condition") != -1)
                        {
                            formula = formula + item;
                        }

                    }
                    formula = formula + ");";
                    sb.AppendLine("function calc(){");
                    sb.AppendLine(start);
                    sb.AppendLine(formula);
                    sb.AppendLine("}");
                }
            }

            // END CALCULATIONS
            return sb.ToString();


        }


        public static string templateRulesCalc(FormTemplates form)
        //public static string rulesCalc(IEnumerable<dynamic> forms)
        {
            // WebApplication1.Models.FormeyEntityModel formsDB = new WebApplication1.Models.FormeyEntityModel();
            WebApplication1.Models.frmy01_DevContext formsDB = new WebApplication1.Models.frmy01_DevContext();
            //FormList form = (forms)obj;

            StringBuilder buildActionField = new StringBuilder();

            StringBuilder sb = new StringBuilder();
            /////////////////////////////
            // RULES ////////////////////
            /////////////////////////////
            // TODO - rules not being applied on load
            // DONE CONDITION
            //      - is equal
            //      - is not equal
            // DONE FIELD TYPES 
            //      - RADIO
            //      - CHECKBOX
            //      - SELECT

            foreach (var rule in form.TemplateRules)
            {
                //sb.Clear();
                buildActionField.Clear();

                var conditionField = formsDB.FieldItems.Find(rule.ConditionField);
                if (conditionField != null)
                { //CHECK: Could have added rule but not added anything to the fields of a rule
                    var conditionFieldType = conditionField.ElementType;

                    string action = rule.Action;
                    //string multiSelectItemId = "";

                    ////////////////////////////
                    // BUILD Condition Field used later in the code
                    ////////////////////////////
                    string buildConditionField = "input_" + conditionFieldType + "_" + rule.ConditionField;

                    ////////////////////////////////////
                    // Get the fields to Action(show/hide), this can be multiple
                    //string[] arrActionFields;
                    ////////////////////////////////////
                    string[] actionFields = rule.ActionField.Split(',');
                    int i = 0;
                    foreach (string field in actionFields)
                    {
                        buildActionField.Append("#item_" + field);
                        if (actionFields.Length > 1 && i != actionFields.Length - 1) buildActionField.Append(",");
                        i++;
                    }

                    //hide action field/s 
                    string visA = ""; //set visibility item based on action
                    string visB = "";
                    if (action == "Show")
                    {
                        sb.Append("$('" + buildActionField + "').fadeOut('slow');");
                        visA = ".fadeIn('slow')";
                        visB = ".fadeOut('slow')";

                    }
                    //attach event listener to condition field if action HIDE
                    if (action == "Hide")
                    {
                        visA = ".fadeOut('slow')";
                        visB = ".fadeIn('slow')";
                    }

                    string conditionRule = "";
                    if (rule.Condition.IndexOf("Not") != -1 || rule.Condition.IndexOf("Empty") != -1)
                    {
                        conditionRule = "!";
                    }

                    var conditionItemBuild = "";

                    // EMPTY / FILLED LOGIC
                    if (rule.Condition.IndexOf("Empty") != -1 || rule.Condition.IndexOf("Filled") != -1)
                    {   // select single item no match value
                        if (conditionFieldType == "radio" || conditionFieldType == "checkbox")
                            conditionItemBuild = "$(\"input[name='" + buildConditionField + "']\")";
                        else //select
                        {
                            if (conditionFieldType == "select")
                                conditionItemBuild = "$(\"#" + buildConditionField + " :selected\").val()!='' ";
                            if (conditionFieldType == "textbox" || conditionFieldType == "email")
                                conditionItemBuild = "$(\"#" + buildConditionField + "\").val().length!=0";
                        }

                    }
                    else //check for specific item in multi item
                    {
                        if (conditionFieldType == "radio" || conditionFieldType == "checkbox")
                            conditionItemBuild = "$(\"input[name='" + buildConditionField + "'][value='" + rule.ConditionMatchValue + "']\")";
                        else //select
                            conditionItemBuild = "$(\"#" + buildConditionField + " option[value='" + rule.ConditionMatchValue + "']\").is(':selected')";
                    }

                    // CONTAINS VALUE / NOT CONTAIN VALUE
                    if (rule.Condition.IndexOf("Contain") != -1)
                    {
                        conditionItemBuild = "$(\"#" + buildConditionField + "\").val().indexOf('" + rule.ConditionMatchValue + "')!=-1 ";
                    }

                    /////////////////////////////////////
                    // ADD EVENT LISTENERS 
                    /////////////////////////////////////
                    if (conditionFieldType == "radio") //if RADIO 
                    {
                        sb.AppendLine("$(\"input[name='" + buildConditionField + "']\")");
                        sb.AppendLine(".on( 'click', function() { ");
                        //sb.AppendLine("if( " + conditionRule + );
                        sb.AppendLine("if( " + conditionRule + conditionItemBuild);
                        sb.AppendLine(".is(':checked')) { $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else { $('" + buildActionField + "')" + visB + "}  });");
                    }

                    if (conditionFieldType == "select")  //if SELECT 
                    {
                        sb.AppendLine("$(\"#" + buildConditionField + "\").on( 'change', function() { ");
                        //sb.AppendLine("if( " + conditionRule + "$(\"#" + buildConditionField + " option[value='" + rule.ConditionMatchValue + "']\")");
                        //sb.AppendLine("if( " + conditionRule + "$(\"#" + buildConditionField + " \")");
                        sb.AppendLine("if( " + conditionRule + conditionItemBuild + ")");
                        sb.AppendLine("{  $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else {  $('" + buildActionField + "')" + visB + "}  });");
                    }

                    if (conditionFieldType == "checkbox") // if CHECKBOX
                    {
                        sb.AppendLine("$(\"input[name='" + buildConditionField + "']\")");
                        sb.AppendLine(".on( 'click', function() { ");
                        sb.AppendLine("if( " + conditionRule + conditionItemBuild);
                        sb.AppendLine(".is(':checked')) { $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else { $('" + buildActionField + "')" + visB + "}  });");
                    }

                    if (conditionFieldType == "textbox" || conditionFieldType == "email") // if TEXTBOX
                    {
                        sb.AppendLine("$(\"input[name='" + buildConditionField + "']\")");
                        sb.AppendLine(".on( 'keyup', function() { ");
                        sb.AppendLine("if( " + conditionRule + "(" + conditionItemBuild + "))");
                        sb.AppendLine("{ ");
                        sb.AppendLine("     $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("  else { ");
                        sb.AppendLine("     $('" + buildActionField + "')" + visB + "}  ");
                        sb.AppendLine("});");

                        sb.AppendLine("function " + buildConditionField + "_" + action + "() { ");
                        sb.AppendLine("if( " + conditionRule + "(" + conditionItemBuild + ")");
                        sb.AppendLine(") { $('" + buildActionField + "')" + visA + "} ");
                        sb.AppendLine("else { $('" + buildActionField + "')" + visB + "}} " + buildConditionField + "_" + action + "();");

                    }

                } //end CHECK

            };
            //end rules

            /////////////////////////////
            // CALCULATIONS ////////////////////
            /////////////////////////////

            foreach (var calc in form.TemplateCalculations)
            {

                if (!String.IsNullOrEmpty(calc.Formula))
                { //check formula is not empty
                    var formula = " $('#" + calc.ResultFieldOut + "').val(";
                    var start = "";
                    string[] formulaArray = calc.Formula.Split('#');

                    foreach (var item in formulaArray)
                    {
                        if (item.IndexOf("input_") != -1)
                        {
                            if ((item.IndexOf("_checkbox") != -1))
                            {
                                start += "var " + item + "=0; ";
                                start += "$('input:checkbox[name=" + item + "]:checked').each(function()";
                                start += "    {";
                                start += "if(isNaN(parseFloat($(this).val() ))) { " + item + "+=0;}";
                                start += " else { " + item + "+=parseFloat($(this).val()) }"; // else get the value
                                //start += item + "+= $(this).val();";
                                start += "    });";
                                formula += "parseFloat(" + item + ")";

                                var field = item.Split("_");
                                var fieldId = Convert.ToInt32(field[2]);


                                var formFieldElements = formsDB.FieldItemElements
                                                  .Where(c => c.FieldItemId == fieldId);

                                // var last = formFieldElements.Last();
                                foreach (var element in formFieldElements)
                                {
                                    var fieldElement = item + "_" + element.FieldItemElementId;
                                    sb.AppendLine("$('#" + fieldElement + "').change(function() { calc(); }); ");
                                }
                            }
                            else
                            {
                                sb.AppendLine("$('#" + item + "').change(function() { calc(); }); ");

                                start += "var " + item + "=0; ";
                                start += "if(isNaN(parseFloat($('#" + item + "').val() ))) { " + item + "=0;}";
                                start += " else { " + item + "=parseFloat($('#" + item + "').val()) }"; // else get the value

                                formula += "parseFloat(" + item + ")";
                            }
                        }
                        else //if (item.IndexOf("<condition") != -1)
                        {
                            formula = formula + item;
                        }

                    }
                    formula = formula + ");";
                    sb.AppendLine("function calc(){");
                    sb.AppendLine(start);
                    sb.AppendLine(formula);
                    sb.AppendLine("}");
                }
            }

            // END CALCULATIONS
            return sb.ToString();


        }

        public static Guid addToLogGeneral(Guid formId, ClientInfo c)
        {
            using(var formsDB = new WebApplication1.Models.frmy01_DevContext())
            {
                //Models.FormeyEntityModel ;
                //add new form load to LogGeneral
                
                //formsDB.Configuration.AutoDetectChangesEnabled = false;

                string name = RegionInfo.CurrentRegion.DisplayName;
                LogGenerals lg = new LogGenerals();
                lg.LogGeneralId = Guid.NewGuid();
                lg.DateAdded = DateTime.UtcNow;
                lg.FormListId = formId;// ViewBag.id;
                lg.Device = c.UserAgent.Family;
                lg.Browser = c.UserAgent.Family;
                lg.BrowserVersion = c.UserAgent.Family;
                //lg.Country = name;
                formsDB.LogGenerals.Add(lg);
                formsDB.SaveChanges();

                return lg.LogGeneralId;
            }

            
        }

        //[OutputCache(Duration = 7200, VaryByParam = "userId")]
      

    }

    public class ReCaptchaClass
    {
        public static string Validate(string EncodedResponse)
        {
            var client = new System.Net.WebClient();

            string PrivateKey = System.Configuration.ConfigurationManager.AppSettings["RECAPTCHA_server"]; // "6LfqmAoTAAAAAPCQ0DW8dwTcRZlyMOYp-7mAzQbu";

            var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));

            var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaClass>(GoogleReply);

            return captchaResponse.Success;
        }

        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }


        private List<string> m_ErrorCodes;
    }

}