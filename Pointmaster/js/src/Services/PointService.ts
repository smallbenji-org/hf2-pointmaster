import axios, { type AxiosResponse } from "axios";
import { getTenantHeaders } from "./tenantHeaders";

export default class PointService {
    public async getAll(): Promise<Point[]> {
        try {
            const reponse: AxiosResponse<Point[]> = await axios({
                url: `/api/v1/points`,
                method: "GET",
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
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
                withCredentials: true,
                headers: {
                    "Content-Type": "application/json",
                    ...getTenantHeaders()
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

    public async deletePoint(data: number): Promise<boolean> {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/points`,
                method: "DELETE",
                data: JSON.stringify(data),
                withCredentials: true,
                headers: {
                    "Content-Type": "application/json",
                    ...getTenantHeaders()
                }
            });

            if (response.status == 200) {
                return true;
            }

            return false;
        } catch {
            return false;
        }
    }
}