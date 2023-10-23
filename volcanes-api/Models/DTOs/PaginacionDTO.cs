namespace volcanes_api.Models.DTOs;

public class PaginacionDTO
{
    public int Pagina { get; set; } = 1;

    private int registrosPorPagina = 10;

    private int RegistrosMaximos { get; set; } = 15;

    public int RegistrosPorPagina
    {
        get
        {
            return registrosPorPagina;
        }
        set
        {
            registrosPorPagina =  (value > RegistrosMaximos) ? RegistrosMaximos : value;
        }
    }
}