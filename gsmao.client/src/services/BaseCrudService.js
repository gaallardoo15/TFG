import {RestUtilities} from "./RestUtilities";
export class BaseCrudService {
    constructor(apiUrl, idKey) {
        this.apiUrl = apiUrl;
        this.idKey = idKey;

        // Bind methods
        this.fetchAll = this.fetchAll.bind(this);
        this.fetch = this.fetch.bind(this);
        this.search = this.search.bind(this);
        this.update = this.update.bind(this);
        this.create = this.create.bind(this);
        this.save = this.save.bind(this);
        this.delete = this.delete.bind(this);
    }

    fetchAll() {
        return RestUtilities.get(this.apiUrl);
    }

    fetch(id) {
        return RestUtilities.get(`${this.apiUrl}/${id}`);
    }

    search(query) {
        return RestUtilities.get(`${this.apiUrl}/search/?q=${query}`);
    }

    update(instance) {
        return RestUtilities.put(`${this.apiUrl}/${instance[this.idKey]}`, instance);
    }

    create(instance) {
        return RestUtilities.post(this.apiUrl, instance);
    }

    save(instance) {
        if (instance[this.idKey]) {
            return this.update(instance);
        } else {
            return this.create(instance);
        }
    }

    delete(id) {
        return RestUtilities.delete(`${this.apiUrl}/${id}`);
    }

    download(id) {
        return RestUtilities.get(`${this.apiUrl}/${id}/download`);
    }
}
