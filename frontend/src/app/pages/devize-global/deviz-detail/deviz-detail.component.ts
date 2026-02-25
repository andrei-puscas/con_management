import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  DevizeService,
  type DevizDto,
  type CreateDevizLinieRequest,
} from '../../../core/devize.service';
import { AuthService } from '../../../core/auth.service';
import { ZardCardComponent } from '@/shared/components/card';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardLoaderComponent } from '@/shared/components/loader';
import { ZardAlertComponent } from '@/shared/components/alert';

@Component({
  selector: 'app-deviz-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    ZardCardComponent,
    ZardButtonComponent,
    ZardLoaderComponent,
    ZardAlertComponent,
  ],
  templateUrl: './deviz-detail.component.html',
})
export class DevizDetailComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private devizeService = inject(DevizeService);
  auth = inject(AuthService);

  devizId = computed(() => Number(this.route.snapshot.paramMap.get('id')));
  deviz = signal<DevizDto | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  downloadingPdf = signal(false);

  // Edit mode
  editing = signal(false);
  saving = signal(false);
  editError = signal<string | null>(null);

  editTitlu = signal('');
  editNumarInregistrare = signal('');
  editBeneficiar = signal('');
  editExecutant = signal('');
  editCotaTVA = signal(19);
  editData = signal('');
  editLinii = signal<CreateDevizLinieRequest[]>([]);

  constructor() {
    this.devizeService.getById(this.devizId()).subscribe({
      next: (d) => {
        this.deviz.set(d);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Devizul nu a fost găsit.');
      },
    });
  }

  startEdit(): void {
    const d = this.deviz();
    if (!d) return;
    this.editTitlu.set(d.titlu);
    this.editNumarInregistrare.set(d.numarInregistrare ?? '');
    this.editBeneficiar.set(d.beneficiar ?? '');
    this.editExecutant.set(d.executant ?? '');
    this.editCotaTVA.set(d.cotaTVA);
    this.editData.set(d.data.split('T')[0]);
    this.editLinii.set(
      d.linii.map((l) => ({
        numar: l.numar,
        descriere: l.descriere,
        um: l.um,
        cantitate: l.cantitate,
        pretUnitar: l.pretUnitar,
      }))
    );
    this.editError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.editError.set(null);
  }

  addLinie(): void {
    const nextNr = this.editLinii().length + 1;
    this.editLinii.set([
      ...this.editLinii(),
      { numar: nextNr, descriere: '', um: 'buc', cantitate: 0, pretUnitar: 0 },
    ]);
  }

  removeLinie(index: number): void {
    this.editLinii.set(
      this.editLinii()
        .filter((_, i) => i !== index)
        .map((l, i) => ({ ...l, numar: i + 1 }))
    );
  }

  updateLinie(index: number, field: keyof CreateDevizLinieRequest, value: string | number): void {
    this.editLinii.set(
      this.editLinii().map((l, i) =>
        i === index
          ? { ...l, [field]: field === 'descriere' || field === 'um' ? value : Number(value) }
          : l
      )
    );
  }

  totalLinie(l: CreateDevizLinieRequest): number {
    return l.cantitate * l.pretUnitar;
  }

  editTotalFaraTva(): number {
    return this.editLinii().reduce((s, l) => s + l.cantitate * l.pretUnitar, 0);
  }

  saveEdit(): void {
    if (!this.editTitlu().trim()) {
      this.editError.set('Titlul este obligatoriu.');
      return;
    }
    this.saving.set(true);
    this.editError.set(null);

    this.devizeService
      .updateGlobal(this.devizId(), {
        titlu: this.editTitlu().trim(),
        numarInregistrare: this.editNumarInregistrare() || undefined,
        beneficiar: this.editBeneficiar() || undefined,
        executant: this.editExecutant() || undefined,
        cotaTVA: this.editCotaTVA(),
        data: this.editData(),
        linii: this.editLinii(),
      })
      .subscribe({
        next: (updated) => {
          this.deviz.set(updated);
          this.saving.set(false);
          this.editing.set(false);
        },
        error: () => {
          this.saving.set(false);
          this.editError.set('Eroare la salvare. Încearcă din nou.');
        },
      });
  }

  downloadPdf(): void {
    const d = this.deviz();
    if (!d) return;
    this.downloadingPdf.set(true);
    const url = this.devizeService.getPdfUrl(d.id);
    const token = this.auth.getToken();
    fetch(url, { headers: { Authorization: `Bearer ${token}` } })
      .then((res) => res.blob())
      .then((blob) => {
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = `deviz_${d.titlu.replace(/\s+/g, '_')}_${new Date().toISOString().split('T')[0]}.pdf`;
        a.click();
        URL.revokeObjectURL(a.href);
        this.downloadingPdf.set(false);
      })
      .catch(() => this.downloadingPdf.set(false));
  }

  deleteDeviz(): void {
    const d = this.deviz();
    if (!d) return;
    if (!confirm(`Ștergi devizul „${d.titlu}"? Această acțiune nu poate fi anulată.`)) return;
    this.devizeService.deleteGlobal(d.id).subscribe({
      next: () => this.router.navigate(['/devize']),
    });
  }
}
