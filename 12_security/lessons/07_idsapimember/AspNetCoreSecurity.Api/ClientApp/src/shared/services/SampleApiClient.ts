import Axios from "axios";
import { ForecastResult, IForecast } from "./ForecastResult";

const getForecasts = async (token?: string): Promise<ForecastResult> => {
  try {
    const response = await Axios.get<IForecast[]>("/api/sampledata/weatherforecasts", { headers: { Authorization: "Bearer " + token } });
    return ForecastResult.fromResult(response.data);
  } catch (e) {
    return ForecastResult.fromError(e);
  }
};

const callMembershipApi = async (token?: string): Promise<any> => {
    try {
        const response = await Axios.get<IForecast[]>("https://localhost:5001/api/v1/memberships", { headers: { Authorization: "Bearer " + token } });
        return response.data;
    } catch (e) {
        return e;
    }
};

export default {
    getForecasts,
    callMembershipApi
};
