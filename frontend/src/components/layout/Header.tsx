import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';


export default function Header() {
    const { isAuthenticated, user, logout } = useAuthStore();
    const navigate = useNavigate();


    const handleLogout = () => {
        logout();
        navigate('/login');
    };


    return (
        <header className="bg-white border-b shadow-sm">
            <div className="container mx-auto px-4 py-4 flex items-center justify-between">
                <Link to="/" className="text-xl font-semibold">Klipper</Link>


                <nav className="space-x-4">
                    <Link to="/salon-manager-login" className="hover:underline">Salon Manager</Link>
                    <Link to="/employee-login" className="hover:underline">Employee</Link>
                    {!isAuthenticated && (
                        <>
                            <Link to="/login" className="hover:underline">Login</Link>
                            <Link to="/register" className="hover:underline">Register</Link>
                        </>
                    )}


                    {isAuthenticated && (
                        <>
                            <span className="mr-2">{user?.firstName}</span>
                            <button onClick={handleLogout} className="px-3 py-1 border rounded">Logout</button>
                        </>
                    )}
                </nav>
            </div>
        </header>
    );
}