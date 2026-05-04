export interface ConsultResponse {
  id: string;
  doctorId: string;
  doctorName: string;
  pacientId: string;
  pacientEmail: string;
  startAt: string;
  endAt: string;
  status: string;
  notes?: string | null;
  createdAt: string;
}

export interface CreateConsultPayload {
  doctorId: string;
  pacientId: string;
  startAt: string;
  endAt: string;
  status: string;
  notes?: string;
}

export interface DoctorOption {
  id: string;
  name: string;
  speciality: string;
  status: boolean;
}

export interface TimeSlot {
  startTime: string;
  endTime: string;
  available: boolean;
}

export interface DayAvailability {
  dayOfWeek: number;
  dayLabel: string;
  slots: TimeSlot[];
}

export interface DoctorAvailability {
  doctorId: string;
  doctorName: string;
  speciality: string;
  days: DayAvailability[];
}

export interface AddressPayload {
  street: string;
  neighborhood: string;
  state: string;
  country: string;
  complement: string;
}

export interface RegisterPacientPayload {
  email: string;
  cpf: string;
  document: string;
  phone: string;
  address: AddressPayload;
}

export interface PacientPortalResponse {
  id: string;
  email: string;
  cpf: string;
  document: string;
  phone: string;
  address: AddressPayload;
}
