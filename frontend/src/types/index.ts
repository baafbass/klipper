export type Role = 'Customer' | 'Employee' | 'SalonManager' | 'SystemAdmin';

export interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    phoneNumber?: string;
    role: Role;
    isActive?: boolean;
}

export interface LoginRequest {
    email: string;
    password: string;
}


export interface LoginResponse {
    token: string;
    refreshToken?: string;
    user: User;
}

export interface RegisterRequest {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    phoneNumber?: string;
    dateOfBirth?: string; // ISO string
}

export interface Salon {
    id: string;
    name: string;
    description: string;
    address: string;
    city: string;
    phoneNumber: string;
    email: string;
    isActive: boolean;
    workingHours: WorkingHours[];
}

export interface Service {
    id: string;
    salonId: string;
    name: string;
    description: string;
    durationMinutes: number;
    price: number;
    category: string;
    isActive: boolean;
}

export interface Appointment {
    id: string;
    customerId: string;
    customerName: string;
    employeeId: string;
    employeeName: string;
    salonId: string;
    salonName: string;
    appointmentDate: string;
    startTime: string;
    endTime: string;
    status: 'Pending' | 'Confirmed' | 'Completed' | 'Cancelled' | 'NoShow';
    totalPrice: number;
    totalDurationMinutes: number;
    notes: string;
    services: AppointmentService[];
}

export interface WorkingHours {
    id: string;
    dayOfWeek: number;
    dayName: string;
    openTime: string;
    closeTime: string;
    isOpen: boolean;
}

export interface AppointmentService {
    serviceId: string;
    serviceName: string;
    price: number;
    durationMinutes: number;
}

export interface AvailableTimeSlot {
    startTime: string;
    endTime: string;
    employeeId: string;
    employeeName: string;
}