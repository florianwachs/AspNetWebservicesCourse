import Axios from "axios";
import { ForecastResult, IForecast } from "./ForecastResult";

const getForecasts = async (token?: string): Promise<ForecastResult> => {
  try {
    const response = await Axios.get<IForecast[]>("https://localhost:44387/api/sampledata/weatherforecasts", { headers: { Authorization: "Bearer " + token } });
    return ForecastResult.fromResult(response.data);
  } catch (e) {
    return ForecastResult.fromError(e);
  }
};

export default {
  getForecasts: getForecasts
};
