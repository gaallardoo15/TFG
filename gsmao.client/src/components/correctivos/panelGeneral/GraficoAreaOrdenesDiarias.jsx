import ApexCharts from "react-apexcharts";
import {useTranslation} from "react-i18next";

export const GraficoAreaOrdenesDiarias = ({data}) => {
    const {t} = useTranslation();
    // let nOrdenes = data?.map(function (objeto) {
    //     return objeto.numeroOrdenesTotales;
    // });

    // Mapeo de series y categorías a partir de los datos
    const series = [
        {
            name: t("Órdenes"),
            data: data?.map((item) => [new Date(item.fecha).getTime(), item.numeroOrdenesTotales]),
            color: "#1c791cc2",
        },
    ];

    const options = {
        chart: {
            type: "area",
            height: 180,
            zoom: {
                enabled: false,
            },
        },
        title: {
            text: t("Número de Órdenes Diarias"),
            align: "left",
        },
        xaxis: {
            type: "datetime", // Cambiado a 'datetime'
            labels: {
                formatter: function (value) {
                    const date = new Date(value);
                    const day = date.getDate().toString().padStart(2, "0");
                    const month = (date.getMonth() + 1).toString().padStart(2, "0");
                    const year = date.getFullYear();
                    return `${day}/${month}/${year}`;
                },
            },
            tooltip: {
                enabled: false,
            },
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            curve: "straight",
            width: 2,
        },
        fill: {
            type: "gradient",
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.7,
                opacityTo: 1,
                stops: [65, 100, 100],
            },
        },
        tooltip: {
            custom: function ({series, seriesIndex, dataPointIndex, w}) {
                let dato = series[seriesIndex][dataPointIndex];
                let fechaString = data[dataPointIndex].fecha;
                let fechaDeteccion = new Date(fechaString);

                // Obtener día, mes y año
                let dia = ("0" + fechaDeteccion.getDate()).slice(-2);
                let mes = ("0" + (fechaDeteccion.getMonth() + 1)).slice(-2);
                let anio = fechaDeteccion.getFullYear(); // Agregar ceros a la izquierda si es necesario

                // Formatear la fecha en formato "0001-01-01"
                fechaDeteccion = dia + "-" + mes + "-" + anio;

                let retorno = '<div class="tooltipGraficaOrdenesPanelGeneral">';
                retorno +=
                    '<span class=""><b>' +
                    fechaDeteccion +
                    ": </b>" +
                    data[dataPointIndex].numeroOrdenesTotales +
                    " OTs</span>";
                retorno +=
                    '<span class="text-center badge rounded-pill" style="background-color:var(--estadoEnCurso); letter-Spacing:1px;"> En Curso: ' +
                    data[dataPointIndex].nOrdenesEnCurso +
                    "</span>";
                retorno +=
                    '<span class="text-center badge rounded-pill" style="background-color:var(--estadoAbierta); letter-Spacing:1px;"> Abiertas: ' +
                    data[dataPointIndex].nOrdenesAbiertas +
                    "</span>";
                retorno +=
                    '<span class="text-center badge rounded-pill" style="background-color:var(--estadoEsperandoMaterial); letter-Spacing:1px;"> Material: ' +
                    data[dataPointIndex].nOrdenesMaterial +
                    "</span>";
                retorno +=
                    '<span class="text-center badge rounded-pill" style="background-color:var(--estadoCerrada); letter-Spacing:1px;"> Cerradas: ' +
                    data[dataPointIndex].nOrdenesCerradas +
                    "</span>";
                retorno +=
                    '<span class="text-center badge rounded-pill" style="background-color:var(--estadoAnulada); letter-Spacing:1px;"> Anuladas: ' +
                    data[dataPointIndex].nOrdenesAnuladas +
                    "</span>";

                return retorno + "</div>";
            },
        },
    };

    return (
        <div style={{width: "100%", height: 200}}>
            <ApexCharts options={options} series={series} type="area" height={200} width="100%" />
        </div>
    );
};
