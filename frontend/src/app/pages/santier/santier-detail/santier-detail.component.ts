import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { SantierService, type SantierDto } from '../../../core/santier.service';
import { LucrariService, type LucrareDto } from '../../../core/lucrari.service';
import { ZardDialogService } from '@/shared/components/dialog/dialog.service';
import { LucrareFormDialogComponent } from '../lucrare-form-dialog/lucrare-form-dialog.component';
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
  selector: 'app-santier-detail',
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
  templateUrl: './santier-detail.component.html',
})
export class SantierDetailComponent {
  private route = inject(ActivatedRoute);
  private santierService = inject(SantierService);
  private lucrariService = inject(LucrariService);
  private dialogService = inject(ZardDialogService);

  santier = signal<SantierDto | null>(null);
  lucrari = signal<LucrareDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  proiectId = computed(() => Number(this.route.snapshot.paramMap.get('proiectId')));
  santierId = computed(() => Number(this.route.snapshot.paramMap.get('santierId')));

  constructor() {
    const sid = this.santierId();
    const pid = this.proiectId();
    if (sid && pid) {
      this.santierService.getById(sid).subscribe({
        next: (s) => this.santier.set(s ?? null),
        error: () => this.error.set('Șantier negăsit.'),
      });
      this.loadLucrari(sid);
    } else {
      this.loading.set(false);
      this.error.set('Șantier invalid.');
    }
  }

  loadLucrari(santierId: number): void {
    this.loading.set(true);
    this.lucrariService.getAll(santierId).subscribe({
      next: (list) => {
        this.lucrari.set(list ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Eroare la încărcarea lucrărilor.');
      },
    });
  }

  refresh(): void {
    const sid = this.santierId();
    if (sid) {
      this.santierService.getById(sid).subscribe((s) => this.santier.set(s ?? null));
      this.loadLucrari(sid);
    }
  }

  openAddLucrare(): void {
    const sid = this.santierId();
    this.dialogService.create({
      zContent: LucrareFormDialogComponent,
      zTitle: 'Adaugă lucrare',
      zDescription: 'Asignează una sau mai multe echipe la lucrare.',
      zData: { santierId: sid, lucrare: null, onSuccess: () => this.refresh() },
      zHideFooter: true,
      zWidth: '32rem',
    });
  }

  openEditLucrare(l: LucrareDto, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    this.dialogService.create({
      zContent: LucrareFormDialogComponent,
      zTitle: 'Editează lucrare',
      zDescription: 'Modifică datele și echipele asignate.',
      zData: { santierId: this.santierId(), lucrare: l, onSuccess: () => this.refresh() },
      zHideFooter: true,
      zWidth: '32rem',
    });
  }

  deleteLucrare(l: LucrareDto, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    if (!confirm(`Ștergi lucrarea?`)) return;
    this.lucrariService.delete(l.id).subscribe({
      next: (ok) => {
        if (ok) this.refresh();
        else this.error.set('Nu s-a putut șterge lucrarea.');
      },
      error: () => this.error.set('Eroare la ștergere.'),
    });
  }
}
