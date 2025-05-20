/* eslint-disable react-refresh/only-export-components */
import {Route, Routes} from "react-router-dom";
import {BrowserRouter as Router} from "react-router-dom";
import {ToastContainer} from "react-toastify";
import {library} from "@fortawesome/fontawesome-svg-core";
import {far} from "@fortawesome/free-regular-svg-icons";
import {fas} from "@fortawesome/free-solid-svg-icons";
import {useIdleTimer} from "react-idle-timer";

import {CLIENT} from "../../config.js";
import {SignInPage} from "./Auth/SignInPage.jsx";
import {EmptyDatatable} from "./common/EmptyDatatable.jsx";
import {DefaultLayout} from "./common/layout/DefaultLayout.jsx";
import {ActivosComponentesPage} from "./Configuracion/ActivosComponentes/ActivosComponentesPage.jsx";
import {CentrosCostesLocalizacionesPage} from "./Configuracion/CentrosCostesLocalizaciones/CentrosCostesLocalizacionesPage.jsx";
import {EmpresasPage} from "./Configuracion/Empresas/EmpresasPage.jsx";
import {MecanismosFalloIncidenciasResolucionesPage} from "./Configuracion/MecanismosFalloIncidenciasResoluciones/MecanismosFalloIncidenciasResolucionesPage.jsx";
import {PlantasPage} from "./Configuracion/Plantas/PlantasPage.jsx";
import {UsersPage} from "./Configuracion/Usuario/UsersPage.jsx";
import {PanelGeneralPage} from "./correctivos/panelGeneral/PanelGeneralPage.jsx";
import {RegistroOrdenesPage} from "./correctivos/registroOrdenes/RegistroOrdenesPage.jsx";
import {InformeOrdenesPage} from "./Informes/InformeOrdenesPage.jsx";
import {EstudioComparativoPage} from "./KPIs/Estudio comparativo/EstudioComparativoPage.jsx";
import {IndicadoresConfiabilidadPage} from "./KPIs/IndicadoresConfiabilidadPage.jsx";
import {IndicadoresOTPage} from "./KPIs/IndicadoresOTPage.jsx";
import {SoportePage} from "./SoportePage.jsx";

//Traducción
import "@/utils/i18n/i18n";
// Estilos
import "./../styles/index.css";
import "@/styles/login.css";
import "./../../node_modules/bootstrap/dist/css/bootstrap.css";
import "@xyflow/react/dist/style.css";
import { useState } from "react";

// eslint-disable-next-line no-unsanitized/method
import(`../styles/${CLIENT}.css`);





library.add(fas, far);

export const IS_DEBUG = true;

const t = (s) => s;

// eslint-disable-next-line react-refresh/only-export-components
export const NavRoutes = {
    Correctivos: {
        path: "/correctivos",
        title: t("Correctivos"),
        navName: t("Correctivos"),
        icon: "fa-solid fa-check-to-slot",
        elemento: "correctivo",
        componente: null,
        subRoutes: {
            PanelGeneral: {
                path: "/correctivos/panelGeneral",
                title: t("Órdenes de Trabajo"),
                navName: t("Panel General"),
                icon: "fa-solid fa-house",
                elemento: "panelGeneral",
                componente: PanelGeneralPage,
            },
            RegistroOrdenes: {
                path: "/correctivos/registroOrdenes",
                title: t("Nueva Orden de Trabajo"),
                navName: t("Registro de Órdenes"),
                icon: "fa-solid fa-plus",
                elemento: "registroOrdenes",
                componente: RegistroOrdenesPage,
            },
        },
    },
    Configuracion: {
        path: "/configuracion",
        title: t("Configuracion"),
        navName: t("Configuración"),
        icon: "fa-solid fa-gear",
        elemento: "configuracion",
        subRoutes: {
            Empresas: {
                path: "/configuracion/empresas",
                title: t("Gestión Empresas"),
                navName: t("Empresas"),
                icon: "fa-solid fa-industry",
                elemento: "empresas",
                componente: EmpresasPage,
            },
            Plantas: {
                path: "/configuracion/plantas",
                title: t("Gestión Plantas"),
                navName: t("Plantas"),
                icon: "fa-solid fa-building",
                elemento: "plantas",
                componente: PlantasPage,
            },
            Usuarios: {
                path: "/configuracion/usuarios",
                title: t("Gestión Usuarios"),
                navName: t("Usuarios"),
                icon: "fa-solid fa-user-group",
                elemento: t("usuarios"),
                componente: UsersPage,
            },
            LocalizacionesCentrosCostes: {
                path: "/configuracion/localizacionesCentrosCostes",
                title: t("Gestión Localizaciones"),
                navName: t("Localizaciones"),
                icon: "fa-solid fa-house-laptop",
                elemento: "localizacionesCentrosCostes",
                componente: CentrosCostesLocalizacionesPage,
            },
            MecanismosFalloIncidenciasResoluciones: {
                path: "/configuracion/mecanismosFalloIncidenciasResoluciones",
                title: t("Gestión Incidencias y Resoluciones"),
                navName: t("Incidencias y Resoluciones"),
                icon: "fa-solid fa-list-check",
                elemento: "mecanismosFalloIncidenciaResoluciones",
                componente: MecanismosFalloIncidenciasResolucionesPage,
            },
            ActivosComponentes: {
                path: "/configuracion/activosComponentes",
                title: t("Gestión Activos y Componentes"),
                navName: t("Activos y Componentes"),
                icon: "fa-solid fa-desktop",
                elemento: "activosComponentes",
                componente: ActivosComponentesPage,
            },
        },
    },
    Informes: {
        path: "/informes",
        title: t("Informes"),
        navName: t("Informes"),
        icon: "fa-solid fa-file",
        elemento: "informe",
        componente: null,
        subRoutes: {
            InformeOrdenes: {
                path: "/informes/informeOrdenes",
                title: t("Informe Órdenes"),
                navName: t("Informe Órdenes"),
                icon: "fa-solid fa-file-contract",
                elemento: "informeOrdenes",
                componente: InformeOrdenesPage,
            },
        },
    },
    Kpis: {
        path: "/kpis",
        title: t("KPIS"),
        navName: t("Kpis"),
        icon: "fa-solid fa-chart-simple",
        elemento: "kpi",
        componente: null,
        subRoutes: {
            IndicadoresOT: {
                path: "/kpis/indicadoresOT",
                title: t("Indicadores OTs"),
                navName: t("Indicadores OTs"),
                icon: "fa-solid fa-chart-line",
                elemento: "indicadorOT",
                componente: InformeOrdenesPage,
            },
            IndicadoresConfiabilidad: {
                path: "/kpis/indicadoresConfiabilidad",
                title: t("Indicadores Confiabilidad"),
                navName: t("Indicadores Confiabilidad"),
                icon: "fa-solid fa-gauge",
                elemento: "indicadorConfiabilidad",
                componente: IndicadoresConfiabilidadPage,
            },
            EstudioComparativo: {
                path: "/kpis/estudioComparativo",
                title: t("Estudio Comparativo"),
                navName: t("Estudio comparativo"),
                icon: "fa-solid fa-magnifying-glass-chart",
                elemento: "estudioComparativo",
                componente: EstudioComparativoPage,
            },
        },
    }
};

//funcion que me devuelve todos los objetos de rutas y subrutas a un mismo nivel, eliminando las rutas principales de las subrutas (estas no tienen navegacion)
function aplanadorNavRoutes(navRoutes) {
    const rutas = {...navRoutes};

    for (const key in navRoutes) {
        if (navRoutes[key].subRoutes) {
            for (const subKey in navRoutes[key].subRoutes) {
                rutas[subKey] = navRoutes[key].subRoutes[subKey];
            }
        }
    }

    return rutas;
}

export const RoutePaths = Object.assign(aplanadorNavRoutes(NavRoutes), {
    Register: {path: "/register", title: "TITLE", navName: "NAV_NAME", icon: "", elemento: ""},
    SignIn: {path: "/", title: "TITLE", navName: "NAV_NAME", icon: "", elemento: ""},
});

const permissions = {
    [t("SUPER_ADMINISTRADOR")]: {whitelist: ["*"]},
    [t("ADMINISTRADOR")]: {blacklist: [RoutePaths.Plantas.navName, RoutePaths.Empresas.navName]},
    [t("JEFE_MANTENIMIENTO")]: {
        blacklist: [RoutePaths.Usuarios.navName, RoutePaths.Plantas.navName, RoutePaths.Empresas.navName],
    },
    [t("RESPONSABLE")]: {
        blacklist: [
            RoutePaths.ActivosComponentes.navName,
            RoutePaths.LocalizacionesCentrosCostes.navName,
            RoutePaths.Usuarios.navName,
            RoutePaths.Plantas.navName,
            RoutePaths.Empresas.navName,
        ],
    },
    [t("RESPONSABLE_MATERIALES")]: {
        blacklist: [
            RoutePaths.ActivosComponentes.navName,
            RoutePaths.LocalizacionesCentrosCostes.navName,
            RoutePaths.Usuarios.navName,
            RoutePaths.Plantas.navName,
            RoutePaths.Empresas.navName,
        ],
    },
    [t("RESPONSABLE_TALLER")]: {
        blacklist: [
            RoutePaths.ActivosComponentes.navName,
            RoutePaths.LocalizacionesCentrosCostes.navName,
            RoutePaths.Usuarios.navName,
            RoutePaths.Plantas.navName,
            RoutePaths.Empresas.navName,
        ],
    },
    [t("OPERARIO")]: {
        blacklist: [RoutePaths.Configuracion.navName, RoutePaths.Informes.navName, RoutePaths.Kpis.navName],
    },
};

export const redirections = {
    [t("SUPER_ADMINISTRADOR")]: RoutePaths.PanelGeneral.path,
    [t("ADMINISTRADOR")]: RoutePaths.PanelGeneral.path,
    [t("JEFE_MANTENIMIENTO")]: RoutePaths.PanelGeneral.path,
    [t("RESPONSABLE")]: RoutePaths.PanelGeneral.path,
    [t("RESPONSABLE_MATERIALES")]: RoutePaths.PanelGeneral.path,
    [t("RESPONSABLE_TALLER")]: RoutePaths.PanelGeneral.path,
    [t("OPERARIO")]: RoutePaths.PanelGeneral.path,
};

export default function App() {

    return (
        <Router>
            <Routes>
                <Route path={RoutePaths.SignIn.path} element={<SignInPage />} />
                {/* PAGINAS CORRECTIVOS */}
                <Route
                    path={RoutePaths.PanelGeneral.path}
                    element={
                        <DefaultLayout
                            component={PanelGeneralPage}
                            datos={RoutePaths.PanelGeneral}
                            claseContenedorPrincipal=""
                        />
                    }
                />
                <Route
                    path={RoutePaths.RegistroOrdenes.path}
                    element={
                        <DefaultLayout
                            component={RegistroOrdenesPage}
                            datos={RoutePaths.RegistroOrdenes}
                            claseContenedorPrincipal="contenedorRegistroOrdenes"
                        />
                    }
                />

                {/* PÁGINAS CONFIGURACIÓN */}
                <Route
                    path={RoutePaths.Empresas.path}
                    element={<DefaultLayout component={EmpresasPage} datos={RoutePaths.Empresas} />}
                />
                <Route
                    path={RoutePaths.Plantas.path}
                    element={<DefaultLayout component={PlantasPage} datos={RoutePaths.Plantas} />}
                />
                <Route
                    path={RoutePaths.Usuarios.path}
                    element={<DefaultLayout component={UsersPage} datos={RoutePaths.Usuarios} />}
                />
                <Route
                    path={RoutePaths.LocalizacionesCentrosCostes.path}
                    element={
                        <DefaultLayout
                            component={CentrosCostesLocalizacionesPage}
                            datos={RoutePaths.LocalizacionesCentrosCostes}
                        />
                    }
                />
                <Route
                    path={RoutePaths.MecanismosFalloIncidenciasResoluciones.path}
                    element={
                        <DefaultLayout
                            component={MecanismosFalloIncidenciasResolucionesPage}
                            datos={RoutePaths.MecanismosFalloIncidenciasResoluciones}
                        />
                    }
                />
                <Route
                    path={RoutePaths.ActivosComponentes.path}
                    element={<DefaultLayout component={ActivosComponentesPage} datos={RoutePaths.ActivosComponentes} />}
                />
                {/* PÁGINAS INFORMES */}
                <Route
                    path={RoutePaths.InformeOrdenes.path}
                    element={
                        <DefaultLayout
                            component={InformeOrdenesPage}
                            datos={RoutePaths.InformeOrdenes}
                            claseContenedorPrincipal=""
                        />
                    }
                />
                {/* PÁGINAS KPIS */}
                <Route
                    path={RoutePaths.IndicadoresOT.path}
                    element={
                        <DefaultLayout
                            component={IndicadoresOTPage}
                            datos={RoutePaths.IndicadoresOT}
                            claseContenedorPrincipal=""
                        />
                    }
                />
                <Route
                    path={RoutePaths.IndicadoresConfiabilidad.path}
                    element={
                        <DefaultLayout
                            component={IndicadoresConfiabilidadPage}
                            datos={RoutePaths.IndicadoresConfiabilidad}
                            claseContenedorPrincipal=""
                        />
                    }
                />
                <Route
                    path={RoutePaths.EstudioComparativo.path}
                    element={
                        <DefaultLayout
                            component={EstudioComparativoPage}
                            datos={RoutePaths.EstudioComparativo}
                            claseContenedorPrincipal=""
                        />
                    }
                />

                {/* PÁGINA SOPORTE */}
                {/* <Route
                    path={RoutePaths.Soporte.path}
                    element={
                        <DefaultLayout
                            mostrarTitulo={false}
                            component={SoportePage}
                            datos={RoutePaths.Soporte}
                            claseContenedorPrincipal=""
                        />
                    }
                /> */}
                {/* PAGINA NO ENCONTRDA */}
                <Route
                    path="*"
                    element={
                        <EmptyDatatable
                            iconName="fa-solid fa-screwdriver-wrench"
                            size={150}
                            height={98}
                            informacion={t("Página no encontrada")}
                            fontSize={20}
                        />
                    }
                />
            </Routes>
            <ToastContainer
                position="top-center"
                autoClose={7000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
                theme="colored"
            />
        </Router>
    );
}

/**
 * Verifica si un usuario con un rol específico tiene permiso para acceder a un elemento de navegación.
 *
 * @param {string} role - El rol del usuario.
 * @param {string} navName - El nombre del elemento de navegación o recurso al que se desea acceder.
 * @returns {boolean} - Devuelve `true` si el usuario tiene permiso para acceder al elemento, de lo contrario `false`.
 *
 * @description
 * La función evalúa los permisos del usuario basándose en las listas de control de acceso (`whitelist` y `blacklist`)
 * definidas para su rol en el objeto `permissions`. La lógica de permisos se aplica de la siguiente manera:
 *
 * - **Si el rol no está definido en `permissions`**:
 *   - Se deniega el acceso (devuelve `false`).
 *
 * - **Si el rol tiene definidas ambas listas (`whitelist` y `blacklist`)**:
 *   - El acceso se permite solo si el `navName` está en la `whitelist` (o si la `whitelist` incluye `"*"`)
 *     **y** no está en la `blacklist`.
 *
 * - **Si solo la `whitelist` está definida y no está vacía**:
 *   - El acceso se permite únicamente a los elementos que están en la `whitelist` o si la `whitelist` incluye `"*"`.
 *
 * - **Si solo la `blacklist` está definida y no está vacía**:
 *   - El acceso se permite a todos los elementos que **no** están en la `blacklist`.
 *
 * - **Si ninguna de las listas está definida o ambas están vacías**:
 *   - Se deniega el acceso por defecto (devuelve `false`).
 *
 * @example
 * // Ejemplo de uso:
 * const permissions = {
 *   admin: {
 *     whitelist: ['dashboard', 'settings', '*'],
 *     blacklist: ['restrictedSection']
 *   },
 *   user: {
 *     blacklist: ['adminPanel']
 *   }
 * };
 *
 * canSee('admin', 'dashboard'); // Devuelve true
 * canSee('admin', 'restrictedSection'); // Devuelve false
 * canSee('user', 'home'); // Devuelve true
 * canSee('user', 'adminPanel'); // Devuelve false
 */
export const canSee = (role, navName) => {
    const rolePermissions = permissions[role];

    if (!rolePermissions) {
        return false;
    }

    const {whitelist, blacklist} = rolePermissions;

    if (whitelist && whitelist.length > 0 && blacklist && blacklist.length > 0) {
        // Si ambas listas están presentes, permitir solo los elementos que están en la whitelist y no están en la blacklist
        return (whitelist.includes(navName) || whitelist.includes("*")) && !blacklist.includes(navName);
    } else if (whitelist && whitelist.length > 0) {
        // Solo whitelist presente
        return whitelist.includes(navName) || whitelist.includes("*");
    } else if (blacklist && blacklist.length > 0) {
        // Solo blacklist presente
        return !blacklist.includes(navName);
    } else {
        // Ninguna lista presente
        return false; // O true, según prefieras
    }
};
