namespace GSMAO.Server.DTOs
{
    public class InformeDTO
    {
        public int Id { get; set; }
        public string? IdSAP { get; set; }
        public required string TipoOrden { get; set; }
        public required string EstadoOrden { get; set; }
        public required int IdActivo { get; set; }
        public required string ActivoSAP { get; set; }
        public required string Activo { get; set; }
        public required string Criticidad { get; set; }
        public required string CentroCosteSAP { get; set; }
        public required string CentroCoste { get; set; }
        public required DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public double? TiempoParadaOrden { get; set; }
        public required string Usuarios { get; set; }
        public required string KKSComponente { get; set; }
        public required string Componente { get; set; }
        public required string MecanismoDeFallo { get; set; }
        public required string Incidencia { get; set; }
        public string? Resolucion { get; set; }
        public required DateTime FechaDeteccion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public double? TiempoParadaIncidencia { get; set; }
    }

    public class InformeExcelDTO : InformeDTO
    {
        public required string JerarquiaComponente { get; set; }
        public int NivelesComponente { get; set; }
        public string? ComentarioOrden { get; set; }
        public string? ComentarioResolucion { get; set; }
        public string? Materiales { get; set; }
        public bool CambioPieza { get; set; }
        public bool AfectaProduccion { get; set; }
        public bool ParoMaquina { get; set; }
    }
}
