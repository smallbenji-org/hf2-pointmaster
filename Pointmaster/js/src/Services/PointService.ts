import axios, { type AxiosResponse } from "axios";

export default class PointService {
    public async getAll(): Promise<Point[]> {
        try {
            const reponse: AxiosResponse<Point[]> = await axios({
                url: `/api/v1/points`,
                method: "GET"
            });

            return Array.isArray(reponse.data) ? reponse.data : [];
        } catch {
            return [];
        }
    }
}