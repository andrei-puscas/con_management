import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProiecteService, type ProiectDto } from '../../../core/proiecte.service';
import { SantierService, type SantierDto } from '../../../core/santier.service';
import { ZardDialogService } from '@/shared/components/dialog/dialog.service';
import { SantierFormDialogComponent } from '../../santier/santier-form-dialog/santier-form-dialog.component';
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
  selector: 'app-proiect-detail',
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
  templateUrl: './proiect-detail.component.html',
})
export class ProiectDetailComponent {
  private route = inject(ActivatedRoute);
  private proiecteService = inject(ProiecteService);
  private santierService = inject(SantierService);
  private dialogService = inject(ZardDialogService);

  proiect = signal<ProiectDto | null>(null);
  santiere = signal<SantierDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  proiectId = computed(() => Number(this.route.snapshot.paramMap.get('id')));

  constructor() {
    const id = this.proiectId();
    if (id) {
      this.loadProiect(id);
      this.loadSantiere(id);
    } else {
      this.loading.set(false);
      this.error.set('Proiect invalid.');
    }
  }

  loadProiect(id: number): void {
    this.proiecteService.getById(id).subscribe({
      next: (p) => this.proiect.set(p ?? null),
      error: () => this.error.set('Proiect negăsit.'),
    });
  }

  loadSantiere(proiectId: number): void {
    this.loading.set(true);
    this.santierService.getAll(proiectId).subscribe({
      next: (list) => {
        this.santiere.set(list ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Eroare la încărcarea șantierelor.');
      },
    });
  }

  refresh(): void {
    const id = this.proiectId();
    if (id) {
      this.loadProiect(id);
      this.loadSantiere(id);
    }
  }

  openAddSantier(): void {
    const proiectId = this.proiectId();
    this.dialogService.create({
      zContent: SantierFormDialogComponent,
      zTitle: 'Adaugă șantier',
      zDescription: 'Șantierul va fi asociat acestui proiect.',
      zData: { proiectId, santier: null, onSuccess: () => this.refresh() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  openEditSantier(s: SantierDto, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    this.dialogService.create({
      zContent: SantierFormDialogComponent,
      zTitle: 'Editează șantier',
      zDescription: 'Modifică datele șantierului.',
      zData: { proiectId: this.proiectId(), santier: s, onSuccess: () => this.refresh() },
      zHideFooter: true,
      zWidth: '28rem',
    });
  }

  deleteSantier(s: SantierDto, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    if (!confirm(`Ștergi șantierul „${s.adresa}"?`)) return;
    this.santierService.delete(s.id).subscribe({
      next: (ok) => {
        if (ok) this.refresh();
        else this.error.set('Nu s-a putut șterge șantierul.');
      },
      error: () => this.error.set('Eroare la ștergere.'),
    });
  }
}
