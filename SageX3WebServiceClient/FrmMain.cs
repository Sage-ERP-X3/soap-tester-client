using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using SageX3SoapWsTester.CAdxWebServiceXmlCC;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace SageX3SoapWsTester
{
    public partial class FrmMain : Form
    {

        #region Properties/Fields

        CAdxWebServiceXmlCCServiceBasic _x3Ws = new CAdxWebServiceXmlCCServiceBasic(); // X3 webservice object
        string _paramFile = "";
        #endregion 

        #region Form methods

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            SetInitialValues();
        }

        //Text box hotkeys
        private void txtResult_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, txtResult);
        }

        private void txtParam_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, txtParam);
        }

        private void txtReplDesc_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, txtReplDesc);
        }

        private void txtMsgRep_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, txtMsgRep);
        }

        private void txtTrace_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, txtTrace);
        }

        //Button actions
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            CallWebService();
        }

        private void bntBeautify_Click(object sender, EventArgs e)
        {
            FormatResult();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clear contents
            ClearOutputs(false);
        }

        private void btnX3_Click(object sender, EventArgs e)
        {
            String x3Url = txtWebsite.Text.Trim() + "/auth/login/page";
            Process.Start("chrome.exe", x3Url);
        }

        //Selection actions
        private void cmbWebService_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWsMethodList();
            SetCriteriaKey();
        }

        private void lstMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeFormState();
        }


        // Link actions
        private void lnkResult_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExternal(txtResult.Text);
        }

        private void lnkTrace_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExternal(txtTrace.Text);
        }

        private void lnkReplDesc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowExternal(txtReplDesc.Text);
        }

        private void lnkMsgRep_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            ShowExternal(txtMsgRep.Text);
        }

        //Other link actions
        private void lnkParam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists("C:\\Program Files (x86)\\Notepad++\\notepad++.exe") == true)
            {
                Process.Start("notepad++.exe", _paramFile);
            }
            else
            {
                Process.Start("notepad.exe", _paramFile);
            }

        }

        private void lnkConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowConfiguration();
        }

        //Other parameter types
        private void rdbXml_CheckedChanged(object sender, EventArgs e)
        {
            if (pnlParam.Enabled == true)
            {
                GetSampleParameters();
            }
        }

        private void rdbJson_CheckedChanged(object sender, EventArgs e)
        {
            if (pnlParam.Enabled == true)
            {
                GetSampleParameters();
            }
        }

        //Other sub program types
        private void rdbObject_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWsList();
        }

        private void rdbSubProgram_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWsList();
        }

        #endregion

        #region Private methods

        private void SetInitialValues()
        {
            // Get app settings
            txtPoolAlias.Text = ConfigurationManager.AppSettings["PoolAlias"];
            txtRetrievalUrl.Text = ConfigurationManager.AppSettings["RetreivalUrl"];
            txtWebsite.Text = ConfigurationManager.AppSettings["x3Website"];
            txtCodeUser.Text = ConfigurationManager.AppSettings["UserID"];
            txtPassword.Text = ConfigurationManager.AppSettings["Password"];
            if (File.Exists("ExtConfig.txt") == true)
            {
                txtWebsite.Text = File.ReadAllText("ExtConfig.Txt");
            }

            // Set initial defaults
            lstMethods.SelectedIndex = 0;

            // Set parameter input type 
            rdbXml.Checked = true;
            // Set configutation defaults
            cmbAppTraceLevel.SelectedIndex = 2;
            cmbReturnFormat.SelectedIndex = 0;
            chkTraceApp.Checked = true;
            chkTraceWs.Checked = true;
            chkBeautify.Checked = true;
            // Set code language 
            cmbCodeLang.SelectedIndex = 0;
            // Set basci authentication
            rdbBasic.Checked=true;
            // Set web service type to object
            rdbObject.Checked = true;
        }

        private void ChangeFormState()
        {
            // Disable all selection pannels
            pnlSelection.Enabled = true;
            pnlConfiguration.Enabled = true;
            pnlList.Enabled = false;
            pnlCriteria.Enabled = false;
            pnlParam.Enabled = false;
            pnlDelLines.Enabled = false;
            pnlAction.Enabled = false;

            string methodName = lstMethods.Text.ToString().ToUpper().Trim();
            switch (methodName)
            {
                case "GETDESCRIPTION":
                case "GETDATAXMLSCHEMA":
                    break;
                case "RUN":
                    pnlParam.Enabled = true;
                    GetSampleParameters();
                    break;
                case "QUERY":
                    pnlList.Enabled = true;
                    pnlCriteria.Enabled = true;
                    break;
                case "READ":
                    pnlCriteria.Enabled = true;
                    break;
                case "MODIFY":
                    pnlCriteria.Enabled = true;
                    pnlParam.Enabled = true;
                    GetSampleParameters();
                    break;
                case "DELETE":
                    pnlCriteria.Enabled = true;
                    break;
                case "DELETELINES":
                    PopulateBlockKeys();
                    pnlCriteria.Enabled = true;
                    pnlDelLines.Enabled = true;
                    break;
                case "SAVE":
                    GetSampleParameters();
                    pnlParam.Enabled = true;
                    break;
                case "ACTIONOBJECT":
                    GetSampleParameters();
                    pnlAction.Enabled = true;
                    pnlParam.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void UpdateWsList()
        {
            // Clean up the web srvice drop down list
            cmbWebService.Items.Clear();

            string wsName = "";

            if (rdbObject.Checked == true)
            {
                // Add objects to Web Service List 
                string objectsPath = Application.StartupPath + "\\ParamFiles\\Object\\";
                List<string> dirsO = new List<string>(Directory.EnumerateDirectories(objectsPath));
                foreach (var dir in dirsO)
                {
                    wsName = dir.Substring(dir.LastIndexOf("\\") + 1);
                    cmbWebService.Items.Add(wsName);
                }

            }
            else
            {
                // Add sub programs to Web Service List 
                string subProgsPath = Application.StartupPath + "\\ParamFiles\\SubProgram\\";
                List<string> dirs = new List<string>(Directory.EnumerateDirectories(subProgsPath));
                foreach (var dir in dirs)
                {
                    wsName = dir.Substring(dir.LastIndexOf("\\") + 1);
                    cmbWebService.Items.Add(wsName);
                }
            }

            cmbWebService.SelectedIndex = 0;

        }

        private void UpdateWsMethodList()
        {
            string wsName = cmbWebService.Text.Trim().ToUpper();

            //Get current selected method 
            string curMethod = lstMethods.Text;

            // Rebuild the methods drop
            lstMethods.Items.Clear();
            lstMethods.Items.Add("getDescription");
            lstMethods.Items.Add("getDataXmlSchema");

            if (rdbObject.Checked == true)
            {
                lstMethods.Items.Add("query");
                lstMethods.Items.Add("read");
                lstMethods.Items.Add("delete");
                lstMethods.Items.Add("save");
                lstMethods.Items.Add("deleteLines");
                lstMethods.Items.Add("modify");
                lstMethods.Items.Add("actionObject");
            }
            else
            {
                lstMethods.Items.Add("run");
            }

            int selIndex = lstMethods.FindString(curMethod);
            if (selIndex >= 0)
            {
                lstMethods.SelectedIndex = selIndex;
            }
            else
            {
                lstMethods.SelectedIndex = 0;
            }
        }

        private void SetCriteriaKey()
        {
            string wsName = cmbWebService.Text.ToUpper().Trim();
            string criteriaKey = "TEST";
            switch (wsName)
            {
                case "WSAUS":
                    //Users
                    criteriaKey = "WSTST";
                    break;
                case "WSBPC":
                    // Customers
                    criteriaKey = "WSBPC";
                    break;
                case "WSITM":
                    //PRoducts
                    criteriaKey = "WSITM";
                    break;
                case "WSSOH_WS":
                    // Sales order
                    criteriaKey = "WSSOH";
                    break;
                case "WSREP":
                    // Sales Rep
                    criteriaKey = "WSREP";
                    break;
                case "WSOPP":
                    // Projects
                    criteriaKey = "WSOPP";
                    break;
                case "WSITS":
                    // Product sales
                    criteriaKey = "";
                    break;
                case "WSSQH":
                    //Quote
                    criteriaKey = "WSSQH";
                    break;
                case "WSMFG":
                    // Work order
                    criteriaKey = "WSMFG";
                    break;
                default:
                    break;
            }

            txtCriteria.Text = criteriaKey;
        }

        private void ErrorHandler(string errMessage)
        {
            string debugApp = System.Environment.GetEnvironmentVariable("SEMDEBUG");
            if (debugApp == "ON")
            {
                txtMsgRep.Refresh();
                if (MessageBox.Show(errMessage + ".\r\nDo you want to debug?", this.lblTitle.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Debugger.Break();
                }
            }else 
            {
                MessageBox.Show(errMessage);
            }
        }

        private void FormatResult()
        {
            string srcString = txtResult.Text.ToString().Trim(); 
            string retText="";
            if (String.IsNullOrEmpty(srcString) == true)
            {
                MessageBox.Show("No text to convert");
            }
            else if (srcString.Substring(0, 5).ToLower() == "<?xml")
            {
                retText = FormatXmlString(txtResult.Text.ToString());
            }
            else if (cmbReturnFormat.Text.ToString() == "JSON")
            {
                retText = FormatJsonString(srcString);
            }else
            {
                retText = srcString;
            }
            txtResult.Text = retText;
            txtResult.Refresh();
        }

        private string FormatXmlString(string xmlText)
        {
            string retString = "";
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xmlText);

                writer.Formatting = System.Xml.Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                retString = sReader.ReadToEnd().ToString();

            }
            catch (XmlException ex)
            {
                ErrorHandler("Unable to format the Xml" + ex.Message);
            }
            return retString;
        }

        private string FormatJsonString(string json)
        {
            string retString = "";
            try
            {
                var t = json;
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(t);
                var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
                retString = f.ToString();
            } catch (Exception ex)
            {
                 ErrorHandler(ex.Message);
            }
            return retString;
        }

        private string CreateTextFile(string outputString, string fileName)
        {
            string tmpFolder = Path.GetTempPath();
            string outputFile = tmpFolder + fileName;

            try
            {
                using (StreamWriter outfile = new StreamWriter(outputFile))
                {
                    outfile.Write(outputString);
                }
            } catch (Exception ex)
            {
                 ErrorHandler(ex.Message);
            }

            return outputFile;
        }

        private string ReadTextFile(string inputFile)
        {
            string retValue = "";
            try
            {
                retValue = File.ReadAllText(inputFile);
            }
            catch (Exception ex)
            {
                 ErrorHandler(ex.Message);
            }

            return retValue;
        }

        private void ClearOutputs(bool innitialize)
        {
            // Clear the current text
            txtResult.Text = (innitialize == true) ? "Retrieving details from web service ..." : "";
            txtResult.Refresh();
            txtMsgRep.Text = "";
            txtMsgRep.Refresh();
            txtTrace.Text = "";
            txtTrace.Refresh();
            txtReplDesc.Text = "";
            txtReplDesc.Refresh();
        }

        private void ShowExternal(string showText)
        {
            string modifiedText= showText.Trim();

            if (String.IsNullOrEmpty(modifiedText) == false 
                && modifiedText.Substring(0, 5).ToUpper() == "<?XML")
            {
                modifiedText = modifiedText.Replace("\r", "");
            }

            try
            {
                if (String.IsNullOrEmpty(modifiedText.Trim()) == true)
                {
                    MessageBox.Show("No data to display");
                }
                else if ((File.Exists("C:\\Program Files (x86)\\Notepad++\\notepad++.exe") == true))
                {
                    // Dump trace to a temporary text file
                    String traceFile = CreateTextFile(modifiedText, "trace.txt");
                    // Open Notepad++
                    Process.Start("notepad++.exe", traceFile);
                }
                else if (modifiedText.Substring(0, 5).ToUpper() == "<?XML")
                {
                    // Dump trace to a temporary text file
                    string resultFile = CreateTextFile(modifiedText, "result.xml");
                    // Open internet exploere 
                    Process.Start("IEXPLORE.EXE", resultFile);
                }else
                {
                    // Dump trace to a temporary text file
                    String traceFile = CreateTextFile(modifiedText, "trace.txt");
                    // Open notepad 
                    Process.Start("notepad.exe", traceFile);
                }
            }
            catch (Exception ex)
            {
                 ErrorHandler(ex.Message);
            }
        }

        private void GetSampleParameters()
        {
            string wsName = cmbWebService.Text.ToUpper().Trim();
            string fileType = (rdbXml.Checked == true) ? ".XML" : ".JSON";
            string wsType = (rdbObject.Checked == true) ? "Object" : "SubProgram";
            _paramFile = Application.StartupPath +
                        "\\ParamFiles\\" + wsType  + "\\" + wsName + "\\" + 
                        wsName + "_" + lstMethods.Text.ToUpper() + 
                        fileType;
            try
            {
                if (File.Exists(_paramFile) == true)
                {
                    txtParam.Text = ReadTextFile(_paramFile);
                } else
                {
                    txtParam.Text = "";
                }

            } catch (Exception ex)
            {
                ErrorHandler(ex.Message);
            }

         }

        private string GetRequestConfig()
        {
            StringBuilder sb = new StringBuilder();
            //Set return format
            sb.Append("adxwss.optreturn=" + cmbReturnFormat.Text.ToString().Trim());

            if (chkTraceWs.Checked == true)
            {
                // Web service trace
                sb.Append("&adxwss.trace.on=on");
                sb.Append("&adxwss.trace.size=" + txtTraceWsSize.Text.ToString().Trim());
            }

            if (chkTraceApp.Checked == true)
            {
                // Application trace
                sb.Append("&adonix.trace.on=on");
                sb.Append("&adonix.trace.size=" + txtAppTraceSize.Text.ToString().Trim());
                sb.Append("&adonix.trace.level=3");
            }


            if (chkBeautify.Checked == true)
            {
                //Beautify output (new)
                sb.Append("&adxwss.beautify=true");
            }

            return sb.ToString();

        }

        private void ShowConfiguration()
        {
            string requestConfig = GetRequestConfig();
            ShowExternal(requestConfig);
        }

        private void ShowResults(CAdxWebServiceXmlCC.CAdxResultXml result, string timeTaken)
        {
            int setPage = 0;

            txtTesterTime.Text = timeTaken;
            txtResult.Text = "";
            txtTrace.Text = "";
            txtMsgRep.Text = "";
            txtReplDesc.Text = "";


            try
            {
                // Show Reply
                if (result.resultXml != null)
                {
                    txtResult.Text = result.resultXml.ToString().Trim();
                    FormatResult();
                    setPage = 1;
                }
                // Show message and report
                if (result.messages != null && result.messages.Length > 0)
                {
                    txtMsgRep.Text = txtMsgRep.Text +
                                        "************ Message ************";
                    // Show error message
                    for (int i = 0; i < result.messages.Length; i++)
                    {
                        txtMsgRep.Text = txtMsgRep.Text +
                                            "\n" +
                                            result.messages[i].message.ToString();
                    }
                    setPage = 2;
                }

                // Show error report
                if (result.technicalInfos.processReportSize > 0)
                {
                    txtMsgRep.Text = txtMsgRep.Text +
                                        "\n\n************ Report ************\n\n" +
                                        result.technicalInfos.processReport;
                }

                txtResult.Text = txtResult.Text.Replace("\n", "\r\n");
                txtMsgRep.Text = txtMsgRep.Text.Replace("\n", "\r\n");


                // Show trace
                if (result.technicalInfos.traceRequestSize > 0)
                {
                    txtTrace.Text = result.technicalInfos.traceRequest.Replace("\n", "\r\n");
                }

                // Show reply description
                StringBuilder repDesc = new StringBuilder();
                repDesc.Append("status=[" + result.status.ToString() + "]\r\n");
                if (result.resultXml != null)
                {
                    repDesc.Append("hasResult=[" + (result.resultXml.Length > 0).ToString() + "]\r\n");
                }
                else
                {
                    repDesc.Append("hasResult=[0]\r\n");
                }

                // Show Response description
                repDesc.Append("hasProcessReport=[" + (result.technicalInfos.processReportSize > 0).ToString() + "]\r\n");
                repDesc.Append("hasTraceRequest=[" + (result.technicalInfos.traceRequestSize > 0).ToString() + "]\r\n");
                repDesc.Append("clientDuration=[ *** See tester time *** ]\r\n");
                repDesc.Append("serverDuration=[" + result.technicalInfos.totalDuration.ToString() + "]\r\n");
                repDesc.Append("loadWebsDuration=[" + result.technicalInfos.loadWebsDuration.ToString() + "]\r\n");
                repDesc.Append("poolDistribDuration=[" + result.technicalInfos.poolDistribDuration.ToString() + "]\r\n");
                repDesc.Append("poolWaitDuration=[" + result.technicalInfos.poolWaitDuration.ToString() + "]\r\n");
                repDesc.Append("poolExecDuration=[" + result.technicalInfos.poolExecDuration.ToString() + "]\r\n");
                repDesc.Append("poolRequestDuration=[" + result.technicalInfos.poolRequestDuration.ToString() + "]\r\n");
                repDesc.Append("switchUser=[" + result.technicalInfos.changeUserId.ToString() + "]\r\n");
                repDesc.Append("switchLang=[" + result.technicalInfos.changeLanguage.ToString() + "]\r\n");
                repDesc.Append("flushAdx=[" + result.technicalInfos.flushAdx.ToString() + "]\r\n");
                repDesc.Append("reloadWebs=[" + result.technicalInfos.reloadWebs.ToString() + "]\r\n");
                repDesc.Append("resumitAfterDBOpen=[" + result.technicalInfos.resumitAfterDBOpen.ToString() + "]\r\n");
                repDesc.Append("poolEntryIdx=[" + result.technicalInfos.poolEntryIdx.ToString() + "]\r\n");
                repDesc.Append("rowInDistribStack=[" + result.technicalInfos.rowInDistribStack.ToString() + "]\r\n");
                repDesc.Append("nbDistribCycle=[" + result.technicalInfos.nbDistributionCycle.ToString() + "]\r\n");
                repDesc.Append("busy=[" + result.technicalInfos.busy.ToString() + "]\r\n");
                txtReplDesc.Text = repDesc.ToString();

                tabWs.SelectedIndex = setPage;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex.Message);
            }
        }

        private void PopulateBlockKeys()
        {
            string wsName = cmbWebService.Text.ToUpper().Trim();
            if (wsName == "WSAUS")
            {
                // addresses
                txtBlockKey.Text = "AUS2~NBADR";
            }
            else if (wsName == "WSBPC")
            {
                // addresses
                txtBlockKey.Text = "BPAC~NBADR";
            }
            else if (wsName == "WSITM")
            {
                //pack units
                txtBlockKey.Text = "ITM3~BASTAB";
            }
            else if (wsName == "WSSOH_WS")
            {
                txtBlockKey.Text = "SOH4~NBLIG";
            }
            else if (wsName == "WSREP")
            {
                // addresses
                txtBlockKey.Text = "BPAR~NBADR";
            }
            else
            {
                txtBlockKey.Text = "nolines";
            }
            
        }

        private string[] GetDeleteLineKeys()
        {
            var lineKeys = new List<string>();
            string[] tmpKeys = txtLineKeys.Text.ToString().Split('\n');

            foreach (string key in tmpKeys)
            {
                string addkey = key.Replace("\r", "");
                if (string.IsNullOrEmpty(addkey) != true)
                {
                    lineKeys.Add(addkey);
                }
            }
            
            return lineKeys.ToArray();
        }

        private string GetAccessToken(string retrievalUrl)
        {
            WebClient client = new WebClient();
            // Get the embedded access token and strip out the double quotes
            string accessToken = client.DownloadString(retrievalUrl).Replace("\"", "");
            return accessToken;
        }

        private void TextBoxKeyDown(KeyEventArgs e, TextBox txtBox)
        {
            if (e.Control & e.KeyCode == Keys.A)
            {
                txtBox.SelectAll();
            }
            else if (e.Control & e.KeyCode == Keys.Back)
            {
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
            }
        }

        #endregion

        #region Left list methods

        private CAdxParamKeyValue[] GetCriteria(CAdxCallContext cAdxCallContext)
        {
            // Split Criteria string
            String[] keyValues = txtCriteria.Text.Split('\n');

            // Get left list keys
            XmlNode leftListNode = GetLeftListItems(cAdxCallContext);

            // SSHA Only supports upto 50 parameters
            CAdxParamKeyValue[] objectKeys = new CAdxParamKeyValue[keyValues.Length];

            if (leftListNode != null)
            {
                int keyIndex = 0;

                for (int i = 0; i < keyValues.Length; i++)
                {
                    if (String.IsNullOrEmpty(keyValues[i].Trim()) == false)
                    {
                        CAdxParamKeyValue paramKeyValue = CreateParamKeyValue(leftListNode, keyValues, i);
                        objectKeys[keyIndex] = paramKeyValue;
                        keyIndex++;
                    }
                }
            }

            return objectKeys;
        }

        private XmlNode GetLeftListItems(CAdxCallContext callContext)
        {
            XmlNode grpNode = null;

            try
            {
                string wsName = cmbWebService.Text.ToUpper().Trim();
                CAdxWebServiceXmlCC.CAdxResultXml result = _x3Ws.getDescription(callContext, wsName);

                if (result.resultXml != null)
                {
                    XmlDocument xmlD = new XmlDocument();
                    xmlD.LoadXml(result.resultXml);
                    XmlNodeList adxKey = xmlD.GetElementsByTagName("ADXKEY");
                    grpNode = adxKey.Item(0).FirstChild;
                }
                else
                {
                    if (result.messages != null && result.messages.Length > 0)
                    {
                        MessageBox.Show(result.messages[0].message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler(ex.Message);
            }

            return grpNode;
        }

        private CAdxParamKeyValue CreateParamKeyValue(XmlNode leftListXmlNode, String[] keyValues, int i)
        {
            CAdxParamKeyValue paramKeyValue = new CAdxParamKeyValue();
            paramKeyValue.key = leftListXmlNode.ChildNodes[i].Attributes["NAM"].Value;
            // Clean up value string
            string value = keyValues[i].Replace("\r", string.Empty);
            value = value.Replace("\n", string.Empty);
            paramKeyValue.value = value;
            return paramKeyValue;
        }

        #endregion

        #region Web Services Methods

        private void CallWebService()
        {
            try
            {
                //general variables
                string[] lineKeys;
                //Create WS objets
                CAdxWebServiceXmlCC.CAdxCallContext cAdxCallContext = new CAdxCallContext();
                CAdxWebServiceXmlCC.CAdxResultXml cAdxResultXml = new CAdxResultXml();
                CAdxParamKeyValue[] objectKeys;
                // Grab settings 
                string objectXml = txtParam.Text.ToString().Trim();
                string blockKey = txtBlockKey.Text.Trim();
                int listSize = String.IsNullOrEmpty(txtListSize.Text) ? 0 : int.Parse(txtListSize.Text);
                string actionCode = txtActionCode.Text.Trim();
                //Records application start time 
                DateTime dt = DateTime.Now;

                // Clear results, messages, traces, etc and set url
                ClearOutputs(true);

                // Set URL, get call context and authenticate
                _x3Ws.Url = txtWebsite.Text.Trim() + "/soap-generic/syracuse/collaboration/syracuse/CAdxWebServiceXmlCC";
                cAdxCallContext = GetCallContext();

                //Trust all certificates
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                // Trust our stagging server
                // System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, cert, chain, errors) => cert.Subject.Contains("sagex3.com"));

                //System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, cert, chain, errors) => cert.Subject.Contains("sageerpx3online.com"));

                // If you are using a third party certificate, add code here to validate it.

                // V9 requires prior authorization, establish the authorization method
                if (rdbBasic.Checked == true)
                {
                    //use basic authentication
                    _x3Ws.BasicAuth = true;
                    _x3Ws.Credentials = new System.Net.NetworkCredential(txtCodeUser.Text.ToString().Trim(), txtPassword.Text.ToString().Trim());
                }else
                {
                    //use Sage-ID OAuth authentication
                    _x3Ws.BasicAuth = false;
                    _x3Ws.AccessToken = GetAccessToken(txtRetrievalUrl.Text.Trim());
                }
                _x3Ws.PreAuthenticate = true;

                // Call appropiate WS method
                string wsName = cmbWebService.Text.ToUpper().Trim();
                string methodName = lstMethods.Text.ToUpper().Trim();
                switch (methodName)
                {
                    case "GETDESCRIPTION":
                        cAdxResultXml = _x3Ws.getDescription(cAdxCallContext, wsName);
                        break;
                    case "GETDATAXMLSCHEMA":
                        cAdxResultXml = _x3Ws.getDataXmlSchema(cAdxCallContext, wsName);
                        break;
                    case "RUN":
                        cAdxResultXml = _x3Ws.run(cAdxCallContext,wsName, objectXml);
                        break;
                    case "QUERY":
                        objectKeys = GetCriteria(cAdxCallContext);
                        cAdxResultXml = _x3Ws.query(cAdxCallContext, wsName, objectKeys, listSize);
                        break;
                    case "READ":
                        objectKeys = GetCriteria(cAdxCallContext);
                        cAdxResultXml = _x3Ws.read(cAdxCallContext,wsName, objectKeys);
                        break;
                    case "MODIFY":
                        objectKeys = GetCriteria(cAdxCallContext);
                        cAdxResultXml = _x3Ws.modify(cAdxCallContext,wsName, objectKeys, objectXml);
                        break;
                    case "SAVE":
                        cAdxResultXml = _x3Ws.save(cAdxCallContext, wsName, objectXml);
                        break;
                    case "DELETE":
                        objectKeys = GetCriteria(cAdxCallContext);
                        cAdxResultXml = _x3Ws.delete(cAdxCallContext,wsName, objectKeys);
                        break;
                    case "DELETELINES":
                        objectKeys = GetCriteria(cAdxCallContext);
                        lineKeys = GetDeleteLineKeys();
                        cAdxResultXml = _x3Ws.deleteLines(cAdxCallContext, wsName, objectKeys, blockKey, lineKeys);
                        break;
                    case "ACTIONOBJECT":
                        //**********************************************************************************************
                        cAdxResultXml = _x3Ws.actionObject(cAdxCallContext, wsName, actionCode, objectXml);
                        break;
                    default:
                        MessageBox.Show("Select a supported methods");
                        break;
                }

                ShowResults(cAdxResultXml, (DateTime.Now - dt).TotalMilliseconds.ToString());
            }
            catch (Exception ex)
            {
                ErrorHandler(ex.Message);
            }
        }

        private CAdxWebServiceXmlCC.CAdxCallContext GetCallContext()
        {
            CAdxCallContext callContext = new CAdxWebServiceXmlCC.CAdxCallContext();
            callContext.codeLang = cmbCodeLang.Text.ToString().Trim();
            callContext.poolAlias = txtPoolAlias.Text.ToString().Trim();
            callContext.requestConfig = GetRequestConfig();
            if (String.IsNullOrEmpty(lblPoolId.Text.Trim()) == true)
            {
                callContext.poolId = lblPoolId.Text.Trim();
            }
            return callContext;
        }

        #endregion

    }
}
