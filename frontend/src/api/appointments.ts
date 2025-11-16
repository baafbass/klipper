import axiosInstance from './axiosConfig';

export const appointmentApi = {
  getAvailability: (payload: { salonId: string; employeeId?: string | null; date: string; serviceIds: string[] }) =>
    axiosInstance.post('/appointments/availability', payload),

  createAppointment: (payload: {
    customerId: string;
    employeeId: string;
    salonId: string;
    appointmentDate: string; // ISO date
    startTime: string; // "HH:mm"
    serviceIds: string[];
    notes?: string;
  }) => axiosInstance.post('/appointments', payload),
  
  getMyAppointments: () => axiosInstance.get('/appointments/me'),
  confirm: (id: string) => axiosInstance.post(`/appointments/${id}/confirm`),
  cancel: (id: string, reason: string) => axiosInstance.post(`/appointments/${id}/cancel`, { reason }),
};
