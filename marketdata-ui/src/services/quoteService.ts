import api from './api';

export interface QuoteInput {
  instrumentType: string;
  tenor: string;
  value: string;
}

export interface QuoteRequest {
  curveName: string;
  curveDate: string;
  quotes: QuoteInput[];
}

export interface QuoteOutput {
  quoteId: string;
  instrumentId: string;
  instrumentType: string;
  tenor: string;
  value: string;
  createdAt: string;
  updatedAt: string;
}

export interface QuoteResponse {
  curveId: string;
  curveName: string;
  curveDate: string;
  quotes: QuoteOutput[];
}

export interface RollRequest {
  curveName: string;
  targetDate: string;
  overwrite?: boolean;
}

export interface RollResponse {
  sourceCurveId: string;
  sourceDate: string;
  targetCurveId: string;
  targetDate: string;
  instrumentCount: number;
  quotes: QuoteOutput[];
  message: string;
}

/**
 * Save quotes for a curve
 */
export const saveQuotes = async (request: QuoteRequest): Promise<QuoteResponse> => {
  const response = await api.post<QuoteResponse>('/quotes', request);
  return response.data;
};

/**
 * Get quotes for a curve by name and date
 */
export const getQuotes = async (curveName: string, curveDate: string): Promise<QuoteResponse | null> => {
  try {
    const response = await api.get<QuoteResponse>('/quotes', {
      params: { curveName, curveDate }
    });
    return response.data;
  } catch (error: any) {
    if (error.response?.status === 404) {
      return null;
    }
    throw error;
  }
};

/**
 * Roll curve and quotes from previous date to target date
 */
export const rollQuotes = async (request: RollRequest): Promise<RollResponse> => {
  const response = await api.post<RollResponse>('/quotes/roll', request);
  return response.data;
};
