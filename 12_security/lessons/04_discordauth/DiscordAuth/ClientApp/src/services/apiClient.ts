import Axios, { AxiosRequestConfig } from "axios";

const config: AxiosRequestConfig = {
  baseURL: `api/v1`,
};

export const apiClient = Axios.create(config);

export async function isLoggedIn(): Promise<boolean> {
  try {
    const result = await apiClient.get("discordinfo/me");
    return result.data;
  } catch (e) {
    console.log(e);
  }

  return false;
}

export async function getUserInfo(): Promise<IUserInfo> {
  try {
    const result = await apiClient.get<IUserInfo>("discordinfo/me");
    return result.data;
  } catch (e) {
    console.log(e);
  }

  return { username: "Unknown" } as IUserInfo;
}

export interface IUserInfo {
  username: string;
  avatar: string;
  guilds: IGuild[];
}

export interface IGuild {
  id: string;
  name: string;
  icon: string;
  owner: boolean;
  permissions: number;
}
