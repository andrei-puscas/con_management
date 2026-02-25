import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DevizeService, type CreateDevizLinieRequest } from '../../../core/devize.service';
import { ProiecteService, type ProiectDto } from '../../../core/proiecte.service';
import { ZardCardComponent } from '@/shared/components/card';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardAlertComponent } from '@/shared/components/alert';
import { ZardLoaderComponent } from '@/shared/components/loader';

@Component({
  selector: 'app-deviz-form',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    ZardCardComponent,
    ZardButtonComponent,
    ZardAlertComponent,
    ZardLoaderComponent,
  ],
  templateUrl: './deviz-form.component.html',
})
export class DevizFormComponent {
  private devizeService = inject(DevizeService);
  private proiecteService = inject(ProiecteService);
  private router = inject(Router);

  proiecte = signal<ProiectDto[]>([]);
  proiecteLoading = signal(true);

  saving = signal(false);
  error = signal<string | null>(null);

  proiectId = signal<number | null>(null);
  titlu = signal('');
  numarInregistrare = signal('');
  beneficiar = signal('');
  executant = signal('');
  cotaTVA = signal(19);
  data = signal(new Date().toISOString().split('T')[0]);
  linii = signal<CreateDevizLinieRequest[]>([]);

  constructor() {
    this.proiecteService.getAll().subscribe({
      next: (list) => {
        this.proiecte.set(list ?? []);
        this.proiecteLoading.set(false);
      },
      error: () => this.proiecteLoading.set(false),
    });
  }

  addLinie(): void {
    const nextNr = this.linii().length + 1;
    this.linii.set([
      ...this.linii(),
      { numar: nextNr, descriere: '', um: 'buc', cantitate: 0, pretUnitar: 0 },
    ]);
  }

  removeLinie(index: number): void {
    const updated = this.linii()
      .filter((_, i) => i !== index)
      .map((l, i) => ({ ...l, numar: i + 1 }));
    this.linii.set(updated);
  }

  updateLinie(index: number, field: keyof CreateDevizLinieRequest, value: string | number): void {
    const updated = this.linii().map((l, i) =>
      i === index
        ? { ...l, [field]: field === 'descriere' || field === 'um' ? value : Number(value) }
        : l
    );
    this.linii.set(updated);
  }

  totalLinie(l: CreateDevizLinieRequest): number {
    return l.cantitate * l.pretUnitar;
  }

  totalFaraTva(): number {
    return this.linii().reduce((s, l) => s + l.cantitate * l.pretUnitar, 0);
  }

  save(): void {
    if (!this.proiectId()) {
      this.error.set('Selectează un proiect.');
      return;
    }
    if (!this.titlu().trim()) {
      this.error.set('Titlul devizului este obligatoriu.');
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    this.devizeService
      .createGlobal({
        proiectId: this.proiectId()!,
        titlu: this.titlu().trim(),
        numarInregistrare: this.numarInregistrare() || undefined,
        beneficiar: this.beneficiar() || undefined,
        executant: this.executant() || undefined,
        cotaTVA: this.cotaTVA(),
        data: this.data(),
        linii: this.linii(),
      })
      .subscribe({
        next: (deviz) => {
          this.saving.set(false);
          this.router.navigate(['/devize', deviz.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Eroare la salvarea devizului. Încearcă din nou.');
        },
      });
  }
}
