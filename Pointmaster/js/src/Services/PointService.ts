import axios, { type AxiosResponse } from "axios";
import { getTenantHeaders } from "./tenantHeaders";

function getErrorMessage(error: unknown, fallbackMessage: string): string {
    if (axios.isAxiosError(error)) {
        const responseData = error.response?.data as { message?: string; errors?: string[] } | undefined;

        if (responseData?.message) {
            return responseData.message;
        }

        if (Array.isArray(responseData?.errors) && responseData.errors.length > 0) {
            return responseData.errors.join(" ");
        }

        if (error.response?.status === 403) {
            return "You do not have access to this tenant.";
        }

        if (error.response?.status === 404) {
            return "The requested patrulje or post could not be found.";
        }
    }

    return fallbackMessage;
}

function normalizeGivPointOverview(payload: unknown): GivPointOverview | null {
    if (!payload || typeof payload !== "object") {
        return null;
    }

    const data = payload as {
        patrulje?: Patrulje;
        Patrulje?: Patrulje;
        posts?: GivPointOverviewItem[];
        Posts?: GivPointOverviewItem[];
    };

    const patrulje = data.patrulje ?? data.Patrulje;
    const posts = data.posts ?? data.Posts ?? [];

    if (!patrulje) {
        return null;
    }

    return {
        patrulje,
        posts: Array.isArray(posts) ? posts : []
    };
}

export default class PointService {
    public async getAll(): Promise<ApiResult<Point[]>> {
        try {
            const reponse: AxiosResponse<Point[]> = await axios({
                url: `/api/v1/points`,
                method: "GET",
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
            });

            return {
                success: reponse.status === 200,
                message: reponse.status === 200 ? "Points loaded." : "Unable to load points.",
                data: Array.isArray(reponse.data) ? reponse.data : []
            };
        } catch (error) {
            return {
                success: false,
                message: getErrorMessage(error, "Unable to load points."),
                data: []
            };
        }
    }

    public async createPoint(data: PointDTO): Promise<ApiResult<null>> {
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

            return {
                success: response.status === 200,
                message: response.status === 200 ? "Point created." : "Unable to create point.",
                data: null
            };
        } catch (error) {
            return {
                success: false,
                message: getErrorMessage(error, "Unable to create point."),
                data: null
            };
        }
    }

    public async deletePoint(data: number): Promise<ApiResult<null>> {
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

            return {
                success: response.status === 200,
                message: response.status === 200 ? "Point deleted." : "Unable to delete point.",
                data: null
            };
        } catch (error) {
            return {
                success: false,
                message: getErrorMessage(error, "Unable to delete point."),
                data: null
            };
        }
    }

    public async getGivPointOverview(patruljeId: number): Promise<ApiResult<GivPointOverview>> {
        try {
            const response: AxiosResponse<GivPointOverview> = await axios({
                url: `/api/v1/givpoint/${patruljeId}`,
                method: "GET",
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
            });

            const normalized = normalizeGivPointOverview(response.data);
            if (!normalized) {
                return {
                    success: false,
                    message: "Unable to load GivPoint overview.",
                    data: null
                };
            }

            return {
                success: response.status === 200,
                message: response.status === 200 ? "GivPoint overview loaded." : "Unable to load GivPoint overview.",
                data: response.status === 200 ? normalized : null
            };
        } catch (error) {
            return {
                success: false,
                message: getErrorMessage(error, "Unable to load GivPoint overview."),
                data: null
            };
        }
    }

    public async saveGivPoint(patruljeId: number, data: GivPointUpdateDTO): Promise<ApiResult<Point>> {
        try {
            const response: AxiosResponse<{ message?: string; point?: Point }> = await axios({
                url: `/api/v1/givpoint/${patruljeId}`,
                method: "PUT",
                data: data,
                withCredentials: true,
                headers: {
                    "Content-Type": "application/json",
                    ...getTenantHeaders()
                }
            });

            return {
                success: response.status === 200,
                message: response.data?.message ?? (response.status === 200 ? "Point saved." : "Unable to save point."),
                data: response.data?.point ?? null
            };
        } catch (error) {
            return {
                success: false,
                message: getErrorMessage(error, "Unable to save point."),
                data: null
            };
        }
    }
}