import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardDialogRef } from '@/shared/components/dialog/dialog-ref';
import { Z_MODAL_DATA } from '@/shared/components/dialog/dialog.service';
import { ProiecteService, type ProiectDto } from '@/core/proiecte.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardAlertComponent } from '@/shared/components/alert';

export interface ProiectFormDialogData {
  proiect: ProiectDto | null;
  onSuccess: () => void;
}

@Component({
  selector: 'app-proiect-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardInputDirective,
    ZardAlertComponent,
  ],
  templateUrl: './proiect-form-dialog.component.html',
})
export class ProiectFormDialogComponent {
  private proiecteService = inject(ProiecteService);
  private fb = inject(FormBuilder);
  readonly dialogRef = inject(ZardDialogRef<ProiectFormDialogComponent, boolean>);
  readonly data = inject<ProiectFormDialogData>(Z_MODAL_DATA);

  formError = signal<string | null>(null);
  saving = signal(false);
  readonly isEditMode = this.data.proiect !== null;

  form = this.fb.group({
    nume: ['', Validators.required],
    client: ['', Validators.required],
    dataStart: [this.formatDateForInput(new Date()), Validators.required],
    dataSfarsit: [''],
    stare: ['Activ', Validators.required],
    buget: [null as number | null],
    moneda: ['RON' as 'RON' | 'EUR'],
  });

  constructor() {
    if (this.data.proiect) {
      const p = this.data.proiect;
      this.form.patchValue({
        nume: p.nume,
        client: p.client,
        dataStart: this.formatDateForInput(p.dataStart),
        dataSfarsit: p.dataSfarsit ? this.formatDateForInput(p.dataSfarsit) : '',
        stare: p.stare,
        buget: p.buget,
        moneda: (p.moneda === 'EUR' ? 'EUR' : 'RON') as 'RON' | 'EUR',
      });
    }
  }

  private formatDateForInput(d: string | Date): string {
    const date = typeof d === 'string' ? new Date(d) : d;
    return date.toISOString().slice(0, 10);
  }

  private toIso(dateStr: string | null): string {
    if (!dateStr) return '';
    return new Date(dateStr).toISOString();
  }

  private parseBuget(value: number | string | null | undefined): number | null {
    if (value === null || value === undefined || value === '') return null;
    const n = typeof value === 'string' ? parseFloat(value) : value;
    return isNaN(n) ? null : n;
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    this.formError.set(null);
    const id = this.data.proiect?.id ?? null;
    if (id !== null) {
      const v = this.form.getRawValue();
      this.saving.set(true);
      this.proiecteService.update(id, {
        nume: v.nume ?? undefined,
        client: v.client ?? undefined,
        dataStart: v.dataStart ? this.toIso(v.dataStart) : undefined,
        dataSfarsit: v.dataSfarsit ? this.toIso(v.dataSfarsit) : null,
        stare: v.stare ?? undefined,
        buget: this.parseBuget(v.buget),
        moneda: v.moneda ?? 'RON',
      }).subscribe({
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
      const v = this.form.getRawValue();
      this.saving.set(true);
      this.proiecteService.create({
        nume: v.nume ?? '',
        client: v.client ?? '',
        dataStart: this.toIso(v.dataStart ?? ''),
        dataSfarsit: v.dataSfarsit ? this.toIso(v.dataSfarsit) : null,
        stare: v.stare ?? 'Activ',
        buget: this.parseBuget(v.buget),
        moneda: v.moneda ?? 'RON',
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
