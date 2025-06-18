using System;

namespace ExamenApi.Models
{
    public class Tarea
    {
        public int IdTarea { get; set; }
        public string NombreTarea { get; set; } = string.Empty;
        public DateTime? FechaInicioPlan { get; set; }
        public DateTime? FechaFinPlan { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdRecurso { get; set; }
        public Empleado? Recurso { get; set; }
        public int? IdPadre { get; set; }
        public Tarea? TareaPadre { get; set; }
        public string Predecesora { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int? Progreso { get; set; }
    }
} 