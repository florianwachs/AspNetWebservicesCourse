export class ForecastResult {
  constructor(public readonly hasErrors: boolean, public readonly error: any, public readonly forecasts: IForecast[]) {}

  static fromResult(forecasts: IForecast[]): ForecastResult {
    return new ForecastResult(false, null, forecasts);
  }

  static fromError(e: any): ForecastResult {
    return new ForecastResult(true, e, []);
  }
}

export interface IForecast {
  dateFormatted: string;
  temperatureC: number;
  summary: string;
}
