import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';

import SystemAdminDashboard from './pages/SystemAdminDashboard';
import SystemAdminAddManagerPage from './pages/SystemAdminAddManagerPage';
import SystemAdminLoginPage from './pages/SystemAdminLoginPage';
import SalonManagerLoginPage from './pages/SalonManagerLoginPage';

import { useAuthStore } from './store/authStore';
import { JSX } from 'react/jsx-runtime';


function PrivateRoute({ children }: { children: JSX.Element }) {
    const { isAuthenticated } = useAuthStore();
    if (!isAuthenticated) return <Navigate to="/login" replace />;
    return children;
}

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<PrivateRoute><HomePage /></PrivateRoute>} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                 
                <Route path="/salon-manager-login" element={<SalonManagerLoginPage/>}/>
                <Route path="/admin/login" element={<SystemAdminLoginPage />} />
                <Route path="/admin/dashboard" element={<PrivateRoute><SystemAdminDashboard /></PrivateRoute>} />
                <Route path="/admin/add-manager" element={<PrivateRoute><SystemAdminAddManagerPage /></PrivateRoute>} />
            </Routes>
        </BrowserRouter>
    );
}