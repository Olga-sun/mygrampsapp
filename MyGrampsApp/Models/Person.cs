using System;
using System.Collections.Generic;
using System.Text;

namespace MyGrampsApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Occupation { get; set; }

        // Допоміжна властивість для виводу ПІБ одним рядком
        public string FullName => $"{FirstName} {LastName}";
    }
}
