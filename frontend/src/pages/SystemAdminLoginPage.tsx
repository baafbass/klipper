import React from 'react';
import LoginSysAdminForm from '../components/auth/LoginSysAdminForm';
import AdminHeader from '../components/layout/AdminHeader';
import Footer from '../components/layout/Footer';


export default function SystemAdminLoginPage(){
return (
<div className="min-h-screen flex flex-col">
<AdminHeader />
<main className="container mx-auto px-4 py-12 flex-1">
<LoginSysAdminForm />
</main>
<Footer />
</div>
);
}