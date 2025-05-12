using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;

namespace GSMAO.Server.Database
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // USUARIOS
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<CreateUserDTO, Usuario>();
            CreateMap<UpdateUsuarioDTO, Usuario>();
            
            // ROLES
            CreateMap<Rol, RolDTO>();

            // ESTADOS USUARIOS
            CreateMap<EstadoUsuario, EstadoUsuarioDTO>();

            // ESTADOS ACTIVOS
            CreateMap<EstadoActivo, EstadoActivoDTO>();

            // EMPRESAS
            CreateMap<Empresa, EmpresaDTO>();
            CreateMap<EmpresaDTO, Empresa>();
            CreateMap<CreateEmpresaDTO, Empresa>();

            // PLANTAS
            CreateMap<Planta, PlantaDTO>();
            CreateMap<PlantaDTO, Planta>();
            CreateMap<CreatePlantaDTO, Planta>();
            CreateMap<UpdatePlantaDTO, Planta>();
            CreateMap<Planta, UserPlantaDTO>();

            // LOCALIZACIONES
            CreateMap<Localizacion, LocalizacionDTO>();
            CreateMap<LocalizacionDTO, Localizacion>();
            CreateMap<CreateLocalizacionDTO, Localizacion>();
            CreateMap<UpdateLocalizacionDTO, Localizacion>();
            CreateMap<Localizacion, LocalizacionTableDTO>();

            // CENTROS DE COSTE
            CreateMap<CentroCoste, CentroCosteDTO>();
            CreateMap<CentroCosteDTO, CentroCoste>();
            CreateMap<CreateCentroCosteDTO, CentroCoste>();
            CreateMap<UpdateCentroCosteDTO, CentroCoste>();
            CreateMap<CentroCoste, CentroCosteTableDTO>();

            // ACTIVOS
            CreateMap<Activo, ActivoDTO>();
            CreateMap<Activo, ActivoMinDTO>();
            CreateMap<CreateActivoDTO, Activo>();
            CreateMap<UpdateActivoDTO, Activo>();
            CreateMap<Activo, ActivoTableDTO>();
            CreateMap<Activo, ActivoOrdenDTO>();

            // MECANISMOS DE FALLOS
            CreateMap<CreateMecanismoDeFalloDTO, MecanismoDeFallo>();
            CreateMap<MecanismoDeFalloDTO, MecanismoDeFallo>();
            CreateMap<MecanismoDeFallo, MecanismoDeFalloDTO>();

            // INCIDENCIAS
            CreateMap<CreateIncidenciaDTO, Incidencia>();
            CreateMap<IncidenciaDTO, Incidencia>();
            CreateMap<Incidencia, IncidenciaDTO>();
            CreateMap<UpdateIncidenciaDTO, Incidencia>();
            CreateMap<CreateIncidenciaOrdenDTO, IncidenciaOrden>();
            CreateMap<UpdateIncidenciaOrdenDTO, IncidenciaOrden>();

            // RESOLUCIONES 
            CreateMap<CreateResolucionDTO, Resolucion>();
            CreateMap<ResolucionDTO, Resolucion>();
            CreateMap<Resolucion, ResolucionDTO>();
            CreateMap<UpdateResolucionIncidenciaOrdenDTO, IncidenciaOrden>();

            // CRITICIDADES
            CreateMap<Criticidad, CriticidadDTO>();

            // COMPONENTES
            CreateMap<Componente, ComponenteDTO>();
            CreateMap<Componente, ComponenteTableDTO>();
            CreateMap<CreateComponenteDTO, Componente>();
            CreateMap<UpdateComponenteDTO, Componente>();
            CreateMap<ComponenteDTO, ComponenteTableDTO>()
                .ForPath(dest => dest.IdComponentePadre, opt => opt.MapFrom(src => src.ComponentePadre != null ? src.ComponentePadre.Id : 0));

            // ACTIVOS_COMPONENTES
            CreateMap<CreateComponenteDTO, Activo_Componente>()
                .ForMember(dest => dest.IdComponente, opt => opt.MapFrom(src => src.IdComponentePadre));

            // ORDENES
            CreateMap<Orden, OrdenDTO>()
                .ForMember(dest => dest.Usuarios, opt => opt.MapFrom(src => src.UsuariosOrden.Select(uo => uo.Usuario)))
                .ForMember(dest => dest.Creador, opt => opt.MapFrom(src => src.Usuario));
            CreateMap<Usuario, UsuarioOrdenDTO>();
            CreateMap<IncidenciaOrden, IncidenciaOrdenDTO>();
            CreateMap<ReasignarUsuarioOrdenDTO, Usuario_Orden>()
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.idUsuarioDestino));
            CreateMap<Usuario_Orden, UsuarioOrdenDTO>();
            CreateMap<UpdateOrdenDTO, Orden>()
                .ForMember(dest => dest.FechaCierre, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.FechaCierre) ? (DateTime?)null : DateTime.Parse(src.FechaCierre)));

            // ESTADOS ORDENES
            CreateMap<EstadoOrden, EstadoOrdenDTO>();

            // TIPOS ORDENES
            CreateMap<TipoOrden, TipoOrdenDTO>();

            // HISTORIAL CAMBIOS USUARIOS ORDEN
            CreateMap<HistorialCambiosUsuarioOrden, RegistroHistorialCambiosOrdenDTO>(); 

            // KPIs
            CreateMap<OrdenesPeriodoAuxDTO, OrdenesPeriodoDTO>();
            CreateMap<ConfiabilidadPeriodoDTO, IndicadoresConfiabilidadPorActivoDTO>()
                .ForPath(dest => dest.Indicadores.TotalOrdenesCerradas, opt => opt.MapFrom(src => src.OrdenesCerradas))
                .ForPath(dest => dest.Indicadores.TotalMTBF, opt => opt.MapFrom(src => src.MTBF))
                .ForPath(dest => dest.Indicadores.TotalMTTR, opt => opt.MapFrom(src => src.MTTR))
                .ForPath(dest => dest.Indicadores.TotalDisponibilidad, opt => opt.MapFrom(src => src.Disponibilidad))
                .ForPath(dest => dest.Indicadores.TotalConfiabilidad, opt => opt.MapFrom(src => src.Confiabilidad));
        }
    }
}
