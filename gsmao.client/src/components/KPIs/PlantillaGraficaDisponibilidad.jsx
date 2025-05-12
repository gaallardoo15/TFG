/* eslint-disable react/display-name */
import React, {useState} from "react";
import ApexCharts from "react-apexcharts";
import {useTranslation} from "react-i18next";

export const PlantillaGraficaDisponibilidad = React.memo(({md = 12, titulo, color, data, tipoGrafico, ...rest}) => {
    const {t} = useTranslation();
    const [isLoading, setIsLoading] = useState(true); // Controla el estado de carga

    // Mapeo de datos
    const periodos = data?.map((objeto) => t(objeto.nombrePeriodo)) || [];
    const disponibilidad = data?.map((objeto) => objeto.valor) || [];
    // Configuración de opciones adaptadas
    const options = {
        series: [
            {
                name: titulo,
                data: disponibilidad,
            },
        ],
        chart: {
            height: 200,
            width: "99%",
            type: "area",
            zoom: {
                enabled: false,
            },
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            curve: "smooth",
            width: 2,
        },
        title: {
            text: titulo,
            align: "left",
        },
        grid: {
            row: {
                colors: ["#f3f3f3", "transparent"], // takes an array which will be repeated on columns
                opacity: 0.7,
            },
        },
        xaxis: {
            categories: periodos,
        },
        yaxis: {
            axisBorder: {
                show: false,
            },
            axisTicks: {
                show: false,
            },
            labels: {
                show: true,
            },
            min: 0,
        },
        /*colors: ["var(--graficaCorrectivos)"],*/
        noData: {
            text: "No existen datos",
            align: "center",
            verticalAlign: "middle",
            offsetX: 0,
            offsetY: -5,
            style: {
                color: "#444",
                fontSize: "16px",
            },
        },
    };
    return (
        periodos.length > 0 && (
            <div className={`col-md-${md} containerGraficoKPI`} {...rest}>
                <div className="plantillaGraficaKPI">
                    <ApexCharts options={options} series={options.series} type="area" height={150} width="99%" />
                </div>
            </div>
        )
    );
});
