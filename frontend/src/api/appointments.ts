// src/api/appointments.ts
import axiosInstance from './axiosConfig';

export const appointmentApi = {
  // availability: { salonId, employeeId?, date, serviceIds: [] }
  getAvailability: (payload: {
    salonId: string;
    employeeId?: string | null;
    date: string; // ISO date string
    serviceIds: string[];
  }) => axiosInstance.post('/appointments/availability', payload),

  // create appointment
  createAppointment: (payload: {
    customerId: string;
    employeeId: string;
    salonId: string;
    appointmentDate: string; // ISO date
    startTime: string; // "HH:mm:ss"
    serviceIds: string[];
    notes?: string;
  }) => axiosInstance.post('/appointments', payload),

  getMyAppointments: () => axiosInstance.get('/appointments/me'),

  getAppointmentById: (id: string) => axiosInstance.get(`/appointments/${id}`),

  cancelAppointment: (id: string, reason: string) =>
    axiosInstance.post(`/appointments/${id}/cancel`, { reason }),

  confirmAppointment: (id: string) =>
    axiosInstance.post(`/appointments/${id}/confirm`, {}),
};
