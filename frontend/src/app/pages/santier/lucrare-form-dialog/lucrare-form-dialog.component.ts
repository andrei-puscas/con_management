import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardDialogRef } from '@/shared/components/dialog/dialog-ref';
import { Z_MODAL_DATA } from '@/shared/components/dialog/dialog.service';
import { LucrariService, type LucrareDto } from '@/core/lucrari.service';
import { EchipeService, type EchipaDto } from '@/core/echipe.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardInputDirective } from '@/shared/components/input';
import { ZardAlertComponent } from '@/shared/components/alert';

export interface LucrareFormDialogData {
  santierId: number;
  lucrare: LucrareDto | null;
  onSuccess: () => void;
}

@Component({
  selector: 'app-lucrare-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardInputDirective,
    ZardAlertComponent,
  ],
  templateUrl: './lucrare-form-dialog.component.html',
})
export class LucrareFormDialogComponent implements OnInit {
  private lucrariService = inject(LucrariService);
  private echipeService = inject(EchipeService);
  private fb = inject(FormBuilder);
  readonly dialogRef = inject(ZardDialogRef<LucrareFormDialogComponent, boolean>);
  readonly data = inject<LucrareFormDialogData>(Z_MODAL_DATA);

  formError = signal<string | null>(null);
  saving = signal(false);
  echipe = signal<EchipaDto[]>([]);
  readonly isEditMode = this.data.lucrare !== null;

  form = this.fb.nonNullable.group({
    descriere: ['', Validators.required],
    termen: [this.formatDateForInput(new Date()), Validators.required],
    stare: ['Planificat', Validators.required],
    echipaIds: [[] as number[]],
  });

  ngOnInit(): void {
    this.echipeService.getAll().subscribe({
      next: (list) => this.echipe.set(list ?? []),
    });
    if (this.data.lucrare) {
      const l = this.data.lucrare;
      this.form.patchValue({
        descriere: l.descriere,
        termen: this.formatDateForInput(l.termen),
        stare: l.stare,
        echipaIds: l.echipaIds ?? [],
      });
    }
  }

  private formatDateForInput(d: string | Date): string {
    const date = typeof d === 'string' ? new Date(d) : d;
    return date.toISOString().slice(0, 10);
  }

  private toIso(dateStr: string): string {
    return new Date(dateStr).toISOString();
  }

  toggleEchipa(id: number): void {
    const current = this.form.get('echipaIds')?.value ?? [];
    const next = current.includes(id) ? current.filter((x) => x !== id) : [...current, id];
    this.form.get('echipaIds')?.setValue(next);
  }

  isEchipaSelected(id: number): boolean {
    return (this.form.get('echipaIds')?.value ?? []).includes(id);
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  save(): void {
    this.formError.set(null);
    const santierId = this.data.santierId;
    const v = this.form.getRawValue();
    const payload = {
      santierId,
      descriere: v.descriere,
      termen: this.toIso(v.termen),
      stare: v.stare,
      echipaIds: v.echipaIds,
    };
    if (this.data.lucrare) {
      this.saving.set(true);
      this.lucrariService.update(this.data.lucrare.id, { ...payload }).subscribe({
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
      this.lucrariService.create(payload).subscribe({
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
