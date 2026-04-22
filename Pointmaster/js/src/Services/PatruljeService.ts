import axios, { type AxiosResponse } from "axios";
import { getTenantHeaders } from "./tenantHeaders";

export default class PatruljeService {
    public async getAll(): Promise<Patrulje[]> {
        try {
            const response: AxiosResponse<Patrulje[]> = await axios({
                url: `/api/v1/patrulje`,
                method: "GET",
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
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
                withCredentials: true,
                headers: {
                    'Content-Type': 'application/json',
                    ...getTenantHeaders()
                }
            });

            if (response.status == 200)
                return true;
            return false;
        } catch {
            return false;
        }
    }

    public async deletePatrulje(id: number): Promise<boolean> {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/patrulje`,
                method: "DELETE",
                data: JSON.stringify(id),
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