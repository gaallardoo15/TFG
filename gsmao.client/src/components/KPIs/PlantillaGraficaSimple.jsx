/* eslint-disable react/display-name */
import React, {useState} from "react";
import ApexCharts from "react-apexcharts";
import {useTranslation} from "react-i18next";

export const PlantillaGraficaSimple = React.memo(
    ({md = 12, titulo, color, data, tipoGrafico = "bar", unidad = "%", tooltip = true, ...rest}) => {
        const {t} = useTranslation();
        const [isLoading, setIsLoading] = useState(true); // Controla el estado de carga
        // Mapeo de datos
        const periodos = data?.map((objeto) => objeto.nombrePeriodo);
        const porcentajesOrdenes = data?.map((objeto) =>
            objeto.porcentajeOrdenes ? Math.round(objeto.porcentajeOrdenes * 100) / 100 : objeto?.valor || 0,
        );
        const activosOrdenes = tooltip ? data?.map((objeto) => objeto.ordenesPorActivo) : [];

        // Configuración de opciones adaptadas
        const options = {
            series: [
                {
                    name: t("OT's"),
                    data: porcentajesOrdenes,
                },
            ],
            chart: {
                height: 230,
                width: "99%",
                type: tipoGrafico,
                animations: {
                    enabled: true, // Habilita las animaciones
                    easing: "easeinout", // Puedes cambiar el tipo de easing según lo que prefieras
                    speed: 1500, // Velocidad de la animación (en milisegundos)
                },
            },
            plotOptions: {
                bar: {
                    borderRadius: 4,
                    dataLabels: {
                        position: "top",
                    },
                },
            },
            dataLabels: {
                enabled: true,
                formatter: function (val) {
                    return val + unidad;
                },
                offsetY: -20,
                style: {
                    fontSize: "12px",
                    colors: ["#304758"],
                },
            },
            xaxis: {
                categories: periodos,
                position: "bottom",
                axisBorder: {
                    show: false,
                },
                axisTicks: {
                    show: false,
                },
                crosshairs: {
                    fill: {
                        type: "gradient",
                        gradient: {
                            colorFrom: "#D8E3F0",
                            colorTo: "#BED1E6",
                            stops: [0, 100],
                            opacityFrom: 0.4,
                            opacityTo: 0.5,
                        },
                    },
                },
                tooltip: {
                    enabled: false,
                },
            },
            yaxis: {
                axisBorder: {
                    show: false,
                },
                axisTicks: {
                    show: false,
                },
                labels: {
                    show: false,
                    formatter: function (val) {
                        return val + unidad;
                    },
                },
            },
            title: {
                text: t(titulo),
                floating: true,
                offsetY: 0,
                align: "left",
                style: {
                    color: "#444",
                },
            },

            tooltip: tooltip
                ? {
                      enabled: true,
                      custom: function ({dataPointIndex}) {
                          let retorno =
                              '<div class="p-2 bg-light">' +
                              "<p><b>OTs: </b>" +
                              data[dataPointIndex].numOrdenes +
                              " Órdenes</p>";
                          for (let i = 0; i < activosOrdenes[dataPointIndex].length; i++) {
                              retorno +=
                                  "<p><b>Activo " +
                                  activosOrdenes[dataPointIndex][i].idActivo +
                                  "</b>: " +
                                  activosOrdenes[dataPointIndex][i].numOrdenesActivo +
                                  " Órdenes</p>";
                          }
                          return retorno + "</div>";
                      },
                  }
                : {
                      enabled: true,
                  },
            noData: {
                text: t("No existen datos en este período"),
                align: "center",
                verticalAlign: "middle",
                offsetX: 0,
                offsetY: -25,
                style: {
                    color: "#444",
                    fontSize: "16px",
                },
            },
            colors: [color],
        };

        return (
            <div className={`col-md-${md} containerGraficoKPI`} {...rest}>
                <div className="plantillaGraficaKPI">
                    <ApexCharts
                        options={options}
                        series={options.series}
                        type={tipoGrafico}
                        height={200}
                        width="100%"
                    />
                </div>
            </div>
        );
    },
);
