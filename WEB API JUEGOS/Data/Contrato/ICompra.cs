namespace WEB_API_JUEGOS.Data.Contrato
{
    public interface ICompra
    {

        int RegistrarCompra(int idUsuario, decimal total);
        bool RegistrarDetalle(int idCompra, int idJuego, decimal precio);
    }
}
