namespace GSMAO.Server.DTOs
{
    public class IndicadoresConfiabilidadDTO
    {    
        public int TotalOrdenesCerradas { get; set; }
        public double TotalDisponibilidad { get; set; }
        public double TotalConfiabilidad { get; set; }
        public double TotalMTBF { get; set; }
        public double TotalMTTR { get; set; } 
    }

    public class IndicadoresConfiabilidadPorActivoDTO {
        public int IdActivo { get; set; }
        public required IndicadoresConfiabilidadDTO Indicadores { get; set; }
    }

    public class KPIsConfiabilidadDTO
    {
        public int Year { get; set; }
        public int TotalOrdenesCerradas { get; set; }
        public double TotalDisponibilidad { get; set; }
        public double TotalConfiabilidad { get; set; }
        public double TotalMTBF { get; set; }
        public double TotalMTTR { get; set; } 
        public required List<ConfiabilidadPeriodoDTO> GraficasPeriodos { get; set; }

        public required List<IndicadoresConfiabilidadPorActivoDTO> DesglosePorActivos { get; set; }
    }

    public class KPIsConfiabilidadAuxDTO
    {
        public int Periodo { get; set; }
        public DateTime FechaApertura { get; set; }
        public int IdOrden { get; set; }
        public int IdActivo { get; set; }
        public bool ActivoCritico { get; set; }
        public int IdIncidencia { get; set; }
        public DateTime FechaDeteccion { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }

    public class ConfiabilidadPeriodoDTO
    {
        public int IdActivo { get; set; }
        public int Periodo { get; set; }
        public required string NombrePeriodo { get; set; }
        public required int OrdenesCerradas { get; set; }
        public double MTBF { get; set; }
        public double MTTR { get; set; }
        public double Disponibilidad { get; set; }
        public double Confiabilidad { get; set; }
    }
}
