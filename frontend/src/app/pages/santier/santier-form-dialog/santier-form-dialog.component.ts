import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardDialogRef } from '@/shared/components/dialog/dialog-ref';
import { Z_MODAL_DATA } from '@/shared/components/dialog/dialog.service';
import { SantierService, type SantierDto } from '@/core/santier.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardAlertComponent } from '@/shared/components/alert';

export interface SantierFormDialogData {
  proiectId: number;
  santier: SantierDto | null;
  onSuccess: () => void;
}

@Component({
  selector: 'app-santier-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardInputDirective,
    ZardAlertComponent,
  ],
  templateUrl: './santier-form-dialog.component.html',
})
export class SantierFormDialogComponent {
  private santierService = inject(SantierService);
  private fb = inject(FormBuilder);
  readonly dialogRef = inject(ZardDialogRef<SantierFormDialogComponent, boolean>);
  readonly data = inject<SantierFormDialogData>(Z_MODAL_DATA);

  formError = signal<string | null>(null);
  saving = signal(false);
  readonly isEditMode = this.data.santier !== null;

  form = this.fb.nonNullable.group({
    adresa: ['', Validators.required],
    descriere: [''],
  });

  constructor() {
    if (this.data.santier) {
      this.form.patchValue({
        adresa: this.data.santier.adresa,
        descriere: this.data.santier.descriere ?? '',
      });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    this.formError.set(null);
    const { proiectId } = this.data;
    const v = this.form.getRawValue();
    if (this.data.santier) {
      this.saving.set(true);
      this.santierService.update(this.data.santier.id, { adresa: v.adresa, descriere: v.descriere || null }).subscribe({
        next: (updated) => {
          this.saving.set(false);
          if (updated) {
            this.data.onSuccess();
            this.dialogRef.close(true);
          } else {
            this.formError.set('Eroare la actualizare.');
          }
        },
        error: () => {
          this.saving.set(false);
          this.formError.set('Eroare la actualizare.');
        },
      });
    } else {
      this.saving.set(true);
      this.santierService.create({ proiectId, adresa: v.adresa, descriere: v.descriere || null }).subscribe({
        next: (created) => {
          this.saving.set(false);
          if (created) {
            this.data.onSuccess();
            this.dialogRef.close(true);
          } else {
            this.formError.set('Eroare la creare.');
          }
        },
        error: () => {
          this.saving.set(false);
          this.formError.set('Eroare la creare.');
        },
      });
    }
  }
}
