import axios, { type AxiosResponse } from "axios";
import { getTenantHeaders } from "./tenantHeaders";

export default class StatsService {
    public async points(): Promise<pointStats[]> {
        try {
            const reponse: AxiosResponse<pointStats[]> = await axios({
                url: `/api/v1/stats/points`,
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
}