import type {
  ConsultResponse,
  CreateConsultPayload,
  DoctorAvailability,
  DoctorOption,
  PacientPortalResponse,
  RegisterPacientPayload
} from "../types";

const DEFAULT_API_URL = "/api";

export const API_URL = import.meta.env.VITE_API_URL || DEFAULT_API_URL;

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, {
    headers: {
      "Content-Type": "application/json"
    },
    ...options
  });

  if (!response.ok) {
    let message = "Nao foi possivel concluir a solicitacao.";

    try {
      const errorBody = (await response.json()) as { message?: string };
      if (errorBody?.message) {
        message = errorBody.message;
      }
    } catch {
      message = response.statusText || message;
    }

    throw new Error(message);
  }

  if (response.status === 204) {
    return null as T;
  }

  return (await response.json()) as T;
}

export function getConsultasPorPaciente(pacienteId: string) {
  return request<ConsultResponse[]>(`/Consults/pacient/${pacienteId}`);
}

export function registrarPaciente(payload: RegisterPacientPayload) {
  return request<PacientPortalResponse>("/ClientPortal/pacients/register", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function criarConsulta(payload: CreateConsultPayload) {
  return request<ConsultResponse>("/Consults", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function getMedicos() {
  return request<DoctorOption[]>("/ClientPortal/doctors");
}

export function getDisponibilidadeMedico(doctorId: string) {
  return request<DoctorAvailability>(`/ClientPortal/doctors/${doctorId}/availability`);
}
