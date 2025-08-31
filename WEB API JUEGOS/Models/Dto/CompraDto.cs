using System.Collections.Generic;

namespace WEB_API_JUEGOS.Models.Dto
{
    public class CompraDto
    {

        public int ID_USUARIO { get; set; }
        public decimal TOTAL { get; set; }
        public List<DetalleCompraDto> DetallesCompra { get; set; }
    }
}
