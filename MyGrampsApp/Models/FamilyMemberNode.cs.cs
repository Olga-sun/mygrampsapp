using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyGrampsApp.Models
{
    public class FamilyMemberNode
    {
        public int Id { get; set; }

        // Додаємо ?, щоб прибрати CS8612
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }

        // Оскільки в Person дата — це DateTime?, тут теж краще використовувати DateTime?
        // або залишити string?, якщо це просто для відображення
        public string? BirthDate { get; set; }

        // Оновлена логіка відображення
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