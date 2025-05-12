import plugin from "@vitejs/plugin-react";
import child_process from "child_process";
import fs from "fs";
import {fileURLToPath, URL} from "node:url";
import path from "path";
import {env} from "process";
import {defineConfig, loadEnv} from "vite";

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== "" ? `${env.APPDATA}/ASP.NET/https` : `${env.HOME}/.aspnet/https`;

const certificateName = "gsmao.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (
        0 !==
        child_process.spawnSync(
            "dotnet",
            ["dev-certs", "https", "--export-path", certFilePath, "--format", "Pem", "--no-password"],
            {stdio: "inherit"},
        ).status
    ) {
        throw new Error("Could not create certificate.");
    }
}

const target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
      ? env.ASPNETCORE_URLS.split(";")[0]
      : "https://localhost:7065";
console.log("Puerto: ", target);

// https://vitejs.dev/config/
export default defineConfig(({mode}) => {
    // Carga el .env desde un directorio superior
    const rootEnv = loadEnv(mode, path.resolve(__dirname, "../GSMAO.Server"), "");
    // rootEnv.CLIENT contendrá 'clientA' (por ejemplo)

    const client = rootEnv.CLIENT || "default";
    console.log(`Compilando para el cliente: ${client}`);

    return {
        plugins: [plugin()],
        define: {client: JSON.stringify(client)},
        resolve: {
            alias: {
                "@": fileURLToPath(new URL("./src", import.meta.url)),
            },
        },
        server: {
            proxy: {
                "^/api": {
                    target,
                    secure: false,
                },
            },
            port: 5173,
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            },
        },
    };
});
