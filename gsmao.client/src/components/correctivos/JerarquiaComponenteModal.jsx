import {useCallback, useEffect, useLayoutEffect, useState} from "react";
import {Button, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";
import {addEdge, Controls, ReactFlow, useEdgesState, useNodesState} from "@xyflow/react";

import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {useFetchListData} from "@/hooks/useFetchData";
import {ordenService} from "@/services/OrdenService";

const calcularAnchoNodo = (label) => {
    // Calcula el ancho en base al número de caracteres del label
    const baseWidth = 120; // Ancho base mínimo
    const widthPorCaracter = 10; // Ancho estimado por cada caracter

    // Devuelve el ancho total, garantizando que sea al menos baseWidth
    return Math.max(baseWidth, label.length * widthPorCaracter);
};

const construirNodosYEdges = (datos, formdata) => {
    const nodes = [];
    const edges = [];

    const activoLabel = `Activo: ${formdata.activo.id} - ${formdata.activo.descripcionES}`;
    const activoWidth = calcularAnchoNodo(activoLabel);
    // Nodo principal del activo
    nodes.push({
        id: String(formdata.idActivo),
        position: {x: 300 - activoWidth / 1.5, y: -100}, // Centrado arriba
        data: {label: activoLabel},
        style: {
            backgroundColor: "#FFDDC1",
            border: "1px solid #FF7F50",
            borderRadius: "40px",
            color: "#333",
            fontSize: "14px",
            fontWeight: "bolder",
            width: activoWidth, // Controla la anchura
        },
    });

    // Conexiones del activo
    datos.forEach((item, index) => {
        // Posición horizontal por fila
        const row = Math.floor(index / 2); // Determina la fila
        const col = index % 2; // Determina la columna
        const x = col * 300; // Espaciado horizontal
        const y = row * 100; // Espaciado vertical (100px entre filas)

        // Determinar si es el último nodo del array
        const esUltimoNodo = index === datos.length - 1;

        const nodo = {
            id: String(item.id),
            position: {x, y}, // Distribución en una cuadrícula
            data: {
                label: `${item.denominacion} - ${item.descripcionES}`,
            },
            style: esUltimoNodo
                ? {backgroundColor: "#D1E8FF", border: "1px solid #007BFF", color: "#333"} // Estilo para el último nivel
                : undefined, // Estilo normal
        };
        nodes.push(nodo);

        // Crear conexión (edge) desde el padre o desde el activo principal si no tiene padre
        const edge = {
            id: `${item.idComponentePadre || formdata.idActivo}-${item.id}`,
            source: String(item.idComponentePadre || formdata.idActivo),
            target: String(item.id),
        };
        edges.push(edge);
    });

    return {nodes, edges};
};

// Uso del componente actualizado
export const JerarquiaComponenteModal = ({show: showModal, onClose: handleClose, componente, formdata}) => {
    const {t} = useTranslation();
    const {
        items: datosJerarquia,
        isLoading,
        isRefetching,
        fetchData,
    } = useFetchListData(() => ordenService.getJerarquiaComponente(componente.id));
    const [errors, setErrors] = useState({});

    const [nodes, setNodes, onNodesChange] = useNodesState([]);
    const [edges, setEdges, onEdgesChange] = useEdgesState([]);

    useLayoutEffect(() => {
        if (!isLoading && datosJerarquia.length > 0 && formdata) {
            const {nodes: newNodes, edges: newEdges} = construirNodosYEdges(datosJerarquia, formdata);
            setNodes(newNodes);
            setEdges(newEdges);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [datosJerarquia, isLoading, formdata]);

    useEffect(() => {
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal]);

    const onConnect = useCallback((params) => setEdges((eds) => addEdge(params, eds)), [setEdges]);
    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading} size="lg">
            <CustomModalHeader title={t(`Jerarquía de ${componente.denominacion} - ${componente.descripcionES}`)} />

            <Modal.Body style={{minHeight: "35vh"}}>
                <div style={{width: "765px", height: "40vh"}}>
                    <ReactFlow
                        nodes={nodes}
                        edges={edges}
                        onNodesChange={onNodesChange}
                        onEdgesChange={onEdgesChange}
                        onConnect={onConnect}
                        fitView>
                        <Controls />
                    </ReactFlow>
                </div>
            </Modal.Body>

            <Modal.Footer>
                <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={isLoading}>
                    {t("Cerrar")}
                </Button>
            </Modal.Footer>
        </BaseModal>
    );
};
