namespace ExamenApi.DTOs
{
    public class EmpleadoDTO
    {
        public int IdEmpleado { get; set; }
        public string CodigoEmpleado { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaInicioContrato { get; set; }
        public string Puesto { get; set; } = string.Empty;
    }
} 