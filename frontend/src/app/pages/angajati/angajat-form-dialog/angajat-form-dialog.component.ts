import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardDialogRef } from '@/shared/components/dialog/dialog-ref';
import { Z_MODAL_DATA } from '@/shared/components/dialog/dialog.service';
import { AngajatiService, type AngajatDto } from '@/core/angajati.service';
import { EchipeService } from '@/core/echipe.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardAlertComponent } from '@/shared/components/alert';

export interface AngajatFormDialogData {
  angajat: AngajatDto | null;
  onSuccess: () => void;
}

@Component({
  selector: 'app-angajat-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardInputDirective,
    ZardAlertComponent,
  ],
  templateUrl: './angajat-form-dialog.component.html',
})
export class AngajatFormDialogComponent implements OnInit {
  private angajatiService = inject(AngajatiService);
  private echipeService = inject(EchipeService);
  private fb = inject(FormBuilder);
  readonly dialogRef = inject(ZardDialogRef<AngajatFormDialogComponent, boolean>);
  readonly data = inject<AngajatFormDialogData>(Z_MODAL_DATA);

  formError = signal<string | null>(null);
  saving = signal(false);
  echipe = signal<{ id: number; nume: string }[]>([]);
  readonly isEditMode = this.data.angajat !== null;
  readonly isEdit = this.data.angajat !== null;

  form = this.fb.nonNullable.group({
    nume: ['', Validators.required],
    rol: ['', Validators.required],
    competente: [''],
    echipaId: [null as number | null],
    createUser: [true],
    userEmail: [''],
  });

  ngOnInit(): void {
    this.echipeService.getAll().subscribe({
      next: (list) => this.echipe.set((list ?? []).map((e) => ({ id: e.id, nume: e.nume }))),
    });
    if (this.data.angajat) {
      this.form.patchValue({
        nume: this.data.angajat.nume,
        rol: this.data.angajat.rol,
        competente: this.data.angajat.competente ?? '',
        echipaId: this.data.angajat.echipaId,
      });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    this.formError.set(null);
    const v = this.form.getRawValue();
    if (this.data.angajat) {
      this.saving.set(true);
      this.angajatiService.update(this.data.angajat.id, { nume: v.nume, rol: v.rol, competente: v.competente || null, echipaId: v.echipaId }).subscribe({
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
      this.angajatiService.create({
        nume: v.nume,
        rol: v.rol,
        competente: v.competente || null,
        echipaId: v.echipaId,
        createUser: v.createUser,
        userEmail: v.userEmail || null,
      }).subscribe({
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
