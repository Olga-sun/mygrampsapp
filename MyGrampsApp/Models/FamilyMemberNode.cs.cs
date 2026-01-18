using System.Collections.ObjectModel;

namespace MyGrampsApp.Models
{
    public class FamilyMemberNode
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public ObservableCollection<FamilyMemberNode> Children { get; set; } = new ObservableCollection<FamilyMemberNode>();
    }
}