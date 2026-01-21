using System;
using System.Collections.Generic;
using System.Text;

namespace MyGrampsApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? Notes { get; set; }
        public int UserId { get; set; }
        public int? BirthPlaceId { get; set; }
        public string BirthPlaceName { get; set; }
        public string? MaidenName { get; set; }
        public string FullName => $"{LastName} {FirstName} {Patronymic} ({BirthDate})".Trim();
        public override string ToString() => FullName;
    }
}
