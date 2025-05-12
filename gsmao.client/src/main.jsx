import {createRoot} from "react-dom/client";

import {CLIENT, CONFIG} from "../config";
import App from "./components/App";

document.title = CONFIG[CLIENT].tituloPrincipal;

// Cambiar favicon dinámicamente
const setLogo = (logoPath) => {
    let link = document.querySelector("link[rel~='icon']");
    if (!link) {
        link = document.createElement("link");
        link.rel = "icon";
        document.head.appendChild(link);
    }
    link.href = logoPath;
};

setLogo(CONFIG[CLIENT].logo);

createRoot(document.getElementById("root")).render(<App />);
