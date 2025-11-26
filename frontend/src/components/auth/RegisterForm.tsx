import React, { useState } from 'react';
import { RegisterRequest } from '../../types';
import { authApi } from '../../api/auth';
import { useNavigate } from 'react-router-dom';
import { Scissors, Calendar, Clock, User, Mail, Lock, Phone, Cake, CheckCircle, AlertCircle, Menu, X, LogOut, Users, Settings, Briefcase } from 'lucide-react';

export default function RegisterForm(){
  
    const [form, setForm] = useState<RegisterRequest>({
        email: '', password: '', firstName: '', lastName: '', phoneNumber: '', dateOfBirth: undefined,
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const navigate = useNavigate();


    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) =>
        setForm((s) => ({ ...s, [e.target.name]: e.target.value }));


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        try {
            await authApi.register(form as RegisterRequest);
            setSuccess('Registration successful. You can now login.');
            setTimeout(() => navigate('/login'), 1200);
        } catch (err: any) {
            setError(err?.response?.data?.message || 'Registration failed');
        } finally {
            setLoading(false);
        }
    };

  return (
    <form onSubmit={handleSubmit} className="max-w-2xl mx-auto">
      <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
        <div className="bg-gradient-to-br from-slate-800 to-slate-900 p-8 text-center">
          <div className="inline-block bg-gradient-to-br from-amber-400 to-amber-600 p-4 rounded-2xl shadow-lg mb-4">
            <User className="w-10 h-10 text-slate-900" />
          </div>
          <h2 className="text-3xl font-bold text-white mb-2">Create Account</h2>
          <p className="text-slate-300">Join us for premium barbershop services</p>
        </div>

        <div className="p-8 space-y-6">
          {success && (
            <div className="bg-green-50 border-l-4 border-green-500 p-4 rounded-lg flex items-start space-x-3">
              <CheckCircle className="w-5 h-5 text-green-500 flex-shrink-0 mt-0.5" />
              <div className="text-sm text-green-700">Registration successful! Redirecting to login...</div>
            </div>
          )}

          <div className="grid md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700">First Name</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <User className="w-5 h-5 text-slate-400" />
                </div>
                <input
                  type="text"
                  name="firstName"
                  value={form.firstName}
                  onChange={handleChange}
                  required
                  className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                  placeholder="first name"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700">Last Name</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <User className="w-5 h-5 text-slate-400" />
                </div>
                <input
                  type="text"
                  name="lastName"
                  value={form.lastName}
                  onChange={handleChange}
                  required
                  className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                  placeholder="last name"
                />
              </div>
            </div>
          </div>

          <div className="space-y-2">
            <label className="block text-sm font-semibold text-slate-700">Email Address</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <Mail className="w-5 h-5 text-slate-400" />
              </div>
              <input
                type="email"
                name="email"
                value={form.email}
                onChange={handleChange}
                required
                className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                placeholder="your@email.com"
              />
            </div>
          </div>

          <div className="grid md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700">Phone Number</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <Phone className="w-5 h-5 text-slate-400" />
                </div>
                <input
                  type="tel"
                  name="phoneNumber"
                  value={form.phoneNumber}
                  onChange={handleChange}
                  className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                  placeholder="+90 (555) 000-0000"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700">Date of Birth</label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <Cake className="w-5 h-5 text-slate-400" />
                </div>
                <input
                  type="date"
                  name="dateOfBirth"
                  value={form.dateOfBirth}
                  onChange={handleChange}
                  className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                />
              </div>
            </div>
          </div>

          <div className="space-y-2">
            <label className="block text-sm font-semibold text-slate-700">Password</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                <Lock className="w-5 h-5 text-slate-400" />
              </div>
              <input
                type="password"
                name="password"
                value={form.password}
                onChange={handleChange}
                required
                className="w-full pl-12 pr-4 py-3 border-2 border-slate-200 rounded-xl focus:border-amber-500 focus:ring-4 focus:ring-amber-100 transition-all outline-none"
                placeholder="••••••••"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-gradient-to-r from-green-500 to-green-600 hover:from-green-600 hover:to-green-700 text-white font-bold py-3.5 rounded-xl shadow-lg hover:shadow-xl transition-all transform hover:scale-[1.02] disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <span className="flex items-center justify-center space-x-2">
                <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                <span>Creating account...</span>
              </span>
            ) : (
              'Create Account'
            )}
          </button>

          <div className="text-center pt-4 border-t border-slate-200">
            <p className="text-slate-600">
              Already have an account?{' '}
              <a href="/login" className="text-amber-600 hover:text-amber-700 font-semibold">
                Sign in
              </a>
            </p>
          </div>
        </div>
      </div>
    </form>
  );
};