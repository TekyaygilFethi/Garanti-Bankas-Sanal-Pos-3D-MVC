using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Garanti3D_PAY.Models;

namespace Garanti3D_PAY.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Mode = "PROD";
            ViewBag.ApiVersion = "512";
            ViewBag.TerminalProvUserID = "PROVAUT";
            ViewBag.type = "sales";
            ViewBag.amount = "1050";
            ViewBag.currencycode = "949";
            ViewBag.CardholderPresentCode = "13";
            ViewBag.TerminalUserID = "PROVAUT";
            ViewBag.Secure3DSecurityLevel = "3D";
            ViewBag.lang = "tr";

            ay_doldur();
            sene_doldur();
            return View();

            
        }

        [HttpPost]

        public ActionResult Index(garanti3DViewModel Model)
        {
            string strCardNo = Model.cardnumber.Replace(" ", "");
            string strExpiryMonth = Model.month;
            string strExpiryYear = Model.year.Substring(2, 2); //Senenin son 2 sayısını almak lazım. Yani 2021 >> 21 olmalı !


            string strCCV2 = Model.cardcvv2;




            string strMode = Model.strMode;
            string strApiVersion = Model.strApiVersion;
            string strTerminalProvUserID = Model.terminalprovuserid;
            string strType = Model.strType;
            string strAmount = Model.txnamount; //  İşlem Tutarı. Bunu View kısmında <input> elementi ile almak lazım aslında. Ancak 
            string strCurrencyCode = Model.txncurrencycode;
            string strInstallmentCount = Model.txninstallmentcount; // 'Taksit Sayısı. Boş gönderilirse taksit yapılmaz
            string strTerminalUserID = Model.strTerminalUserID;
            string strOrderID = Model.strOrderID; //Sayısal bir değer. Bu değeri veritabanında tuttuğunuz bir integeri +1 arttırarak her yeni siparişe yeni bir no. vermelisiniz.
            string strCustomeripaddress = "127.0.0.1"; //Denemeyi kendi bilgisayarınızda yaparken ! Sonra bu = Request.UserHostAddress; olmalı.
            string strcustomeremailaddress = "info@seyahat.com";  //Bu daha sonra bu sistemi kullanacak firmanın email'i ile değişecek. Test sırasında önemli değil.

            string strTerminalID = Model.strTerminalID; //Terminal No.
            string _strTerminalID = "0" + strTerminalID; //Terminal No. Başına 0 eklenerek 9 digite tamamlanmalıdır.
            string strCardholderPresentCode = Model.strCardholderPresentCode;

            string strSecure3DSecurityLevel = Model.secure3dsecuritylevel;



            string strlang = Model.lang;

            string strTerminalMerchantID = Model.terminalmerchantid; // 'Üye şyeri Numarası
            string strStoreKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"; // '3D Secure şifreniz. Bunu kendi sanalPos ekranınızda üretmiş olmanız gerekli !
            string strProvisionPassword = "BenimŞifrem$"; // PROVAUT şifresi. Bunu kendi sanalPos ekranınızda üretmiş olmanız gerekli !

 
            string strSuccessURL = "https://localhost:44361/Home/Success";   
            string strErrorURL = "https://localhost:44361/Home/Error";

 
            string strHostAddress = "https://sanalposprov.garanti.com.tr/servlet/gt3dengine";
 
            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA512(strTerminalID + strOrderID + strAmount + strSuccessURL + strErrorURL + strType + strInstallmentCount + strStoreKey + SecurityData).ToUpper();
 
            garanti3DViewModel gv = new garanti3DViewModel();
 
            gv.month = strExpiryMonth;
            gv.year = strExpiryYear;
            gv.cardcvv2 = strCCV2;
            gv.cardnumber = strCardNo;


            gv.customeremailaddress = strcustomeremailaddress;
            gv.strCustomeripaddress = strCustomeripaddress;

            gv.lang = strlang;

            gv.secure3dhash = HashData;
            gv.secure3dsecuritylevel = strSecure3DSecurityLevel;
            gv.strApiVersion = strApiVersion;
            gv.strCardholderPresentCode = strCardholderPresentCode;

            gv.strMode = strMode;
            gv.strOrderID = strOrderID;
            gv.strProvisionPassword = strProvisionPassword;

            gv.strTerminalID = strTerminalID;
            gv.strTerminalUserID = strTerminalUserID;
            gv.strType = strType;

            gv.terminalmerchantid = strTerminalMerchantID;
            gv.terminalprovuserid = strTerminalProvUserID;
            gv.txnamount = strAmount;
            gv.txncurrencycode = strCurrencyCode;
            gv.txninstallmentcount = strInstallmentCount;

            gv.success_url = strSuccessURL;
            gv.error_url = strErrorURL;





            string postData = "cardnumber=" + gv.cardnumber;
            postData += "&cardexpiredatemonth=" + gv.month;
            postData += "&cardexpiredateyear=" + gv.year;
            postData += "&cardcvv2=" + gv.cardcvv2;
            postData += "&mode=" + gv.strMode;
            postData += "&secure3dsecuritylevel=" + gv.secure3dsecuritylevel;
            postData += "&apiversion=" + gv.strApiVersion;



            postData += "&terminalprovuserid=" + gv.terminalprovuserid;
            postData += "&terminaluserid=" + gv.strTerminalUserID;
            postData += "&terminalmerchantid=" + gv.terminalmerchantid;
            postData += "&txntype=" + gv.strType;
            postData += "&txnamount=" + gv.txnamount;
            postData += "&txncurrencycode=" + gv.txncurrencycode;
            postData += "&txninstallmentcount=" + gv.txninstallmentcount;
            postData += "&orderid=" + gv.strOrderID;
            postData += "&terminalid=" + gv.strTerminalID;
            postData += "&successurl=" + gv.success_url;
            postData += "&errorurl=" + gv.error_url;
            postData += "&customeremailaddress=" + gv.customeremailaddress;
            postData += "&customeripaddress=" + gv.strCustomeripaddress;

            postData += "&secure3dhash=" + gv.secure3dhash;
         

            postData += "&lang=" + gv.lang;


            // Create a request using a URL that can receive a post.
            WebRequest request = WebRequest.Create(strHostAddress);
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            // string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();




            //Get the response.

            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            string responseFromServer = "";

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
                //// Display the content.
                Console.WriteLine(responseFromServer);
            }

            //Close the response.
            response.Close();

            ViewBag.sayfa = responseFromServer;


            return RedirectToAction("Banka");
            //return View();
        }

        public ActionResult Banka()
        {


            return View();
        }

        public ActionResult Success()
        {
            string strMode = Request.Form.Get("mode");
            string strApiVersion = Request.Form.Get("apiversion");
            string strTerminalProvUserID = Request.Form.Get("terminalprovuserid");
            string strType = Request.Form.Get("txntype");
            string strAmount = Request.Form.Get("txnamount");
            string strCurrencyCode = Request.Form.Get("txncurrencycode");
            string strInstallmentCount = Request.Form.Get("txninstallmentcount");
            string strTerminalUserID = Request.Form.Get("terminaluserid");
            string strOrderID = Request.Form.Get("oid");
            string strCustomeripaddress = Request.Form.Get("customeripaddress");
            string strcustomeremailaddress = Request.Form.Get("customeremailaddress");
            string strTerminalID = Request.Form.Get("clientid");
            string _strTerminalID = "0" + strTerminalID;
            string strTerminalMerchantID = Request.Form.Get("terminalmerchantid");
            string strStoreKey = "74726176656c63656e7465726d61726d617269736f796b75";
            //HASH doğrulaması için 3D Secure şifreniz
            string strProvisionPassword = "BaBa5249$";
            //HASH doğrulaması için TerminalProvUserID şifresini tekrar yazıyoruz
            string strSuccessURL = Request.Form.Get("successurl");
            string strErrorURL = Request.Form.Get("errorurl");
            string strCardholderPresentCode = "13";
            //3D Model işlemde bu değer 13 olmalı
            string strMotoInd = "N";
            //string strNumber = "";
            //Kart bilgilerinin boş gitmesi gerekiyor
            //string strExpireDate = "";
            //Kart bilgilerinin boş gitmesi gerekiyor
            //string strCVV2 = "";
            //Kart bilgilerinin boş gitmesi gerekiyor
            string strAuthenticationCode = Server.UrlEncode(Request.Form.Get("cavv"));
            string strSecurityLevel = Server.UrlEncode(Request.Form.Get("eci"));
            string strTxnID = Server.UrlEncode(Request.Form.Get("xid"));
            string strMD = Server.UrlEncode(Request.Form.Get("md"));
            string strMDStatus = Request.Form.Get("mdstatus");
            string strMDStatusText = Request.Form.Get("mderrormessage");
            string strHostAddress = "https://sanalposprov.garanti.com.tr/VPServlet";
            //Provizyon için xml'in post edileceği adres
            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strOrderID + strTerminalID + strAmount + SecurityData).ToUpper();
            //Daha kısıtlı bilgileri HASH ediyoruz.

            //    //strMDStatus.Equals(1)
            //    //"Tam Doğrulama"
            //    //strMDStatus.Equals(2)
            //    //"Kart Sahibi veya bankası sisteme kayıtlı değil"
            //    //strMDStatus.Equals(3)
            //    //"Kartın bankası sisteme kayıtlı değil"
            //    //strMDStatus.Equals(4)
            //    //"Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş"
            //    //strMDStatus.Equals(5)
            //    //"Doğrulama yapılamıyor"
            //    //strMDStatus.Equals(6)
            //    //"3-D Secure Hatası"
            //    //strMDStatus.Equals(7)
            //    //"Sistem Hatası"
            //    //strMDStatus.Equals(8)
            //    //"Bilinmeyen Kart No"
            //    //strMDStatus.Equals(0)
            //    //"Doğrulama Başarısız, 3-D Secure imzası geçersiz."

            //Hashdata kontrolü için bankadan dönen secure3dhash değeri alınıyor.
            string strHashData = Request.Form.Get("secure3dhash");
            string ValidateHashData = GetSHA512(strTerminalID + strOrderID + strAmount + strCurrencyCode + strSuccessURL + strErrorURL + strType + strInstallmentCount + strStoreKey + SecurityData).ToUpper();

            //İlk gönderilen ve bankadan dönen HASH değeri yeni üretilenle eşleşiyorsa o zaman 2nci adım olarak provizyon XML'i yaratıp bankaya onay almak için gönderiyoruz.;

            if (strHashData == ValidateHashData)
            {

                ViewBag.lblResult1 = "Sayısal Imza Geçerli";
                //lblResult1.Text = "Sayısal Imza Geçerli";

                //Tam Doğrulama, Kart Sahibi veya bankası sisteme kayıtlı değil, Kartın bankası sisteme kayıtlı değil
                //Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş responselarını alan
                //işlemler için Provizyon almaya çalışıyoruz

                if (strMDStatus == "1" | strMDStatus == "2" | strMDStatus == "3" | strMDStatus == "4")
                {
                    //Provizyona Post edilecek XML Şablonu
                    string strXML = null;
                    strXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<GVPSRequest>" + "<Mode>" + strMode + "</Mode>" + "<Version>" + strApiVersion + "</Version>" + "<ChannelCode></ChannelCode>" + "<Terminal><ProvUserID>" + strTerminalProvUserID + "</ProvUserID><HashData>" + HashData + "</HashData><UserID>" + strTerminalUserID + "</UserID><ID>" + strTerminalID + "</ID><MerchantID>" + strTerminalMerchantID + "</MerchantID></Terminal>" + "<Customer><IPAddress>" + strCustomeripaddress + "</IPAddress><EmailAddress>" + strcustomeremailaddress + "</EmailAddress></Customer>" + "<Card><Number></Number><ExpireDate></ExpireDate><CVV2></CVV2></Card>" + "<Order><OrderID>" + strOrderID + "</OrderID><GroupID></GroupID><AddressList><Address><Type>B</Type><Name></Name><LastName></LastName><Company></Company><Text></Text><District></District><City></City><PostalCode></PostalCode><Country></Country><PhoneNumber></PhoneNumber></Address></AddressList></Order>" + "<Transaction>" + "<Type>" + strType + "</Type><InstallmentCnt>" + strInstallmentCount + "</InstallmentCnt><Amount>" + strAmount + "</Amount><CurrencyCode>" + strCurrencyCode + "</CurrencyCode><CardholderPresentCode>" + strCardholderPresentCode + "</CardholderPresentCode><MotoInd>" + strMotoInd + "</MotoInd>" + "<Secure3D><AuthenticationCode>" + strAuthenticationCode + "</AuthenticationCode><SecurityLevel>" + strSecurityLevel + "</SecurityLevel><TxnID>" + strTxnID + "</TxnID><Md>" + strMD + "</Md></Secure3D>" + "</Transaction>" + "</GVPSRequest>";

                    try
                    {
                        string data = "data=" + strXML;

                        WebRequest _WebRequest = WebRequest.Create(strHostAddress);
                        _WebRequest.Method = "POST";

                        byte[] byteArray = Encoding.UTF8.GetBytes(data);
                        _WebRequest.ContentType = "application/x-www-form-urlencoded";
                        _WebRequest.ContentLength = byteArray.Length;

                        Stream dataStream = _WebRequest.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();

                        WebResponse _WebResponse = _WebRequest.GetResponse();
                        Console.WriteLine(((HttpWebResponse)_WebResponse).StatusDescription);
                        dataStream = _WebResponse.GetResponseStream();

                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();

                        Console.WriteLine(responseFromServer);

                        //00 ReasonCode döndüğünde işlem başarılıdır. Müşteriye başarılı veya başarısız şeklinde göstermeniz tavsiye edilir. (Fraud riski)
                        if (responseFromServer.Contains("<ReasonCode>00</ReasonCode>"))
                        {
                            ViewBag.lblResult2 = strMDStatusText;
                            ViewBag.lblResult3 = "İşlem Başarılı";
                        }
                        else
                        {
                            ViewBag.lblResult2 = strMDStatusText;
                            ViewBag.lblResult3 = "İşlem Başarısız";
                        }

                    }
                    catch (Exception ex)
                    {
                        ViewBag.lblResult2 = strMDStatusText;
                        ViewBag.lblResult3 = ex.Message;
                    }

                }
                else
                {
                    ViewBag.lblResult2 = strMDStatusText;
                    ViewBag.lblResult3 = "İşlem Başarısız";

                }

            }
            else
            {
                ViewBag.lblResult1 = "Güvenlik Uyarısı. Sayısal Imza Geçerli Degil";
                ViewBag.lblResult2 = strMDStatusText;
                ViewBag.lblResult3 = " İşlem Başarısız";
            }

            return View();
        }

        public ActionResult Error()
        {
          

            return View();
        }

        public void ay_doldur()
        {

            List<SelectListItem> items = new List<SelectListItem>();


            //items.Add(new SelectListItem { Text = "", Value = null });
            items.Add(new SelectListItem { Text = "01", Value = "01" });
            items.Add(new SelectListItem { Text = "02", Value = "02" });
            items.Add(new SelectListItem { Text = "03", Value = "03" });
            items.Add(new SelectListItem { Text = "04", Value = "04" });
            items.Add(new SelectListItem { Text = "05", Value = "05" });
            items.Add(new SelectListItem { Text = "06", Value = "06" });
            items.Add(new SelectListItem { Text = "07", Value = "07" });
            items.Add(new SelectListItem { Text = "08", Value = "08" });
            items.Add(new SelectListItem { Text = "09", Value = "09" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "11", Value = "11" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });


            ViewBag.Ay = new SelectList(items, "Value", "Text");
        }

        public void sene_doldur()
        {
            DateTime bugün = DateTime.Now;
            int sene = bugün.Year;

            List<SelectListItem> items = new List<SelectListItem>();

            //items.Add(new SelectListItem { Text = "", Value = null });
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });
            sene = sene + 1;
            items.Add(new SelectListItem { Text = sene.ToString(), Value = sene.ToString() });

            ViewBag.Sene = new SelectList(items, "Value", "Text");
        }
        public static string GetSHA512(string sha512Data)
        {
            var sha = new SHA512CryptoServiceProvider();
            var hashedPassword = sha512Data;
            var hashBytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(hashedPassword);
            var inputBytes = sha.ComputeHash(hashBytes);
            return GetHexaDecimal(inputBytes);
        }
        public string GetSHA1(string SHA1Data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = SHA1Data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes);
        }
        public static string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;

            for (int n = 0; n <= length - 1; n++)
            {
                s.Append(String.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
            }
            return s.ToString();
        }


    }
}
