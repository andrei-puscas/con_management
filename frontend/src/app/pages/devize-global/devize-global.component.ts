import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DevizeService, type DevizDto } from '../../core/devize.service';
import { AuthService } from '../../core/auth.service';
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
  selector: 'app-devize-global',
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
  templateUrl: './devize-global.component.html',
})
export class DevizeGlobalComponent {
  private devizeService = inject(DevizeService);
  auth = inject(AuthService);

  devize = signal<DevizDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor() {
    this.devizeService.getAll().subscribe({
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
}
