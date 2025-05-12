using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<Usuario>(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EstadoUsuario> EstadosUsuario { get; set; }
        public DbSet<Planta> Plantas { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public new DbSet<Rol> Roles { get; set; }
        public DbSet<Localizacion> Localizaciones { get; set; }
        public DbSet<EstadoActivo> EstadosActivo { get; set; }
        public DbSet<Criticidad> Criticidades { get; set; }
        public DbSet<CentroCoste> CentrosDeCostes { get; set; }
        public DbSet<Activo> Activos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<EstadoOrden> EstadosOrden { get; set; }
        public DbSet<EstadoRepuesto> EstadosRepuesto { get; set; }
        public DbSet<Incidencia> Incidencias { get; set; }
        public DbSet<MecanismoDeFallo> MecanismosDeFallo { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<Resolucion> Resoluciones { get; set; }
        public DbSet<TipoOrden> TiposOrden { get; set; }
        public DbSet<Activo_Componente> Activo_Componentes { get; set; }
        public DbSet<Componente> Componentes { get; set; }
        public DbSet<HistorialCambiosUsuarioOrden> HistorialCambiosUsuariosOrdenes { get; set; }
        public DbSet<IncidenciaOrden> IncidenciasOrdenes { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<Usuario_Orden> Usuarios_Ordenes { get; set; }
        public DbSet<InformeExcelDTO> InformeExcelDTO { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()!.Replace("AspNet", ""));
            }

            // Set the default value for Actividad property
            modelBuilder.Entity<Usuario>()
                .Property(u => u.IdEstadoUsuario)
                .HasDefaultValue(1);

            // Set the default value for Confirmado property
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Confirmado)
                .HasDefaultValue(1);

            // Set the default value for EstadoActivo property
            modelBuilder.Entity<Activo>()
                .Property(u => u.IdEstadoActivo)
                .HasDefaultValue(1);

            // Set the foreign key for Ordenes to IncidenciasOrdenes
            modelBuilder.Entity<Orden>()
                .HasMany(o => o.IncidenciasOrden)
                .WithOne(io => io.Orden)
                .HasForeignKey(io => io.IdOrden);
        }
    }
}
