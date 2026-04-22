import axios, { type AxiosResponse } from "axios";

export default class StatsService {
    public async points(): Promise<pointStats[]> {
        try {
            const reponse: AxiosResponse<pointStats[]> = await axios({
                url: `/api/v1/stats/points`,
                method: "GET"
            });

            return Array.isArray(reponse.data) ? reponse.data : [];
        } catch {
            return [];
        }
    }
}