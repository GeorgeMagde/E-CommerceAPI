using System.ComponentModel.DataAnnotations.Schema;

namespace NoobProject.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [ForeignKey("user")]
        public string UserId { get; set; }
        public List<CartEntry> Items { get; set; } = new();
        public AppUser User {  get; set; }
    }
}
