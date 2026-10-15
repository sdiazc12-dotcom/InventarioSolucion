namespace Inventario.lab
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }

        public decimal ValorTotal()
        {
            return Precio * Cantidad;
        }

        public override string ToString()
        {
            return $"{Id,-3} {Nombre,-20} {Precio,8} x{Cantidad,-8} = Q {ValorTotal()}";
        }
    }
}
