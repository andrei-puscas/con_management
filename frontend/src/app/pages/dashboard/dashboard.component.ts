import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { UsersService } from '../../core/users.service';
import { ZardCardComponent } from '@/shared/components/card';
import { ZardLoaderComponent } from '@/shared/components/loader';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ZardCardComponent, ZardLoaderComponent],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent {
  readonly auth = inject(AuthService);
  private usersService = inject(UsersService);

  usersCount = signal<number | null>(null);

  constructor() {
    if (this.auth.isAdmin()) {
      this.usersService.getAll().subscribe({
        next: (list) => this.usersCount.set(list.length),
        error: () => this.usersCount.set(0),
      });
    }
  }
}
