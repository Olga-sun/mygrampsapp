using System;
using System.Collections.Generic;
using System.Text;

namespace MyGrampsApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public int? FatherId { get; set; }
        public int? MotherId { get; set; }
        public List<Person> Children { get; set; } = new List<Person>();

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? Sex { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? Notes { get; set; }
        public int UserId { get; set; }
        public int? BirthPlaceId { get; set; }
        public string? BirthPlaceName { get; set; }
        public string? MaidenName { get; set; }

        
        public string FullName
        {
            get
            {
                string datePart = BirthDate.HasValue ? $" ({BirthDate.Value.ToShortDateString()})" : " (Невідомо)";
                return $"{LastName} {FirstName} {Patronymic}{datePart}".Trim();
            }
        }

        public override string ToString() => FullName;

       
        public string Age
        {
            get
            {
                if (!BirthDate.HasValue) return "—";

                DateTime endDate = DeathDate ?? DateTime.Today;
                int age = endDate.Year - BirthDate.Value.Year;

                if (BirthDate.Value.Date > endDate.AddYears(-age)) age--;

                return age.ToString();
            }
        }
    }
}