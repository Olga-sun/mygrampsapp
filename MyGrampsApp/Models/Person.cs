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
        public string BirthDate { get; set; } = string.Empty; 
        public string? DeathDate { get; set; } 
        public string? Notes { get; set; } 

        // Для фільтрації по користувачу
        public int UserId { get; set; } 
    }
}
