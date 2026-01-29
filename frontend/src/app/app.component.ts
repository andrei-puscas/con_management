import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardAvatarComponent } from '@/shared/components/avatar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ZardButtonComponent, ZardAvatarComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'ConManagement';
  private router = inject(Router);
  constructor(readonly auth: AuthService) {}

  get breadcrumbItems(): { label: string; link: string }[] {
    const url = this.router.url;
    if (url.startsWith('/users')) return [{ label: 'Dashboard', link: '/dashboard' }, { label: 'Utilizatori', link: '/users' }];
    if (url.startsWith('/dashboard')) return [{ label: 'Dashboard', link: '/dashboard' }];
    if (url.startsWith('/login')) return [{ label: 'Autentificare', link: '/login' }];
    return [{ label: 'Dashboard', link: '/dashboard' }];
  }
}
