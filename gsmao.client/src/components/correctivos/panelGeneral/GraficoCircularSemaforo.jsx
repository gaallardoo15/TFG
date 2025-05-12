import React from "react";
import ApexCharts from "react-apexcharts";

const defaultLimitesSemaforo = [
    [5, 20],
    [10, 30],
    [5, 20],
    [5, 20],
];

function getValueColor(value = 25, valueLimits = [33, 66], revertDirection = false) {
    const colors = ["var(--semaforoVerde)", "var(--semaforoNaranja)", "var(--semaforoRojo)"];

    if (revertDirection) {
        colors.reverse();
    }

    if (value < valueLimits[0]) {
        return colors[0];
    }
    if (value > valueLimits[1]) {
        return colors[2];
    }
    return colors[1];
}

// eslint-disable-next-line react/display-name
export const GraficoCircularSemaforo = React.memo(
    ({
        series,
        datos,
        discriminantSeries,
        revertDirections = [false, false, false, false],
        limitesSemaforo = defaultLimitesSemaforo,
    }) => {
        const options = {
            chart: {
                type: "radialBar",
            },
            plotOptions: {
                radialBar: {
                    offsetY: 0,
                    startAngle: 0,
                    endAngle: 270,
                    hollow: {
                        size: "5%",
                        image: undefined,
                    },
                    track: {
                        strokeWidth: "80%", // Ancho de las barras radiales (reduce este valor para hacerlas más estrechas)
                        background: "#e3e3e3",
                        margin: 6,
                    },
                    dataLabels: {
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                    barLabels: {
                        enabled: true,
                        useSeriesColors: true,
                        margin: 8,
                        offsetX: -10,
                        fontSize: "12px",
                        formatter: function (seriesName, opts) {
                            return seriesName + ":  " + datos[opts.seriesIndex].nOrdenesOpcion;
                        },
                    },
                },
            },
            colors: limitesSemaforo.map((limitesParaEstaSerie, index) => {
                return ({value, seriesIndex, w}) => {
                    return getValueColor(
                        discriminantSeries[seriesIndex],
                        limitesParaEstaSerie,
                        revertDirections[index],
                    );
                };
            }),

            stroke: {
                lineCap: "round",
            },
            labels: [
                datos[0]?.descripcionOpcion,
                datos[1]?.descripcionOpcion,
                datos[2]?.descripcionOpcion,
                datos[3]?.descripcionOpcion == "Falla Humana" ? "F.Humana" : datos[3]?.descripcionOpcion,
            ],
        };

        return (
            <div id="contenedorGraficaCircularResponsive" style={{maxWidth: "110%"}}>
                <ApexCharts options={options} series={series} type="radialBar" height={200} />
            </div>
        );
    },
);
