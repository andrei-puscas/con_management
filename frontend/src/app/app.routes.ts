import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { managerGuard } from './core/manager.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard],
  },
  {
    path: 'users',
    loadComponent: () => import('./pages/users/users.component').then(m => m.UsersComponent),
    canActivate: [authGuard],
  },
  {
    path: 'proiecte/:proiectId/santier/:santierId',
    loadComponent: () => import('./pages/santier/santier-detail/santier-detail.component').then(m => m.SantierDetailComponent),
    canActivate: [authGuard],
  },
  {
    path: 'proiecte/:id',
    loadComponent: () => import('./pages/proiecte/proiect-detail/proiect-detail.component').then(m => m.ProiectDetailComponent),
    canActivate: [authGuard],
  },
  {
    path: 'proiecte',
    loadComponent: () => import('./pages/proiecte/proiecte-list/proiecte-list.component').then(m => m.ProiecteListComponent),
    canActivate: [authGuard],
  },
  {
    path: 'echipe',
    loadComponent: () => import('./pages/echipe/echipe.component').then(m => m.EchipeComponent),
    canActivate: [authGuard, managerGuard],
  },
  {
    path: 'angajati',
    loadComponent: () => import('./pages/angajati/angajati.component').then(m => m.AngajatiComponent),
    canActivate: [authGuard, managerGuard],
  },
  { path: '**', redirectTo: 'dashboard' },
];
