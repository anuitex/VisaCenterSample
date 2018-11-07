using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VisaCenter.Repository.Interfaces;

namespace VisaCenter.Repository.Models
{
    [Table("Visa")]
    public class Visa : IEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        // form
        public string FirstName { get; set; }

        public string LastName { get; set; }


        // .....

        public string PassportId { get; set; }

        // calculated 
        public double CreditRate { get; set; }

        public string VisaStatus { get; set; } = "Created";
    }
}
