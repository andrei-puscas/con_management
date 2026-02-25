import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  DevizeService,
  type DevizDto,
  type CreateDevizRequest,
  type CreateDevizLinieRequest,
} from '../../../core/devize.service';
import { AuthService } from '../../../core/auth.service';
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
  selector: 'app-devize',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
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
  templateUrl: './devize.component.html',
})
export class DevizeComponent {
  private route = inject(ActivatedRoute);
  private devizeService = inject(DevizeService);
  auth = inject(AuthService);

  proiectId = computed(() => Number(this.route.snapshot.paramMap.get('id')));

  devize = signal<DevizDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  showForm = signal(false);
  saving = signal(false);
  formError = signal<string | null>(null);

  selectedDeviz = signal<DevizDto | null>(null);

  form = signal<CreateDevizRequest>({
    titlu: '',
    numarInregistrare: '',
    beneficiar: '',
    executant: '',
    cotaTVA: 19,
    data: new Date().toISOString().split('T')[0],
    linii: [],
  });

  constructor() {
    this.loadDevize();
  }

  loadDevize(): void {
    this.loading.set(true);
    this.devizeService.getByProiectId(this.proiectId()).subscribe({
      next: (list) => {
        this.devize.set(list ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Eroare la încărcarea devizelor.');
      },
    });
  }

  openForm(): void {
    this.form.set({
      titlu: '',
      numarInregistrare: '',
      beneficiar: '',
      executant: '',
      cotaTVA: 19,
      data: new Date().toISOString().split('T')[0],
      linii: [],
    });
    this.formError.set(null);
    this.showForm.set(true);
    this.selectedDeviz.set(null);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.formError.set(null);
  }

  addLinie(): void {
    const f = this.form();
    const nextNr = f.linii.length + 1;
    this.form.set({
      ...f,
      linii: [
        ...f.linii,
        { numar: nextNr, descriere: '', um: 'buc', cantitate: 0, pretUnitar: 0 },
      ],
    });
  }

  removeLinie(index: number): void {
    const f = this.form();
    const linii = f.linii.filter((_, i) => i !== index).map((l, i) => ({
      ...l,
      numar: i + 1,
    }));
    this.form.set({ ...f, linii });
  }

  updateLinie(index: number, field: keyof CreateDevizLinieRequest, value: string | number): void {
    const f = this.form();
    const linii = f.linii.map((l, i) =>
      i === index ? { ...l, [field]: field === 'descriere' || field === 'um' ? value : Number(value) } : l
    );
    this.form.set({ ...f, linii });
  }

  totalLinie(l: CreateDevizLinieRequest): number {
    return l.cantitate * l.pretUnitar;
  }

  totalGeneral(): number {
    return this.form().linii.reduce((s, l) => s + l.cantitate * l.pretUnitar, 0);
  }

  saveDeviz(): void {
    const f = this.form();
    if (!f.titlu.trim()) {
      this.formError.set('Titlul devizului este obligatoriu.');
      return;
    }
    this.saving.set(true);
    this.formError.set(null);

    this.devizeService.createForProiect(this.proiectId(), f).subscribe({
      next: () => {
        this.saving.set(false);
        this.showForm.set(false);
        this.loadDevize();
      },
      error: () => {
        this.saving.set(false);
        this.formError.set('Eroare la salvarea devizului.');
      },
    });
  }

  viewDeviz(d: DevizDto): void {
    this.selectedDeviz.set(d);
    this.showForm.set(false);
  }

  closeView(): void {
    this.selectedDeviz.set(null);
  }

  downloadPdf(d: DevizDto): void {
    const url = this.devizeService.getPdfUrl(d.id);
    const token = this.auth.getToken();
    fetch(url, { headers: { Authorization: `Bearer ${token}` } })
      .then((res) => res.blob())
      .then((blob) => {
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = `deviz_${d.titlu.replace(/\s+/g, '_')}.pdf`;
        a.click();
        URL.revokeObjectURL(a.href);
      });
  }

  deleteDeviz(d: DevizDto): void {
    if (!confirm(`Ștergi devizul „${d.titlu}"?`)) return;
    this.devizeService.delete(this.proiectId(), d.id).subscribe({
      next: () => {
        if (this.selectedDeviz()?.id === d.id) this.selectedDeviz.set(null);
        this.loadDevize();
      },
    });
  }

  updateFormField(field: keyof CreateDevizRequest, value: string): void {
    this.form.set({ ...this.form(), [field]: value });
  }
}
