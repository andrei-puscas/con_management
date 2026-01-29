import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { ZardCardComponent } from '@/shared/components/card';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardAlertComponent } from '@/shared/components/alert';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardCardComponent,
    ZardInputDirective,
    ZardButtonComponent,
    ZardAlertComponent,
  ],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  loading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.errorMessage = '';
    if (this.form.invalid) return;
    this.loading = true;
    this.auth.login(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.loading = false;
        if (res?.token) {
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage = 'Email sau parolă incorectă.';
        }
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Email sau parolă incorectă.';
      },
    });
  }
}
