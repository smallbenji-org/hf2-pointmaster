import axios, { type AxiosResponse } from "axios";
import { getTenantHeaders } from "./tenantHeaders";

export default class PostService {
    public async getAll(): Promise<Post[]> {
        try {
            const reponse: AxiosResponse<Post[]> = await axios({
                url: `/api/v1/post`,
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

    public async addPost(name: string) {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/post`,
                method: "POST",
                data: JSON.stringify(name),
                withCredentials: true,
                headers: {
                    'Content-Type': 'application/json',
                    ...getTenantHeaders()
                }
            });

            if (response.status == 200) {
                return true
            }

            return false;
        } catch {
            return false;
        }
    }

    public async deletePost(id: number) {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/post`,
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