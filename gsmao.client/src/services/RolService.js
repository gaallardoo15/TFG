import {BaseCrudService} from "./BaseCrudService";

export const rolesService = new BaseCrudService("/api/roles", "id");
