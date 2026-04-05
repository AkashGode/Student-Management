import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'students', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'students',
    loadComponent: () => import('./components/students/students.component').then(m => m.StudentsComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: 'students' }
];
