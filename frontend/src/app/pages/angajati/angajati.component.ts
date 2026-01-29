import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AngajatiService, type AngajatDto } from '../../core/angajati.service';
import { ZardDialogService } from '@/shared/components/dialog/dialog.service';
import { AngajatFormDialogComponent } from './angajat-form-dialog/angajat-form-dialog.component';
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
  selector: 'app-angajati',
  standalone: true,
  imports: [
    CommonModule,
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
  templateUrl: './angajati.component.html',
})
export class AngajatiComponent {
  private angajatiService = inject(AngajatiService);
  private dialogService = inject(ZardDialogService);

  angajati = signal<AngajatDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor() {
    this.loadAngajati();
  }

  loadAngajati(): void {
    this.loading.set(true);
    this.error.set(null);
    this.angajatiService.getAll().subscribe({
      next: (list) => {
        this.angajati.set(list ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Eroare la încărcare.');
      },
    });
  }

  openCreate(): void {
    this.dialogService.create({
      zContent: AngajatFormDialogComponent,
      zTitle: 'Adaugă angajat',
      zDescription: 'Completează datele angajatului.',
      zData: { angajat: null, onSuccess: () => this.loadAngajati() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  openEdit(angajat: AngajatDto): void {
    this.dialogService.create({
      zContent: AngajatFormDialogComponent,
      zTitle: 'Editează angajat',
      zDescription: 'Modifică datele angajatului.',
      zData: { angajat, onSuccess: () => this.loadAngajati() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  deleteAngajat(angajat: AngajatDto): void {
    if (!confirm(`Ștergi angajatul „${angajat.nume}"?`)) return;
    this.angajatiService.delete(angajat.id).subscribe({
      next: (ok) => {
        if (ok) this.loadAngajati();
        else this.error.set('Nu s-a putut șterge angajatul.');
      },
      error: () => this.error.set('Eroare la ștergere.'),
    });
  }
}
