import {BaseCrudService} from "./BaseCrudService";
import {RestUtilities} from "./RestUtilities";

class UserSevice extends BaseCrudService {
    resetPassword(formdata) {
        return RestUtilities.put(`${this.apiUrl}/${formdata.id}/reset-password`, formdata);
    }

    changeState(id, state) {
        return RestUtilities.put(`${this.apiUrl}/${id}/change-state/${state}`);
    }
}

export const userService = new UserSevice("/api/usuarios", "id");
