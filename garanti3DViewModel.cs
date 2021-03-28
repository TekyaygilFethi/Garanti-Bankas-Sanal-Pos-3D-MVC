using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Garanti3D_PAY.Models
{
    public class garanti3DViewModel
    {
        [Required(ErrorMessage = " ")]
        public string strMode { get; set; }

        [Required(ErrorMessage = " ")]
        public string strApiVersion { get; set; }

        [Required(ErrorMessage = " ")]
        public string terminalprovuserid { get; set; }

        [Required(ErrorMessage = " ")]
        public string strType { get; set; }

        [Required(ErrorMessage = " ")]
        public string txnamount { get; set; }

        [Required(ErrorMessage = " ")]
        public string txncurrencycode { get; set; }

        public string txninstallmentcount { get; set; }

        [Required(ErrorMessage = " ")]
        public string strTerminalUserID { get; set; }

        [Required(ErrorMessage = " ")]
        public string strCardholderPresentCode { get; set; }

        [Required(ErrorMessage = " ")]
        public string secure3dsecuritylevel { get; set; }

        [Required(ErrorMessage = " ")]
        public string strOrderID { get; set; }


        public string strCustomeripaddress { get; set; }

        [Required(ErrorMessage = " ")]
        public string strTerminalID { get; set; }

        [Required(ErrorMessage = " ")]
        public string terminalmerchantid { get; set; }

        [Required(ErrorMessage = " ")]
        public string strStoreKey { get; set; }
        public string secure3dhash { get; set; }

        [Required(ErrorMessage = " ")]
        public string strProvisionPassword { get; set; }

        [Required(ErrorMessage = " ")]

        //public string action { get; set; }

        public string lang { get; set; }

        [Required(ErrorMessage = " ")]
        public string customeremailaddress { get; set; }


        [Required(ErrorMessage = " ")]

        [Display(Name = "Card Number")]
        public string cardnumber { get; set; }



     
        [Required(ErrorMessage = " ")]

        [Display(Name = "Card Expiry Month")]
        public string month { get; set; }

        [Required(ErrorMessage = " ")]

        [Display(Name = "Card Expiry Year")]
        public string year { get; set; }

        [Required(ErrorMessage = " ")]
        [Display(Name = "Card Security Number")]
        public string cardcvv2 { get; set; }


        [Required(ErrorMessage = " ")]

        public string success_url { get; set; }

        [Required(ErrorMessage = " ")]

        public string error_url { get; set; }

        //public string refreshtime { get; set; }
    }
}