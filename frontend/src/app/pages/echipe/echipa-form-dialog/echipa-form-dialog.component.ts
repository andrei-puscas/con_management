import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardDialogRef } from '@/shared/components/dialog/dialog-ref';
import { Z_MODAL_DATA } from '@/shared/components/dialog/dialog.service';
import { EchipeService, type EchipaDto } from '@/core/echipe.service';
import { AngajatiService } from '@/core/angajati.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardAlertComponent } from '@/shared/components/alert';

export interface EchipaFormDialogData {
  echipa: EchipaDto | null;
  onSuccess: () => void;
}

@Component({
  selector: 'app-echipa-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardInputDirective,
    ZardAlertComponent,
  ],
  templateUrl: './echipa-form-dialog.component.html',
})
export class EchipaFormDialogComponent implements OnInit {
  private echipeService = inject(EchipeService);
  private angajatiService = inject(AngajatiService);
  private fb = inject(FormBuilder);
  readonly dialogRef = inject(ZardDialogRef<EchipaFormDialogComponent, boolean>);
  readonly data = inject<EchipaFormDialogData>(Z_MODAL_DATA);

  formError = signal<string | null>(null);
  saving = signal(false);
  angajati = signal<{ id: number; nume: string }[]>([]);
  readonly isEditMode = this.data.echipa !== null;

  form = this.fb.nonNullable.group({
    nume: ['', Validators.required],
    sefEchipaId: [null as number | null],
  });

  ngOnInit(): void {
    this.angajatiService.getAll().subscribe({
      next: (list) => this.angajati.set((list ?? []).map((a) => ({ id: a.id, nume: a.nume }))),
    });
    if (this.data.echipa) {
      this.form.patchValue({
        nume: this.data.echipa.nume,
        sefEchipaId: this.data.echipa.sefEchipaId,
      });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    this.formError.set(null);
    const v = this.form.getRawValue();
    if (this.data.echipa) {
      this.saving.set(true);
      this.echipeService.update(this.data.echipa.id, { nume: v.nume, sefEchipaId: v.sefEchipaId }).subscribe({
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
      this.echipeService.create({ nume: v.nume, sefEchipaId: v.sefEchipaId }).subscribe({
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
