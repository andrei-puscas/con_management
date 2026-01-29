import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UsersService, type UserDto } from '../../core/users.service';
import { ZardDialogService } from '@/shared/components/dialog/dialog.service';
import { UserFormDialogComponent } from './user-form-dialog/user-form-dialog.component';
import { ZardCardComponent } from '@/shared/components/card';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardLoaderComponent } from '@/shared/components/loader';
import { ZardAlertComponent } from '@/shared/components/alert';
import {
  ZardTableComponent,
  ZardTableHeaderComponent,
  ZardTableBodyComponent,
  ZardTableRowComponent,
  ZardTableHeadComponent,
  ZardTableCellComponent,
} from '@/shared/components/table';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ZardCardComponent,
    ZardButtonComponent,
    ZardLoaderComponent,
    ZardAlertComponent,
    ZardTableComponent,
    ZardTableHeaderComponent,
    ZardTableBodyComponent,
    ZardTableRowComponent,
    ZardTableHeadComponent,
    ZardTableCellComponent,
  ],
  templateUrl: './users.component.html',
})
export class UsersComponent {
  private usersService = inject(UsersService);
  private dialogService = inject(ZardDialogService);

  users = signal<UserDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor() {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.error.set(null);
    this.usersService.getAll().subscribe({
      next: (list) => {
        this.users.set(list);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.status === 403 ? 'Nu ai drepturi de admin.' : 'Eroare la încărcare.');
      },
    });
  }

  openCreate(): void {
    this.dialogService.create({
      zContent: UserFormDialogComponent,
      zTitle: 'Adaugă utilizator',
      zDescription: 'Completează datele pentru noul utilizator.',
      zData: { user: null, onSuccess: () => this.loadUsers() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  openEdit(user: UserDto): void {
    this.dialogService.create({
      zContent: UserFormDialogComponent,
      zTitle: 'Editează utilizator',
      zDescription: 'Modifică datele utilizatorului.',
      zData: { user, onSuccess: () => this.loadUsers() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  deleteUser(user: UserDto): void {
    if (!confirm(`Ștergi utilizatorul ${user.email}?`)) return;
    this.usersService.delete(user.id).subscribe({
      next: (ok) => {
        if (ok) this.loadUsers();
        else this.error.set('Nu s-a putut șterge utilizatorul.');
      },
      error: () => this.error.set('Eroare la ștergere.'),
    });
  }
}
