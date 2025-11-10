import React from 'react';
import LoginSalonManagerForm from '../components/auth/LoginSalonManagerForm';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';

export default function LoginPage() {
    return (
        <div className="min-h-screen flex flex-col">
            <Header />
            <main className="container mx-auto px-4 py-12 flex-1">
                <LoginSalonManagerForm />
            </main>
            <Footer />
        </div>
    );
}