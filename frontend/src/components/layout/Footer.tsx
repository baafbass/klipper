import React from 'react';
import { Scissors, Calendar, Clock, User, Mail, Lock, Phone, Cake, CheckCircle, AlertCircle, Menu, X, LogOut, Users, Settings, Briefcase } from 'lucide-react';


export default function Footer(){
  return (
    <footer className="bg-gradient-to-r from-slate-900 via-slate-800 to-slate-900 text-slate-300 py-8 mt-auto">
    <div className="container mx-auto px-4">
      <div className="grid md:grid-cols-3 gap-8 text-center md:text-left">
        <div>
          <div className="flex items-center justify-center md:justify-start space-x-2 mb-3">
            <Scissors className="w-5 h-5 text-amber-400" />
            <h3 className="text-lg font-bold text-white">Klipper</h3>
          </div>
          <p className="text-sm text-slate-400">Professional barbershop management system</p>
        </div>
        <div>
          <h4 className="font-semibold text-white mb-3">Quick Links</h4>
          <ul className="space-y-2 text-sm">
            <li><a href="/salon-manager-login" className="hover:text-amber-400 transition-colors">Salon Manager</a></li>
            <li><a href="/employee-login" className="hover:text-amber-400 transition-colors">Employee</a></li>
          </ul>
        </div>
        <div>
          <h4 className="font-semibold text-white mb-3">Contact</h4>
          <p className="text-sm text-slate-400">support@klipper.com</p>
          <p className="text-sm text-slate-400">+90 (555) 123-4567</p>
        </div>
      </div>
      <div className="border-t border-slate-700 mt-8 pt-6 text-center text-sm text-slate-500">
        <p>&copy; 2025 Klipper. All rights reserved.</p>
      </div>
    </div>
  </footer>
  )
};