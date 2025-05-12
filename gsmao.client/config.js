export const CLIENT = client;

export const CONFIG = {
    hitachi: {
        tituloPrincipal: "GSMAO HITACHI",
        logo: "/LogoHitachi_Reduc.png",
        imagenLogin: "",
        logicaDelNegocio: {
            RolesObligadosAAdjuntarFotoAlCerrarOrden: [
                "OPERARIO",
                "RESPONSABLE",
                "RESPONSABLE_TALLER",
                "RESPONSABLE_MATERIALES",
                "JEFE_MANTENIMIENTO",
            ],
            RolesObligadosAAdjuntarFotoMaterialAlCerrarOrden: [
                "OPERARIO",
                "RESPONSABLE",
                "RESPONSABLE_TALLER",
                "RESPONSABLE_MATERIALES",
                "JEFE_MANTENIMIENTO",
            ],
            UsuariosPermitidosModificarFechasOrdenCerrada: ["acastro@hitachi.com"],
        },
    },
    suitelec: {
        tituloPrincipal: "GSMAO",
        logo: "/LogoSuitelec_Reduc.ico",
        imagenLogin: "",
        logicaDelNegocio: {
            RolesObligadosAAdjuntarFotoAlCerrarOrden: ["OPERARIO"],
            RolesObligadosAAdjuntarFotoMaterialAlCerrarOrden: ["OPERARIO"],
            UsuariosPermitidosModificarFechasOrdenCerrada: [],
        },
    },
};
