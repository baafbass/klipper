import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';


export default function AdminHeader() {
    const { isAuthenticated, user, logout } = useAuthStore();
    const navigate = useNavigate();


    const handleLogout = () => {
        logout();
        navigate('/admin/login');
    };


    return (
        <header className="bg-white border-b shadow-sm">
            <div className="container mx-auto px-4 py-4 flex items-center justify-between">
                <Link to="/" className="text-xl font-semibold">Klipper</Link>


                <nav className="space-x-4">
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