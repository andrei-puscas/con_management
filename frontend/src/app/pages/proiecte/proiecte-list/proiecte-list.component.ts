import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProiecteService, type ProiectDto } from '../../../core/proiecte.service';
import { ZardDialogService } from '@/shared/components/dialog/dialog.service';
import { ProiectFormDialogComponent } from '../proiect-form-dialog/proiect-form-dialog.component';
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
  selector: 'app-proiecte-list',
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
  templateUrl: './proiecte-list.component.html',
})
export class ProiecteListComponent {
  private proiecteService = inject(ProiecteService);
  private dialogService = inject(ZardDialogService);

  proiecte = signal<ProiectDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor() {
    this.loadProiecte();
  }

  loadProiecte(): void {
    this.loading.set(true);
    this.error.set(null);
    this.proiecteService.getAll().subscribe({
      next: (list) => {
        this.proiecte.set(list ?? []);
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
      zContent: ProiectFormDialogComponent,
      zTitle: 'Adaugă proiect',
      zDescription: 'Completează datele proiectului.',
      zData: { proiect: null, onSuccess: () => this.loadProiecte() },
      zHideFooter: true,
      zWidth: '32rem',
    });
  }

  openEdit(proiect: ProiectDto, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    this.dialogService.create({
      zContent: ProiectFormDialogComponent,
      zTitle: 'Editează proiect',
      zDescription: 'Modifică datele proiectului.',
      zData: { proiect, onSuccess: () => this.loadProiecte() },
      zHideFooter: true,
      zWidth: '32rem',
    });
  }

  deleteProiect(proiect: ProiectDto, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    if (!confirm(`Ștergi proiectul „${proiect.nume}"?`)) return;
    this.proiecteService.delete(proiect.id).subscribe({
      next: (ok) => {
        if (ok) this.loadProiecte();
        else this.error.set('Nu s-a putut șterge proiectul.');
      },
      error: () => this.error.set('Eroare la ștergere.'),
    });
  }
}
