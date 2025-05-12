import {useEffect, useState} from "react";

import {SignInForm} from "./SignInForm";

import {BaseModal} from "@/components/common/modales/BaseModal";
import {parseJwt} from "@/utils/parseJwt";
import {AuthStore} from "@/utils/stores/Auth";

export function SingInModal({isIdle = false}) {
    const [showModal, setShowModal] = useState(false);

    const token = AuthStore.getToken(); // Obtener el token
    useEffect(() => {
        if (token) {
            const decodedToken = parseJwt(token); // Decodificar el token
            const expirationTime = decodedToken.exp * 1000; // Convertir a milisegundos
            const timeUntilModal = expirationTime - Date.now() - 10000; // Calcular el tiempo hasta mostrar el modal

            // Comprobar si el tiempo hasta mostrar el modal ya pasó
            if (timeUntilModal < 0) {
                setShowModal(true); // Si ya pasó, mostrar el modal inmediatamente

                return; // Y no configura un timeout
            }
            const timer = setTimeout(() => {
                if (AuthStore.getToken()) {
                    // Comprobar si el token sigue existiendo
                    setShowModal(true); // Mostrar el modal 10 segundos antes de la expiración
                }
            }, timeUntilModal);

            return () => {
                clearTimeout(timer);
            }; // Limpiar el timer al desmontar o al re-renderizar
        }
    }, [token]); // Este efecto se ejecuta solo una vez al montar el componente o cuando cambie el token

    useEffect(() => {
        if (isIdle && AuthStore.getToken()) {
            // Eliminar el token del localStorage
            setShowModal(true); // Mostrar el modal si el estado es idle
            return;
        }
    }, [isIdle]); // Este efecto se ejecuta solo una vez al montar el componente o cuando cambie el estado de idle

    const handleLogin = () => {
        setShowModal(false);
    };

    return (
        <BaseModal show={showModal} size="xl">
            <SignInForm onLogin={handleLogin} formClassName="bodyLoginModal" modalBody={true} />
        </BaseModal>
    );
}
