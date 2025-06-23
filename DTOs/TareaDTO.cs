using System;

namespace ExamenApi.DTOs
{
    public class TareaDTO
    {
        public int IdTarea { get; set; }
        public string NombreTarea { get; set; } = string.Empty;
        public DateTime? FechaInicioPlan { get; set; }
        public DateTime? FechaFinPlan { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdRecurso { get; set; }
        public string RecursoNombre { get; set; } = string.Empty;
        public int? IdPadre { get; set; }
        public int? Predecesora { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int? Progreso { get; set; }
    }
} 