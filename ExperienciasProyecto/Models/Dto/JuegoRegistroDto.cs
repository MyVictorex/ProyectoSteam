namespace WEB_API_JUEGOS.Models.Dto
{
    public class JuegoRegistroDto
    {
        public int ID_JUEGO { get; set; }
        public string NOMBRE { get; set; }
        public string DESCRIPCION { get; set; }
        public decimal PRECIO { get; set; }
        public int ID_CATEGORIA { get; set; }
        public string IMAGEN_URL { get; set; }
        public string VIDEO_URL { get; set; }
        public bool ACTIVO { get; set; } = true; // opcional
    }

}
