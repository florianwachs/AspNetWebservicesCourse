import Axios, { AxiosRequestConfig } from "axios";

const config: AxiosRequestConfig = {
    baseURL: `api/v1`,
};

export const apiClient = Axios.create(config);

export async function isLoggedIn(): Promise<boolean> {
    try {
        const result = await apiClient.get("githubinfo/me");
        return result.data;
    } catch (e) {
        console.log(e);
    }

    return false;
}

export async function getUserInfo(): Promise<IUserInfo> {
    try {
        const result = await apiClient.get<IUserInfo>("githubinfo/me");
        return result.data;
    } catch (e) {
        console.log(e);
    }

    return { login: "Unknown" } as IUserInfo;
}

export interface IUserInfo {
    login: string;
    name: string;
    id: number;
    avatar_url: string;
    company: string;
}
