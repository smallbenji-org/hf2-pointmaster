import axios, { type AxiosResponse } from "axios";
import { getTenantHeaders } from "./tenantHeaders";

export default class StatsService {
    public async points(): Promise<pointStats[]> {
        try {
            const response: AxiosResponse<pointStats[]> = await axios({
                url: `/api/v1/stats/points`,
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

    public async latestMatches(): Promise<Point[]> {
        try {
            const response: AxiosResponse<Point[]> = await axios({
                url: `/api/v1/stats/latestmatches`,
                method: "GET",
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
            });

            return Array.isArray(response.data) ? response.data : []
        } catch {
            return [];
        }
    }
}