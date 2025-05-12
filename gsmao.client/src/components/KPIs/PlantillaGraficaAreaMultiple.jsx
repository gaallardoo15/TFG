import React from "react";
import ApexCharts from "react-apexcharts";
import {useTranslation} from "react-i18next";

// eslint-disable-next-line react/display-name
export const PlantillaGraficaAreaMultiple = React.memo(({md = 12, titulo, series, data, ...rest}) => {
    const {t} = useTranslation();

    // Mapear los períodos de la data para las categorías del eje X
    const periodos =
        data.length > 0 &&
        (data[0].nombrePeriodo
            ? data.map((objeto) => objeto.nombrePeriodo) || []
            : data
                  .map((anio) => anio.datosGrafico.map((item) => item.nombrePeriodo)) // Mapea todos los arrays internos
                  .reduce(
                      (maxArray, currentArray) => (currentArray.length > maxArray.length ? currentArray : maxArray),
                      [],
                  )); // Selecciona el de mayor tamaño

    // Configuración del gráfico
    const options = {
        chart: {
            type: "area",
            height: 200,
            zoom: {
                enabled: false,
            },
            animations: {
                enabled: true, // Habilita las animaciones
                easing: "easeinout", // Puedes cambiar el tipo de easing según lo que prefieras
                speed: 1500, // Velocidad de la animación (en milisegundos)
            },
        },
        title: {
            text: t(titulo),
            align: "left",
        },
        xaxis: {
            categories: periodos,
            labels: {
                style: {
                    fontSize: "12px",
                },
            },
        },
        yaxis: {
            labels: {
                style: {
                    fontSize: "12px",
                },
            },
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            curve: "straight",
        },
        fill: {
            type: "gradient",
            gradient: {
                shadeIntensity: 1,
                opacityFrom: 0.2,
                opacityTo: 0.2,
                stops: [65, 100],
            },
        },
        colors: [
            "var(--graficaCorrectivos)",
            "var(--graficaPreventivos)",
            "var(--graficaMejora)",
            "var(--graficaFiabilidadHumana)",
        ],
        grid: {
            row: {
                colors: ["#f3f3f3", "transparent"],
                opacity: 0.5,
            },
        },
        noData: {
            text: t("No existen datos en este período"),
            align: "center",
            verticalAlign: "middle",
            style: {
                color: "#444",
                fontSize: "16px",
            },
        },
    };

    return (
        <div className={`containerGraficoKPI col-md-${md}  mt-0`} {...rest}>
            <div className="plantillaGraficaKPI" style={{width: "100%", height: 230}}>
                <ApexCharts options={options} series={series} type="area" height={200} width="99%" />
            </div>
        </div>
    );
});
