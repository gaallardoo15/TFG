import {useState} from "react";

export const useModalState = (fetchData) => {
    const [modalState, setModalState] = useState({
        show: false,
        type: null, // 'form', 'bulkForm', 'delete', 'detail', etc.
        target: {},
    });

    const openModal = (type, target = {}) => {
        setModalState({show: true, type, target});
    };

    const closeModal = ({shouldRefetch = false} = {}) => {
        setModalState({show: false, type: null, target: {}});
        if (shouldRefetch && fetchData) {
            fetchData(); // Refresca los datos si es necesario
        }
    };

    return {modalState, openModal, closeModal};
};
