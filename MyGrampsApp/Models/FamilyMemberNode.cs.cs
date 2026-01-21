using System.Collections.ObjectModel;

namespace MyGrampsApp.Models
{
    public class FamilyMemberNode
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; } // Додайте, якщо немає
        public string BirthDate { get; set; }  // Додайте, якщо немає

        // Оновіть цей рядок, щоб він включав усі дані
        public string FullName => $"{LastName} {FirstName} {Patronymic} ({BirthDate})".Trim();

        public List<FamilyMemberNode> Children { get; set; } = new List<FamilyMemberNode>();
    }
}
