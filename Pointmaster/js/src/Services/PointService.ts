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

    public async createPoint(data: PointDTO): Promise<boolean> {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/points`,
                method: "POST",
                data: JSON.stringify(data),
                headers: {
                    "Content-Type": "application/json"
                }
            });

            if (response.status == 200){
                return true
            }

            return false;
        } catch {
            return false;
        }
    }
}