using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyGrampsApp.Models
{
    public class FamilyMemberNode
    {
        public int Id { get; set; }

        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }

        public string? BirthDate { get; set; }

        
        public string FullName
        {
            get
            {
                // Якщо дата порожня, виводимо прочерк або "Невідомо"
                string dateDisplay = string.IsNullOrWhiteSpace(BirthDate) ? "—" : BirthDate;
                return $"{LastName} {FirstName} {Patronymic} ({dateDisplay})".Trim();
            }
        }

        public List<FamilyMemberNode> Children { get; set; } = new List<FamilyMemberNode>();
    }
}