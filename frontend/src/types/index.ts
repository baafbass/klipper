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

export interface CreateSalonRequest {
name: string;
description: string;
address: string;
city: string;
phoneNumber: string;
email: string;
}


export interface SalonManagerRequest {
email: string;
password: string;
firstName: string;
lastName: string;
phoneNumber: string;
}

export interface WorkingHours  {
 id: string; 
 dayOfWeek: number; 
 dayName: string; 
 openTime: string; 
 closeTime: string; 
 isOpen: boolean 
};

export interface Service  { 
    id: string; 
    name: string;
    description?: string;
    durationMinutes: number; 
    price: number; 
    category?: string; 
};

export interface Employee  { 
    id: string; 
    email: string; 
    firstName: string; 
    lastName: string; 
    phoneNumber?: string; 
    commissionRate?: number; 
    services?: Service[] 
};

// src/types.ts
// --- Existing types you already have (keep them) ---
// export type CreateSalonRequest = { ... } etc.

// --- Manager / Salon types used by managerApi ---
// Working hours
export interface CreateWorkingHoursRequest {
  dayOfWeek: number;            // 0 = Sunday .. 6 = Saturday
  openTime: string;            // "09:00" or "09:00:00" — backend parses as TimeSpan
  closeTime: string;           // "17:00" or "17:00:00"
  isOpen?: boolean;            // optional; default true
}

export interface UpdateWorkingHoursRequest {
  openTime: string;
  closeTime: string;
  isOpen?: boolean;
}

// Services
export interface CreateServiceRequest {
  name: string;
  description?: string;
  durationMinutes: number;
  price: number;
  category?: string;
}

export interface UpdateServiceRequest {
  name: string;
  description?: string;
  durationMinutes: number;
  price: number;
  category?: string;
}

// Employees
export interface CreateEmployeeRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  commissionRate?: number;
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  commissionRate?: number;
}

// Employee schedules / services
export interface CreateEmployeeScheduleRequest {
  employeeId: string;     // Guid as string
  dayOfWeek: number;
  startTime: string;      // "09:00" or "09:00:00"
  endTime: string;        // "17:00" or "17:00:00"
}

export interface CreateEmployeeServiceRequest {
  employeeId: string;     // Guid as string
  serviceId: string;      // Guid as string
}

// DTOs returned by API (use as needed)
export interface WorkingHoursDto {
  id: string;
  dayOfWeek: number;
  dayName: string;
  openTime: string;
  closeTime: string;
  isOpen: boolean;
}

export interface ServiceDto {
  id: string;
  salonId: string;
  name: string;
  description?: string;
  durationMinutes: number;
  price: number;
  category?: string;
  isActive: boolean;
}

export interface EmployeeDto {
  id: string;
  salonId?: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  commissionRate?: number;
  services?: ServiceDto[];
}


// export interface Service {
//     id: string;
//     salonId: string;
//     name: string;
//     description: string;
//     durationMinutes: number;
//     price: number;
//     category: string;
//     isActive: boolean;
// }

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