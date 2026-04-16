using System.ComponentModel.DataAnnotations.Schema;

namespace NoobProject.Models
{
    public class CartEntry
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("Cart")]
        public int CartId {  get; set; }
        public Product Product { get; set; }
        public Cart Cart { get; set; }
        public int Quantity { get; set; }
    }
}
