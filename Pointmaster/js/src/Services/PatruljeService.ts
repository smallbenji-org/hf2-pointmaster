import axios, { type AxiosResponse } from "axios";

export default class PatruljeService {
    public async getAll(): Promise<Patrulje[]> {
        try {
            const response: AxiosResponse<Patrulje[]> = await axios({
                url: `/api/v1/patrulje`,
                method: "GET",
            });

            return response.data;
        } catch {
            return [];
        }
    }
}