using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EastDapper.UnitTest.Model
{
    [Table("BankName", Schema = "shd")]
    public class BankName
    {
        [Key]
        public int CodeId { get; set; }
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public int? Type { get; set; }
        public int? State { get; set; }
        public int? PreCodeId { get; set; }
        public string NationalId { get; set; }
    }
}
