import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EchipeService, type EchipaDto } from '../../core/echipe.service';
import { ZardDialogService } from '@/shared/components/dialog/dialog.service';
import { EchipaFormDialogComponent } from './echipa-form-dialog/echipa-form-dialog.component';
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
  selector: 'app-echipe',
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
  templateUrl: './echipe.component.html',
})
export class EchipeComponent {
  private echipeService = inject(EchipeService);
  private dialogService = inject(ZardDialogService);

  echipe = signal<EchipaDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor() {
    this.loadEchipe();
  }

  loadEchipe(): void {
    this.loading.set(true);
    this.error.set(null);
    this.echipeService.getAll().subscribe({
      next: (list) => {
        this.echipe.set(list ?? []);
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
      zContent: EchipaFormDialogComponent,
      zTitle: 'Adaugă echipă',
      zDescription: 'Completează datele echipei. Echipele pot fi asignate la lucrări.',
      zData: { echipa: null, onSuccess: () => this.loadEchipe() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  openEdit(echipa: EchipaDto): void {
    this.dialogService.create({
      zContent: EchipaFormDialogComponent,
      zTitle: 'Editează echipă',
      zDescription: 'Modifică datele echipei.',
      zData: { echipa, onSuccess: () => this.loadEchipe() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  deleteEchipa(echipa: EchipaDto): void {
    if (!confirm(`Ștergi echipa „${echipa.nume}"?`)) return;
    this.echipeService.delete(echipa.id).subscribe({
      next: (ok) => {
        if (ok) this.loadEchipe();
        else this.error.set('Nu s-a putut șterge echipa.');
      },
      error: () => this.error.set('Eroare la ștergere.'),
    });
  }
}
