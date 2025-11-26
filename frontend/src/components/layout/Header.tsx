// import React from 'react';
// import { Link, useNavigate } from 'react-router-dom';
// import { useAuthStore } from '../../store/authStore';


// export default function Header() {
//     const { isAuthenticated, user, logout } = useAuthStore();
//     const navigate = useNavigate();


//     const handleLogout = () => {
//         logout();
//         navigate('/login');
//     };


//     return (
//         <header className="bg-white border-b shadow-sm">
//             <div className="container mx-auto px-4 py-4 flex items-center justify-between">
//                 <Link to="/" className="text-xl font-semibold">Klipper</Link>


//                 <nav className="space-x-4">
//                     <Link to="/salon-manager-login" className="hover:underline">Salon Manager</Link>
//                     <Link to="/employee-login" className="hover:underline">Employee</Link>
//                     {!isAuthenticated && (
//                         <>
//                             <Link to="/login" className="hover:underline">Login</Link>
//                             <Link to="/register" className="hover:underline">Register</Link>
//                         </>
//                     )}


//                     {isAuthenticated && (
//                         <>
//                             <span className="mr-2">{user?.firstName}</span>
//                             <button onClick={handleLogout} className="px-3 py-1 border rounded">Logout</button>
//                         </>
//                     )}
//                 </nav>
//             </div>
//         </header>
//     );
// }



import React, { useState } from 'react';
import { Scissors, Calendar, Clock, User, Mail, Lock, Phone, Cake, CheckCircle, AlertCircle, Menu, X, LogOut, Users, Settings, Briefcase } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';

// Header Component
export default function Header(){

    const { isAuthenticated, user, logout } = useAuthStore();
    const navigate = useNavigate();


    const handleLogout = () => {
        logout();
        navigate('/login');
    };

  
  return (
    <header className="bg-gradient-to-r from-slate-900 via-slate-800 to-slate-900 text-white shadow-xl">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-20">
          <div className="flex items-center space-x-3">
            <div className="bg-gradient-to-br from-amber-400 to-amber-600 p-3 rounded-xl shadow-lg transform hover:scale-105 transition-transform">
              <Scissors className="w-6 h-6 text-slate-900" />
            </div>
            <div>
              <h1 className="text-2xl font-bold bg-gradient-to-r from-amber-400 to-amber-200 bg-clip-text text-transparent">
                Klipper
              </h1>
              <p className="text-xs text-slate-400">Professional Management</p>
            </div>
          </div>
          
          <nav className="hidden md:flex items-center space-x-6">
           
            <a href="/salon-manager-login" className="text-slate-300 hover:text-amber-400 transition-colors font-medium">
              Salon Manager
            </a>

            <a href="/employee-login" className="text-slate-300 hover:text-amber-400 transition-colors font-medium">
              Employee
            </a>

            {!isAuthenticated && (
                        <>
                            <Link to="/login" className="flex items-center space-x-2 bg-gradient-to-r from-amber-500 to-amber-600 hover:from-amber-600 hover:to-amber-700 px-5 py-2.5 rounded-lg shadow-lg transition-all transform hover:scale-105">Login</Link>
                            <Link to="/register" className="flex items-center space-x-2 bg-gradient-to-r from-amber-500 to-amber-600 hover:from-amber-600 hover:to-amber-700 px-5 py-2.5 rounded-lg shadow-lg transition-all transform hover:scale-105">Register</Link>
                        </>
            )}


            {isAuthenticated && (
                        <>
                            <span className="mr-2">{user?.firstName}</span>
                            <button onClick={handleLogout} className="flex items-center space-x-2 bg-gradient-to-r from-amber-500 to-amber-600 hover:from-amber-600 hover:to-amber-700 px-5 py-2.5 rounded-lg shadow-lg transition-all transform hover:scale-105">Logout</button>
                        </>
                    )}
          </nav>
        </div>
      </div>
    </header>
  );
};