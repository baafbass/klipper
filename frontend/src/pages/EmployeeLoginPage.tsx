import React from 'react';
import EmployeeLoginForm from '../components/auth/EmployeeLoginForm';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';

export default function EmployeeLoginPage() {
    return (
        <div className="min-h-screen flex flex-col">
            <Header />
            <main className="container mx-auto px-4 py-12 flex-1">
                <EmployeeLoginForm />
            </main>
            <Footer />
        </div>
    );
}