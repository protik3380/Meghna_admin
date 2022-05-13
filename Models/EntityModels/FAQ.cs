using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EFreshAdmin.Models.EntityModels
{
    public class FAQ
    {
        public long Id { get; set; }
        //[Required(ErrorMessage = "Please enter question")]
        public string Question { get; set; }
        [AllowHtml]
        //[Required(ErrorMessage = "Please enter answer")]
        public string Answer { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}