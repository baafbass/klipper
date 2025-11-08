import React from 'react';
import RegisterForm from '../components/auth/RegisterForm';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';

export default function RegisterPage() {
    return (
        <div className="min-h-screen flex flex-col">
            <Header />
            <main className="container mx-auto px-4 py-12 flex-1">
                <RegisterForm />
            </main>
            <Footer />
        </div>
    );
}