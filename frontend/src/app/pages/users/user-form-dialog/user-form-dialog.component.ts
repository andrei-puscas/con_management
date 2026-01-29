import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardDialogRef } from '@/shared/components/dialog/dialog-ref';
import { Z_MODAL_DATA } from '@/shared/components/dialog/dialog.service';
import { UsersService, type UserDto } from '@/core/users.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardAlertComponent } from '@/shared/components/alert';

const ROLES = ['Admin', 'Manager', 'User'] as const;

export interface UserFormDialogData {
  user: UserDto | null;
  onSuccess: () => void;
}

@Component({
  selector: 'app-user-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardInputDirective,
    ZardAlertComponent,
  ],
  templateUrl: './user-form-dialog.component.html',
})
export class UserFormDialogComponent {
  private usersService = inject(UsersService);
  private fb = inject(FormBuilder);
  readonly dialogRef = inject(ZardDialogRef<UserFormDialogComponent, boolean>);
  readonly data = inject<UserFormDialogData>(Z_MODAL_DATA);

  formError = signal<string | null>(null);
  saving = signal(false);

  readonly roles = ROLES;
  readonly isEditMode = this.data.user !== null;

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.minLength(6)]],
    rol: ['User' as (typeof ROLES)[number], Validators.required],
  });

  constructor() {
    if (this.data.user) {
      this.form.patchValue({
        email: this.data.user.email,
        rol: this.data.user.rol as (typeof ROLES)[number],
      });
      this.form.get('password')?.setValue('');
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();
    } else {
      this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
      this.form.get('password')?.updateValueAndValidity();
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    this.formError.set(null);
    const id = this.data.user?.id ?? null;
    const isEdit = id !== null;

    if (isEdit) {
      const payload = {
        email: this.form.get('email')?.value ?? undefined,
        rol: this.form.get('rol')?.value ?? undefined,
        password: (this.form.get('password')?.value as string)?.trim() || undefined,
      };
      if (!payload.email || !payload.rol) {
        this.formError.set('Email și rol sunt obligatorii.');
        return;
      }
      this.saving.set(true);
      this.usersService.update(id, payload).subscribe({
        next: (updated) => {
          this.saving.set(false);
          if (updated) {
            this.data.onSuccess();
            this.dialogRef.close(true);
          } else {
            this.formError.set('Eroare la actualizare. Verifică datele.');
          }
        },
        error: (err) => {
          this.saving.set(false);
          this.formError.set(err?.error?.message ?? err?.error ?? 'Eroare la actualizare.');
        },
      });
    } else {
      const email = this.form.get('email')?.value;
      const password = this.form.get('password')?.value;
      const rol = this.form.get('rol')?.value;
      if (!email || !password || !rol) {
        this.formError.set('Toate câmpurile sunt obligatorii. Parola minim 6 caractere.');
        return;
      }
      if (password.length < 6) {
        this.formError.set('Parola trebuie să aibă minim 6 caractere.');
        return;
      }
      this.saving.set(true);
      this.usersService.create({ email, password, rol }).subscribe({
        next: (created) => {
          this.saving.set(false);
          if (created) {
            this.data.onSuccess();
            this.dialogRef.close(true);
          } else {
            this.formError.set('Eroare la creare. Poate emailul există deja.');
          }
        },
        error: (err) => {
          this.saving.set(false);
          this.formError.set(err?.error?.message ?? err?.error ?? 'Eroare la creare. Verifică emailul.');
        },
      });
    }
  }
}
