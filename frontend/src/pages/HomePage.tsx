import React from 'react';
import Header from '../components/layout/Header';
import Footer from '../components/layout/Footer';

export default function HomePage() {
    return (
        <div className="min-h-screen flex flex-col">
            <Header />
            <main className="container mx-auto px-4 py-12 flex-1">
                <div className="bg-white p-6 rounded shadow">Welcome to the Salon Management App!</div>
            </main>
            <Footer />
        </div>
    );
}