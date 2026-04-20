import axios, { type AxiosResponse } from "axios";

export default class PatruljeService {
    public async getAll(): Promise<Patrulje[]> {
        try {
            const response: AxiosResponse<Patrulje[]> = await axios({
                url: `/api/v1/patrulje`,
                method: "GET",
            });

            return Array.isArray(response.data) ? response.data : [];
        } catch {
            return [];
        }
    }

    public async addPatrulje(name: string): Promise<boolean> {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/patrulje`,
                method: "POST",
                data: JSON.stringify(name),
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (response.status == 200)
                return true;
            return false;
        } catch {
            return false;
        }
    }
}