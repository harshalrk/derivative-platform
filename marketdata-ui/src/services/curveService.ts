import api from './api';

/**
 * Curve API service for frontend.
 * Provides methods to interact with curve endpoints.
 */

export interface InstrumentInput {
  type: string;
  tenor: string;
}

export interface CurveRequest {
  name: string;
  date: string; // ISO date string (YYYY-MM-DD)
  currency: string;
  index: string;
  instruments: InstrumentInput[];
}

export interface InstrumentOutput {
  id: string;
  type: string;
  tenor: string;
}

export interface CurveResponse {
  id: string;
  name: string;
  curveDate: string; // ISO instant string
  currency: string;
  index: string;
  createdAt: string;
  instruments: InstrumentOutput[];
}

export interface CurveSummary {
  id: string;
  name: string;
  curveDate: string;
  currency: string;
  index: string;
  instrumentCount: number;
  createdAt: string;
}

export interface ReferenceDataItem {
  code?: string;
  name?: string;
  type?: string;
  description: string;
}

/**
 * Create a new curve.
 */
export async function createCurve(request: CurveRequest): Promise<CurveResponse> {
  const response = await api.post<CurveResponse>('/curves', request);
  return response.data;
}

/**
 * Get a curve by name and date.
 */
export async function getCurveByNameAndDate(name: string, date: string): Promise<CurveResponse> {
  const response = await api.get<CurveResponse>('/curves/query', {
    params: { name, date },
  });
  return response.data;
}

/**
 * Get a curve by ID.
 */
export async function getCurveById(id: string): Promise<CurveResponse> {
  const response = await api.get<CurveResponse>(`/curves/${id}`);
  return response.data;
}

/**
 * List all curves for a given name (all temporal versions).
 */
export async function listCurvesByName(name: string): Promise<CurveSummary[]> {
  const response = await api.get<CurveSummary[]>('/curves', {
    params: { name },
  });
  return response.data;
}

/**
 * List all unique curve names.
 */
export async function listCurveNames(): Promise<string[]> {
  const response = await api.get<string[]>('/curves');
  return response.data;
}

/**
 * Get all available dates for a curve name (descending order - newest first).
 */
export async function getCurveDates(name: string): Promise<string[]> {
  const response = await api.get<string[]>('/curves/dates', {
    params: { name }
  });
  return response.data;
}

/**
 * Get all available currencies.
 */
export async function getCurrencies(): Promise<ReferenceDataItem[]> {
  const response = await api.get<ReferenceDataItem[]>('/reference/currencies');
  return response.data;
}

/**
 * Get all available indexes.
 */
export async function getIndexes(): Promise<ReferenceDataItem[]> {
  const response = await api.get<ReferenceDataItem[]>('/reference/indexes');
  return response.data;
}

/**
 * Get all available instrument types.
 */
export async function getInstrumentTypes(): Promise<ReferenceDataItem[]> {
  const response = await api.get<ReferenceDataItem[]>('/reference/instrument-types');
  return response.data;
}

/**
 * Get all available tenors.
 */
export async function getTenors(): Promise<ReferenceDataItem[]> {
  const response = await api.get<ReferenceDataItem[]>('/reference/tenors');
  return response.data;
}

/**
 * Update request for modifying curve instruments.
 */
export interface UpdateCurveRequest {
  instruments: InstrumentInput[];
}

/**
 * Update an existing curve's instruments.
 */
export async function updateCurve(id: string, request: UpdateCurveRequest): Promise<CurveResponse> {
  const response = await api.put<CurveResponse>(`/curves/${id}`, request);
  return response.data;
}

/**
 * Delete a curve version by name and date.
 */
export async function deleteCurve(name: string, date: string): Promise<void> {
  await api.delete('/curves', {
    params: { name, date },
  });
}

